using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

using NumericOverflow.Bot.Models;
using NumericOverflow.Bot.Data;

namespace NumericOverflow.Bot.Indexers.Data
{
	public class JsonTopicRepository : ITopicRepository
	{
		private IList<Topic> Topics { get; set; }

		private JsonTopicRepository()
		{
		}

		public JsonTopicRepository(string jsonFilePath)
		{
			if (this.Topics == null)
			{
				this.Topics = JsonConvert.DeserializeObject<IList<Topic>>(File.ReadAllText(jsonFilePath));
			}
		}

		public Topic GetTopic(string topicId)
		{
			return this.Topics.Where(t => t.Id == topicId).FirstOrDefault();
		}

		public IEnumerable<Topic> GetTopics()
		{
			return this.Topics;
		}
	}
}
