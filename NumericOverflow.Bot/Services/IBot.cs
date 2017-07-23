using System;
using System.Collections.Generic;
using System.Text;

using NumericOverflow.Bot.Models;

namespace NumericOverflow.Bot.Services
{
    public interface IBot
    {
		void PipeIn(BotRequest botRequest, ref bool nextPipe);
		void PipeOut(BotRequest botRequest, ref bool nextPipe);
		event RedirectedEventHandler Redirected;
		event FinalizedEventHandler Finalized;
		event ErrorCompletedEventHandler ErrorCompleted;
	}
}
