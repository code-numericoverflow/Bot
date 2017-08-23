using System;
using System.Collections.Generic;
using System.Text;

namespace NumericOverflow.Bot.Models
{
    public class TopicParameter
    {
		public TopicParameter()
		{
			this.Choices = new List<ChoiceItem>();
		}

		public TopicParameter(string typeName, string id, string title, string description, bool required, object @default, object value)
			: this()
		{
			this.TypeName = typeName;
			this.Id = id;
			this.Title = title;
			this.Description = description;
			this.Required = required;
			this.Default = @default;
			this.Value = value;
		}

		public string TopicId { get; set; }
		public string Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public bool Required { get; set; }
		public object Default { get; set; }
		public object Value { get; set; }
		public string TypeName { get; set; }
		public List<ChoiceItem> Choices { get; set; }
	}
}
