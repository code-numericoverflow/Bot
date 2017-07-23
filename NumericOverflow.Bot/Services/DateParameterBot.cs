using System;
using System.Collections.Generic;
using System.Text;
using NumericOverflow.Bot.Models;

namespace NumericOverflow.Bot.Services
{
    public class DateParameterBot : BotBase
    {
		public override void PipeIn(BotRequest botRequest, ref bool nextPipe)
		{
			var currentState = botRequest.DialogState.CurrentState as DateParameterState;
			var input = botRequest.InputText.ToLowerInvariant() + currentState.Context;

			DateTime dateTime;
			if (DateTime.TryParse(input, out dateTime))
			{
				this.OnSelected(botRequest, dateTime);
			}
			else if (input.Contains("today"))
			{
				this.OnSelected(botRequest, DateTime.Today);
			}
			else if (input.Contains("tomorrow"))
			{
				this.OnSelected(botRequest, DateTime.Today.AddDays(1));
			}
			else if (input.Contains("yesterday"))
			{
				this.OnSelected(botRequest, DateTime.Today.AddDays(-1));
			}
			else
			{
				base.AddError(botRequest);
			}
		}

		private void OnSelected(BotRequest botRequest, DateTime value)
		{
			var currentState = botRequest.DialogState.CurrentState as DateParameterState;
			currentState.Value = value;
			currentState.ErrorCount = 0;
			botRequest.Bag = currentState.Id + "=" + value.ToString() + ";";
			this.OnFinalize(botRequest);
		}
	}
}
