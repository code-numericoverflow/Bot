using System;
using System.Collections.Generic;
using System.Text;

using NumericOverflow.Bot.Models;

namespace NumericOverflow.Bot.Data
{
    public interface IChoiceRepository
    {
		IEnumerable<ChoiceItem> GetChoices(string topicParameterId);
    }
}
