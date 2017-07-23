using System;
using System.Collections.Generic;
using System.Text;

namespace NumericOverflow.Bot.Models
{
    public class TopicParameter
    {
		public TopicParameter(Type type, string id, string title, string description, bool required, object @default, object value)
		{
			this.Type = type;
			this.Id = id;
			this.Title = title;
			this.Description = description;
			this.Required = required;
			this.Default = @default;
			this.Value = value;
		}

		public Type Type { get; private set; }
		public string Id { get; private set; }
		public string Title { get; private set; }
		public string Description { get; private set; }
		public bool Required { get; private set; }
		public object Default { get; private set; }
		public object Value { get; private set; }
    }
}
