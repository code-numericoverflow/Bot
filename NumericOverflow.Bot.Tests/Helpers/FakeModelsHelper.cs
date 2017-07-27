using System;
using System.Collections.Generic;

using NumericOverflow.Bot.Models;

namespace NumericOverflow.Bot.Tests.Helpers
{
	public class FakeModelsHelper
	{
		public static IEnumerable<Tuple<Topic, int>> GetFakeIndexedTopics(int count)
		{
			var indexedTopics = new List<Tuple<Topic, int>>();
			for (var i = 0; i < count; i++)
			{
				yield return new Tuple<Topic, int>(
					new Topic(i.ToString(), i.ToString(), i.ToString()),
					100 - i);
			}
		}

		public static TopicParameter GetSampleParameter(string id)
		{
			return new TopicParameter(typeof(DateTime), id, "", "", false, DateTime.Today, DateTime.Today);
		}
	}
}
