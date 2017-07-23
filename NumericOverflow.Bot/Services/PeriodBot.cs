using System;
using System.Collections.Generic;
using System.Text;
using NumericOverflow.Bot.Models;

namespace NumericOverflow.Bot.Services
{
	public class PeriodBot : IBot
	{
		public event RedirectedEventHandler Redirected;
		public event FinalizedEventHandler Finalized;
		public event ErrorCompletedEventHandler ErrorCompleted;

		public void AddToDialog(DialogState dialogState)
		{
			throw new NotImplementedException();
		}

		public virtual void PipeIn(BotRequest botRequest, ref bool nextPipe)
		{
			throw new NotImplementedException();
		}

		public virtual void PipeOut(BotRequest botRequest, ref bool nextPipe)
		{
			throw new NotImplementedException();
		}
	}
}
