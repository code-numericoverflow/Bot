using System;

namespace NumericOverflow.Bot.Models
{
    public class DialogStepState
	{
		public DialogStepState()
		{
			this.MaxError = 3;
			this.ErrorCount = 0;
		}

		public DialogStepState(Type botType) : this()
		{
			this.BotTypeQualifiedName = botType.AssemblyQualifiedName;
		}

		public DialogStepState(Type botType, int errorCount, int maxError)
		{
			this.BotTypeQualifiedName = botType.AssemblyQualifiedName;
			this.ErrorCount = errorCount;
			this.MaxError = maxError;
		}

		public string BotTypeQualifiedName { get; set; }
		public int ErrorCount { get; set; }
		public int MaxError { get; set; }
    }
}
