using System;
using System.Collections.Generic;
using System.Text;

namespace NumericOverflow.Bot.Services
{
    public interface IResolver
    {
		object Resolve(Type type);
    }
}
