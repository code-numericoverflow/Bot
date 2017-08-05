using System;
using System.Linq;
using System.Collections.Generic;
using NumericOverflow.Bot.Models;
using NumericOverflow.Bot.Data;

namespace NumericOverflow.Bot.Services
{
	public class TopicBot : BotBase
	{

		private ITopicIndexer TopicIndexer { get; set; }
		private ITopicParameterRepository TopicParameterRepository { get; set; }
		private IDialogTextRepository DialogTextRepository { get; set; }

		public TopicBot(ITopicIndexer topicIndexer, ITopicParameterRepository topicParameterRepository, IDialogTextRepository dialogTextRepository)
		{
			this.TopicIndexer = topicIndexer;
			this.TopicParameterRepository = topicParameterRepository;
			this.DialogTextRepository = dialogTextRepository;
		}

		public override void PipeIn(BotRequest botRequest, ref bool nextPipe)
		{
			var stepState = this.GetCurrentState(botRequest.DialogState);
			if (stepState == null)
			{
				throw new InvalidCastException();
			}
			if (stepState.CurrentStatus == TopicStepState.Status.Initialized)
			{
				this.GetInitialResponseFor(botRequest);
			}
			else if (stepState.CurrentStatus == TopicStepState.Status.FindTopic)
			{
				// Tomar los topics según inputtext
				var bestTopics = this.TopicIndexer.GetBestScoredTopicsFor(botRequest.InputText);
				var bestTopicsCount = bestTopics.Count();
				if (bestTopicsCount == 0)
				{
					this.GetErrorResponseFor(botRequest);
				}
				else if (bestTopicsCount == 1)
				{
					this.GetTopicSelectedResponseFor(botRequest, bestTopics.First().Item1);
				}
				else
				{
					this.GetTopicChoicesResponseFor(botRequest, bestTopics.Select(t => t.Item1));
				}
			}
		}

		public override void PipeOut(BotRequest botRequest, ref bool nextPipe)
		{
			var stepState = this.GetCurrentState(botRequest.DialogState);
			if (stepState.CurrentStatus == TopicStepState.Status.FindTopicParameter)
			{
				var topicParameter = botRequest.Bag as TopicParameter;
				if (topicParameter != null)
				{
					if (! stepState.ResolvedParameters.Select(p => p.Id).Contains(topicParameter.Id) &&
						 stepState.TopicParameters.Select(p => p.Id).Contains(topicParameter.Id))
					{
						stepState.ResolvedParameters.Add(topicParameter);
						stepState.ErrorCount = 0;
					}
					this.ResolveParameters(botRequest);
				}
				else
				{
					this.AddError(botRequest);
				}
			}
		}

		public virtual void GetDefaultResponseFor(BotRequest botRequest)
		{
			botRequest.DialogState = botRequest.DialogState;
			botRequest.OutText = "";
			botRequest.Choices = null;
		}

		public virtual void GetInitialResponseFor(BotRequest botRequest)
		{
			var currentState = GetCurrentState(botRequest.DialogState);
			currentState.CurrentStatus = TopicStepState.Status.FindTopic;
			currentState.ErrorCount = 0;
			botRequest.OutText = this.DialogTextRepository.GetDialogTextFor("Bot Presentation") + "\r\n" +
				this.DialogTextRepository.GetDialogTextFor("Select Topic");
		}

		public virtual void GetErrorResponseFor(BotRequest botRequest)
		{
			var currentState = GetCurrentState(botRequest.DialogState);
			currentState.ErrorCount++;
			botRequest.OutText = this.DialogTextRepository.GetDialogTextFor("Unknown Topic") + "\r\n" +
				this.DialogTextRepository.GetDialogTextFor("Select Topic");
		}

		public virtual void GetTopicSelectedResponseFor(BotRequest botRequest, Topic topic)
		{
			var currentState = GetCurrentState(botRequest.DialogState);
			currentState.ErrorCount = 0;
			currentState.CurrentTopicId = topic.Id;
			currentState.CurrentTopicInput = botRequest.InputText;
			currentState.CurrentStatus = TopicStepState.Status.FindTopicParameter;
			currentState.TopicParameters = this.GetParametersFor(currentState.CurrentTopicId).ToList();
			currentState.ResolvedParameters = new List<TopicParameter>();
			botRequest.OutText = string.Format(this.DialogTextRepository.GetDialogTextFor("Topic Selected"), topic.Title);
			this.ResolveParameters(botRequest);
		}

		public virtual void GetTopicChoicesResponseFor(BotRequest botRequest, IEnumerable<Topic> topics)
		{
			var currentState = GetCurrentState(botRequest.DialogState);
			currentState.ErrorCount = 0;
			currentState.CurrentStatus = TopicStepState.Status.FindTopic;
			botRequest.OutText = this.DialogTextRepository.GetDialogTextFor("Select Choice");
			botRequest.Choices = this.GetChoicesFor(topics).ToList();
		}

		public virtual TopicStepState GetCurrentState(DialogState dialogState)
		{
			return dialogState.CurrentState as TopicStepState;
		}

		public virtual IEnumerable<ChoiceItem> GetChoicesFor(IEnumerable<Topic> topics)
		{
			return topics.Select(t => new ChoiceItem(t.Id, t.Title));
		}

		public virtual IEnumerable<TopicParameter> GetParametersFor(string topicId)
		{
			return this.TopicParameterRepository.GetTopicParameters(topicId);
		}

		public virtual void ResolveParameters(BotRequest botRequest)
		{
			var currentState = GetCurrentState(botRequest.DialogState);
			var parameterToResolve = currentState.TopicParameters.Where(p => !currentState.ResolvedParameters.Select(r => r.Id).Contains(p.Id)).FirstOrDefault();
			if (parameterToResolve == null)
			{
				// We have all the parameters => Run response
				botRequest.OutText = this.GetCompletedText(botRequest);
				currentState.CurrentStatus = TopicStepState.Status.Initialized;
				this.OnFinalize(botRequest);
			}
			else
			{
				this.RedirectToParameter(parameterToResolve, botRequest);
			}
		}

		public virtual void RedirectToParameter(TopicParameter parameterToResolve, BotRequest botRequest)
		{
			if (parameterToResolve.Type == typeof(DateTime))
			{
				var currentState = GetCurrentState(botRequest.DialogState);
				this.OnRedirect(new DateParameterState() { Id = parameterToResolve.Id, Context = currentState.CurrentTopicInput }, botRequest);
			}
		}

		public virtual string GetCompletedText(BotRequest botRequest)
		{
			var currentState = GetCurrentState(botRequest.DialogState);
			string parameters = string.Join("\r\n", currentState.ResolvedParameters.Select(p => p.Id + "=" + p.Value.ToString()));
			return string.Format(this.DialogTextRepository.GetDialogTextFor("Topic Completed {0} {1}"), currentState.CurrentTopicId, parameters);
		}
	}
}
