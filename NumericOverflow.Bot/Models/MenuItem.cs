using System;
using System.Collections.Generic;
using System.Text;

namespace NumericOverflow.Bot.Models
{
    public class MenuItem
    {

		public MenuItem(string id, string description, Action<BotRequest> action)
		{
			this.ChoiceItem = new ChoiceItem(id, description);
			this.Action = action;
		}

		public ChoiceItem ChoiceItem { get; private set; }
		public Action<BotRequest> Action { get; private set; }
	}
}