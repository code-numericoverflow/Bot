using System;
using System.Collections.Generic;
using System.Text;

using NumericOverflow.Bot.Models;

namespace NumericOverflow.Bot.Services
{
	public class RedirectedEventArgs : EventArgs
	{
		public DialogStepState NewStep { get; private set; }
		public BotRequest BotRequest { get; private set; }

		public RedirectedEventArgs(DialogStepState newStep, BotRequest botRequest)
		{
			this.NewStep = newStep;
			this.BotRequest = botRequest;
		}
	}
	public delegate void RedirectedEventHandler(IBot sender, RedirectedEventArgs e);

	public class FinalizedEventArgs : EventArgs
	{
		public BotRequest BotRequest { get; private set; }

		public FinalizedEventArgs(BotRequest botRequest)
		{
			this.BotRequest = botRequest;
		}
	}
	public delegate void FinalizedEventHandler(IBot sender, FinalizedEventArgs e);

	public class ErrorCompletedEventArgs : EventArgs
	{
		public BotRequest BotRequest { get; private set; }

		public ErrorCompletedEventArgs(BotRequest botRequest)
		{
			this.BotRequest = botRequest;
		}
	}
	public delegate void ErrorCompletedEventHandler(IBot sender, ErrorCompletedEventArgs e);
}
