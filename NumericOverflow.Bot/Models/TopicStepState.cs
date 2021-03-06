﻿using System;
using System.Collections.Generic;
using System.Text;

using NumericOverflow.Bot.Services;

namespace NumericOverflow.Bot.Models
{
    public class TopicStepState : DialogStepState
    {
		public enum Status : int
		{
			Initialized = 0,
			FindTopic = 1,
			FindTopicParameter = 2,
		}

		public TopicStepState() : base(typeof(TopicBot), 0, 3)
		{
			this.TopicParameters = new List<TopicParameter>();
			this.ResolvedParameters = new List<TopicParameter>();
		}

		public Status CurrentStatus { get; set; }
		public string CurrentTopicId { get; set; }
		public string CurrentTopicInput { get; set; }
		public string CurrentTopicParameterId { get; set; }
		public List<TopicParameter> TopicParameters { get; set; }
		public List<TopicParameter> ResolvedParameters { get; set; }
	}
}
