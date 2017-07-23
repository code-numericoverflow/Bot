using System;
using System.Collections.Generic;
using System.Text;

namespace NumericOverflow.Bot.Models
{
    public class BotRequest
    {
		public BotRequest(DialogState dialogState)
		{
			this.DialogState = dialogState;
		}

		public string InputText { get; set; }
		public ChoiceItem SelectedChoice { get; set; }
		public DialogState DialogState { get; set; }
		public string OutText { get; set; }
		public List<ChoiceItem> Choices { get; set; }
		public string Bag { get; set; }
	}
}
