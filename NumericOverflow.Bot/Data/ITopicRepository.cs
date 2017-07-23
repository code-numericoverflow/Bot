using System;
using System.Collections.Generic;
using System.Text;

using NumericOverflow.Bot.Models;

namespace NumericOverflow.Bot.Data
{
    public interface ITopicRepository
    {
		IEnumerable<Topic> GetTopics();
    }
}
