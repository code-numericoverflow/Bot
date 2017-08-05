using System;

using NumericOverflow.Bot.Services;

namespace NumericOverflow.Bot.Sample.Services
{
	public class Resolver : IResolver
	{

		public IServiceProvider ServiceProvider { get; set; }

		public Resolver(IServiceProvider serviceProvider)
		{
			this.ServiceProvider = serviceProvider;
		}

		public object Resolve(Type type)
		{
			return this.ServiceProvider.GetService(type);
		}
	}
}
