using System;
using System.Collections.Generic;
using System.Text;

using NumericOverflow.Bot.Services;

namespace NumericOverflow.Bot.Models
{
    public class TopicStepState : DialogStepState
    {
		public enum Status
		{
			Initialized,
			FindTopic,
			FindTopicParameter,
		}

		public TopicStepState() : base(typeof(TopicBot), 0, 3)
		{
		}

		public Status CurrentStatus { get; set; }
		public string CurrentTopicId { get; set; }
		public string CurrentTopicInput { get; set; }
		public string CurrentTopicParameterId { get; set; }
		public List<TopicParameter> TopicParameters { get; set; }
		public List<TopicParameter> ResolvedParameters { get; set; }
	}
}
