using System;
using System.Collections.Generic;
using System.Text;

namespace NumericOverflow.Bot.Models
{
	public class Topic
	{
		public Topic(string id, string title, string content)
		{
			this.Id = id;
			this.Title = title;
			this.Content = content;
		}

		public string Id { get; private set; }
		public string Title { get; private set; }
		public string Content { get; private set; }
    }
}
