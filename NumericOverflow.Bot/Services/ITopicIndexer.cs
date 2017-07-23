using System;
using System.Collections.Generic;
using System.Text;

using NumericOverflow.Bot.Models;

namespace NumericOverflow.Bot.Services
{
    public interface ITopicIndexer
    {
		IEnumerable<Tuple<Topic, int>> GetBestScoredTopicsFor(string inputText);
    }
}
