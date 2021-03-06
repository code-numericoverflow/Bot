﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using FullTextIndexer.Common.Lists;
using FullTextIndexer.Common.Logging;
using FullTextIndexer.Core.Indexes;
using FullTextIndexer.Core.Indexes.TernarySearchTree;
using FullTextIndexer.Core.IndexGenerators;
using FullTextIndexer.Core.TokenBreaking;

using NumericOverflow.Bot.Data;
using NumericOverflow.Bot.Models;
using NumericOverflow.Bot.Services;

namespace NumericOverflow.Bot.Indexers.Services
{
	public class TextSearchTopicIndexer : ITopicIndexer
	{
		private int MinInputLength = 3;

		private IndexData<string> IndexData { get; set; }
		private ITopicRepository TopicRepository { get; set; }

		public TextSearchTopicIndexer(ITopicRepository topicRepository)
		{
			this.TopicRepository = topicRepository;
			var indexGenerator = GetTopicIndexGenerator();
			this.IndexData = indexGenerator.Generate(new NonNullImmutableList<Topic>(this.TopicRepository.GetTopics()));
		}

		public IEnumerable<Tuple<Topic, int>> GetBestScoredTopicsFor(string inputText)
		{
			if (inputText.Length > MinInputLength)
			{
				var matches = this.IndexData.GetPartialMatches<string>(
								inputText,
								GetTokenBreaker(),
								(tokenMatches, allTokens) => (tokenMatches.Count < allTokens.Count - 1)
									? 0
									: tokenMatches.Sum(m => m.Weight)
								)
								.Take(10)
								.OrderByDescending(match => match.Weight);
				var bestTopics = matches.Select(match => new Tuple<Topic, int>(
					this.TopicRepository.GetTopics().Where(t => t.Id == match.Key).First(),
					(int)match.Weight)
				);
				return bestTopics;
			}
			else
			{
				return Enumerable.Empty< Tuple<Topic, int>>();
			}
		}

		private static IndexGenerator<Topic, string> GetTopicIndexGenerator()
		{
			var sourceStringComparer = new EnglishPluralityStringNormaliser(
				new DefaultStringNormaliser(),
				EnglishPluralityStringNormaliser.PreNormaliserWorkOptions.PreNormaliserLowerCases
				| EnglishPluralityStringNormaliser.PreNormaliserWorkOptions.PreNormaliserTrims
			);

			var stopWords = FullTextIndexer.Core.Constants.GetStopWords("en");
			var contentRetrievers = new List<ContentRetriever<Topic, String>>();
			contentRetrievers.Add(new ContentRetriever<Topic, String>(
				p => new PreBrokenContent<String>(p.Id, p.Title),
				token => (stopWords.Contains(token, sourceStringComparer) ? 0.01f : 1f) * 5f
			));
			contentRetrievers.Add(new ContentRetriever<Topic, String>(
				p => new PreBrokenContent<String>(p.Id, p.Content),
				token => stopWords.Contains(token, sourceStringComparer) ? 0.01f : 1f
			));

			return new IndexGenerator<Topic, string>(
				contentRetrievers.ToNonNullImmutableList(),
				new DefaultEqualityComparer<string>(),
				sourceStringComparer,
				GetTokenBreaker(),
				weightedValues => weightedValues.Sum(),
				new NullLogger()
			);
		}

		private static ITokenBreaker GetTokenBreaker()
		{
			return new WhiteSpaceExtendingTokenBreaker(
				new ImmutableList<char>(new[] {
					'<', '>', '[', ']', '(', ')', '{', '}',
					'.', ',', ':', ';', '"', '?', '!',
					'/', '\\',
					'@', '+', '|', '='
				}),
				new WhiteSpaceTokenBreaker()
			);
		}

	}
}
