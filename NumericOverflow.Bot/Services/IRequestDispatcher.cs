using System;
using System.Collections.Generic;
using System.Text;

using NumericOverflow.Bot.Models;

namespace NumericOverflow.Bot.Services
{
    public interface IRequestDispatcher
    {
		void Dispatch(BotRequest botRequest);
		void AddToDialog(DialogStepState step, BotRequest botRequest);
		void RemoveFromDialog(DialogStepState step, BotRequest botRequest);
		event FinalizingEventHandler Finalizing;
	}
}
