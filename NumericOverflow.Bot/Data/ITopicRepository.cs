using System.Collections.Generic;

using NumericOverflow.Bot.Models;

namespace NumericOverflow.Bot.Data
{
    public interface ITopicRepository
    {
		Topic GetTopic(string Id);
		IEnumerable<Topic> GetTopics();
	}
}
