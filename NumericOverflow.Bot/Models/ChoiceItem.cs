using System;
using System.Collections.Generic;
using System.Text;

namespace NumericOverflow.Bot.Models
{
    public class ChoiceItem
	{
		public ChoiceItem(string id, string description)
		{
			this.Id = id;
			this.Description = description;
		}

		public string Id { get; private set; }
		public string Description { get; private set; }
    }
}
