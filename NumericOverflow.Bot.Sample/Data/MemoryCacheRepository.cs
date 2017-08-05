using System;
using Microsoft.Extensions.Caching.Memory;

using NumericOverflow.Bot.Models;

namespace NumericOverflow.Bot.Sample.Data
{
	public class MemoryCacheRepository : IDialogStateRepository
	{
		private TimeSpan AbsoluteExpirationRelativeToNow { get; set; }
		private TimeSpan SlidingExpiration { get; set; }

		private IMemoryCache MemoryCache { get; set; }

		public MemoryCacheRepository(IMemoryCache memoryCache)
			: this(memoryCache, TimeSpan.FromHours(2), TimeSpan.FromMinutes(30))
		{
		}

		public MemoryCacheRepository(IMemoryCache memoryCache, 
									TimeSpan absoluteExpirationRelativeToNow,
									TimeSpan lidingExpiration)
		{
			this.MemoryCache = memoryCache;
			this.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
			this.SlidingExpiration = lidingExpiration;
		}

		void IDialogStateRepository.Set(string id, DialogState dialogState)
		{
			this.Set<DialogState>(id, dialogState);
		}

		DialogState IDialogStateRepository.Get(string id)
		{
			return this.Get<DialogState>(id);
		}

		private void Set<T>(string id, T obj)
		{
			this.MemoryCache.Set<T>(id, obj, new MemoryCacheEntryOptions()
			{
				AbsoluteExpirationRelativeToNow = this.AbsoluteExpirationRelativeToNow,
				SlidingExpiration = this.SlidingExpiration,
			});
		}

		private T Get<T>(string id)
		{
			return this.MemoryCache.Get<T>(id);
		}

	}
}
