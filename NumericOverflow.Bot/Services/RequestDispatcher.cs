using System.Linq;
using NumericOverflow.Bot.Models;

namespace NumericOverflow.Bot.Services
{
	public class RequestDispatcher : IRequestDispatcher
	{

		private IResolver Resolver { get; set; }

		public RequestDispatcher(IResolver resolver)
		{
			this.Resolver = resolver;
		}

		public virtual void Dispatch(BotRequest botRequest)
		{
			var steps = botRequest.DialogState.GetSteps().ToArray();
			this.PipeSteps(botRequest, steps, true);
			this.PipeSteps(botRequest, steps.Reverse().ToArray(), false);
		}

		private void PipeSteps(BotRequest botRequest, DialogStepState[] steps, bool input)
		{
			var nextPipe = true;
			var nextIndex = 1;
			DialogStepState previousStep = null;
			foreach (var botState in steps)
			{
				botRequest.DialogState.PreviousState = previousStep;
				botRequest.DialogState.CurrentState = botState;
				botRequest.DialogState.NextState = nextIndex >= steps.Count() ? null : steps[nextIndex];
				this.Pipe(botRequest, ref nextPipe, input);
				if (!nextPipe)
				{
					break;
				}

				nextIndex++;
				previousStep = botRequest.DialogState.CurrentState;
			}
		}

		private void Pipe(BotRequest botRequest, ref bool nextPipe, bool input)
		{
			var currentStep = botRequest.DialogState.CurrentState;
			var bot = this.Resolver.Resolve(currentStep.BotType) as IBot;
			bot.Redirected += Bot_Redirected;
			bot.Finalized += Bot_Finalized;
			bot.ErrorCompleted += Bot_ErrorCompleted;
			if (input)
			{
				bot.PipeIn(botRequest, ref nextPipe);
			}
			else
			{
				bot.PipeOut(botRequest, ref nextPipe);
			}
			bot.ErrorCompleted -= Bot_ErrorCompleted;
			bot.Finalized -= Bot_Finalized;
			bot.Redirected -= Bot_Redirected;
		}

		private void Bot_ErrorCompleted(IBot sender, ErrorCompletedEventArgs  e)
		{
			if (e.BotRequest.DialogState.PreviousState != null)
			{
				this.RemoveFromDialog(e.BotRequest.DialogState.CurrentState, e.BotRequest);
			}
		}

		private void Bot_Finalized(IBot sender, FinalizedEventArgs e)
		{
			this.RemoveFromDialog(e.BotRequest.DialogState.CurrentState, e.BotRequest);
		}

		private void Bot_Redirected(IBot sender, RedirectedEventArgs e)
		{
			this.AddToDialog(e.NewStep, e.BotRequest);
		}

		public void AddToDialog(DialogStepState step, BotRequest botRequest)
		{
			botRequest.DialogState.AddStep(step);
		}

		public void RemoveFromDialog(DialogStepState step, BotRequest botRequest)
		{
			botRequest.DialogState.RemoveStep(step);
		}
	}
}
