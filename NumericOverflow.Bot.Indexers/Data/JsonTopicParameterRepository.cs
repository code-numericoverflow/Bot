using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

using NumericOverflow.Bot.Models;
using NumericOverflow.Bot.Data;

namespace NumericOverflow.Bot.Indexers.Data
{
	public class JsonTopicParameterRepository : ITopicParameterRepository
	{
		private IList<TopicParameter> TopicParameters { get; set; }

		private JsonTopicParameterRepository()
		{
		}

		public JsonTopicParameterRepository(string jsonFilePath)
		{
			if (this.TopicParameters == null)
			{
				this.TopicParameters = JsonConvert.DeserializeObject<IList<TopicParameter>>(File.ReadAllText(jsonFilePath));
			}
		}

		public IEnumerable<TopicParameter> GetTopicParameters(string topicId)
		{
			return this.TopicParameters.Where(p => p.TopicId == topicId);
		}
	}
}
