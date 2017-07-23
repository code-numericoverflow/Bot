using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using NumericOverflow.Bot.Models;
using NumericOverflow.Bot.Data;

namespace NumericOverflow.Bot.Services
{
	public class TopicBot : BotBase
	{

		private ITopicIndexer TopicIndexer { get; set; }
		private ITopicParameterRepository TopicParameterRepository { get; set; }

		public TopicBot(ITopicIndexer topicIndexer, ITopicParameterRepository topicParameterRepository)
		{
			this.TopicIndexer = topicIndexer;
			this.TopicParameterRepository = topicParameterRepository;
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
			throw new NotImplementedException();
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
			botRequest.OutText = "Dime qué listado quieres";
		}

		public virtual void GetErrorResponseFor(BotRequest botRequest)
		{
			var currentState = GetCurrentState(botRequest.DialogState);
			currentState.ErrorCount++;
			botRequest.OutText = "Dime qué listado quieres";
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
			botRequest.OutText = "Se ha seleccionado el tópico: " + topic.Title;
			this.ResolveParameters(botRequest);
		}

		public virtual void GetTopicChoicesResponseFor(BotRequest botRequest, IEnumerable<Topic> topics)
		{
			var currentState = GetCurrentState(botRequest.DialogState);
			currentState.ErrorCount = 0;
			currentState.CurrentStatus = TopicStepState.Status.FindTopic;
			botRequest.OutText = "Seleccionar de la lista";
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
			}
			else
			{
				this.ResolveParameter(parameterToResolve, botRequest);
			}
		}

		public virtual void ResolveParameter(TopicParameter parameterToResolve, BotRequest botRequest)
		{
			if (parameterToResolve.Type == typeof(DateTime))
			{
				var currentState = GetCurrentState(botRequest.DialogState);
				this.OnRedirect(new DateParameterState() { Id = parameterToResolve.Id, Context = currentState.CurrentTopicInput }, botRequest);
			}
		}
	}
}
