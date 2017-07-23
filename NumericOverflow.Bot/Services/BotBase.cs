using System;
using System.Collections.Generic;
using System.Text;
using NumericOverflow.Bot.Models;

namespace NumericOverflow.Bot.Services
{
    public abstract class BotBase : IBot
    {
		public virtual event RedirectedEventHandler Redirected;
		public virtual event FinalizedEventHandler Finalized;
		public virtual event ErrorCompletedEventHandler ErrorCompleted;

		public virtual void PipeIn(BotRequest botRequest, ref bool nextPipe)
		{
		}

		public virtual void PipeOut(BotRequest botRequest, ref bool nextPipe)
		{
		}

		protected virtual void AddError(BotRequest botRequest)
		{
			botRequest.DialogState.CurrentState.ErrorCount++;
			if (botRequest.DialogState.CurrentState.ErrorCount >= botRequest.DialogState.CurrentState.MaxError)
			{
				this.OnError(botRequest);
			}
		}

		public virtual void OnRedirect(DialogStepState newStep, BotRequest botRequest)
		{
			if (this.Redirected != null)
			{
				this.Redirected.Invoke(this, new RedirectedEventArgs(newStep, botRequest));
			}
		}

		public virtual void OnFinalize(BotRequest botRequest)
		{
			if (this.Finalized != null)
			{
				this.Finalized.Invoke(this, new FinalizedEventArgs(botRequest));
			}
		}

		public virtual void OnError(BotRequest botRequest)
		{
			if (this.ErrorCompleted != null)
			{
				this.ErrorCompleted.Invoke(this, new ErrorCompletedEventArgs(botRequest));
			}
		}

	}
}
