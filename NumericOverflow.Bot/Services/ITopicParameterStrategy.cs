using System;
using System.Collections.Generic;
using System.Text;
using NumericOverflow.Bot.Models;

namespace NumericOverflow.Bot.Services
{
    public interface ITopicParameterStrategy
    {
		void SelectStrategy(TopicParameter topicParameter, BotRequest botRequest);
    }
}
