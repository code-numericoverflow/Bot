using System;
using System.Collections.Generic;
using System.Text;

using NumericOverflow.Bot.Services;

namespace NumericOverflow.Bot.Models
{
    public class DateParameterState : DialogStepState
    {

		public string Id { get; set; }
		public DateTime Value { get; set; }
		public string Context { get; set; }

		public DateParameterState() : base(typeof(DateParameterBot)) { }

	}
}
