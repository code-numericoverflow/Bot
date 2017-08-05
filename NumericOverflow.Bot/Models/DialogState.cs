using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace NumericOverflow.Bot.Models
{
    public class DialogState
    {
		public List<DialogStepState> Steps { get; set; }
		// To be set by the dispatcher
		public DialogStepState CurrentState { get; set; }
		public DialogStepState PreviousState { get; set; }
		public DialogStepState NextState { get; set; }

		public DialogState()
		{
			this.Steps = new List<DialogStepState>();
		}

		public void AddStep(DialogStepState dialogStepState)
		{
			Steps.Add(dialogStepState);
		}

		public void AddSteps(IEnumerable<DialogStepState> dialogStepState)
		{
			Steps.AddRange(dialogStepState);
		}

		public void RemoveStep(DialogStepState dialogStepState)
		{
			Steps.Remove(dialogStepState);
		}

		public IEnumerable<DialogStepState> GetSteps()
		{
			return this.Steps;
		}

		public DialogStepState GetLastStep()
		{
			return this.Steps.Last();
		}
	}
}
