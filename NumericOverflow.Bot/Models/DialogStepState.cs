using System;
using System.Collections.Generic;
using System.Text;

namespace NumericOverflow.Bot.Models
{
    public class DialogStepState
    {
		public DialogStepState(Type botType) : this(botType, 0, 3)
		{
		}

		public DialogStepState(Type botType, int errorCount, int maxError)
		{
			this.BotType = botType;
			this.ErrorCount = errorCount;
			this.MaxError = maxError;
		}

		public Type BotType { get; private set; }
		public int ErrorCount { get; set; }
		public int MaxError { get; private set; }
    }
}
