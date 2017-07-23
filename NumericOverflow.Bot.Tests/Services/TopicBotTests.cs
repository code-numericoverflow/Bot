using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeItEasy;

using NumericOverflow.Bot.Models;
using NumericOverflow.Bot.Data;
using NumericOverflow.Bot.Services;

namespace NumericOverflow.Bot.Tests.Services
{
	[TestClass]
	public class TopicBotTests
	{
		private bool NoNextPipe = false;

		[TestMethod]
		public void ShouldRespondWithInitialResponseWhenInitialStatus()
		{
			var topicIndexer = A.Fake<ITopicIndexer>();
			IBot sut = new TopicBot(topicIndexer, null);
			var botRequest = this.GetDefaultTopicBotRequest(TopicStepState.Status.Initialized, 1);
			botRequest.InputText = "A";

			sut.PipeIn(botRequest, ref this.NoNextPipe);

			var topicStepState = botRequest.DialogState.GetLastStep() as TopicStepState;
			Assert.AreEqual(TopicStepState.Status.FindTopic, topicStepState.CurrentStatus);
			Assert.AreEqual(0, topicStepState.ErrorCount);
			Assert.AreEqual("", topicStepState.CurrentTopicInput);
		}

		[TestMethod]
		public void ShouldRespondWithErrorWhenNoTopicIsIndexed()
		{
			var topicIndexer = A.Fake<ITopicIndexer>();
			IBot sut = new TopicBot(topicIndexer, null);
			var botRequest = this.GetDefaultTopicBotRequest(TopicStepState.Status.FindTopic, 0);
			botRequest.InputText = "A";

			sut.PipeIn(botRequest, ref this.NoNextPipe);

			var topicStepState = botRequest.DialogState.GetLastStep() as TopicStepState;
			Assert.AreEqual(TopicStepState.Status.FindTopic, topicStepState.CurrentStatus);
			Assert.AreEqual(1, topicStepState.ErrorCount);
			Assert.AreEqual("", topicStepState.CurrentTopicInput);
		}

		[TestMethod]
		public void ShouldRespondWithSelectionWhenOnlyOneTopicIsIndexed()
		{
			var topicIndexer = A.Fake<ITopicIndexer>();
			A.CallTo(() => topicIndexer.GetBestScoredTopicsFor("A")).Returns(this.GetFakeIndexedTopics(1));
			IBot sut = new TopicBot(topicIndexer, A.Fake<ITopicParameterRepository>());
			var botRequest = this.GetDefaultTopicBotRequest(TopicStepState.Status.FindTopic, 0);
			botRequest.InputText = "A";

			sut.PipeIn(botRequest, ref this.NoNextPipe);

			var topicStepState = botRequest.DialogState.GetLastStep() as TopicStepState;
			Assert.AreEqual(TopicStepState.Status.FindTopicParameter, topicStepState.CurrentStatus);
			Assert.AreEqual(0, topicStepState.ErrorCount);
			Assert.AreEqual("A", topicStepState.CurrentTopicInput);
		}

		[TestMethod]
		public void ShouldRespondWithChoicesWhenTwoOrMoreTopicsAreIndexed()
		{
			var topicIndexer = A.Fake<ITopicIndexer>();
			A.CallTo(() => topicIndexer.GetBestScoredTopicsFor("A")).Returns(this.GetFakeIndexedTopics(3));
			IBot sut = new TopicBot(topicIndexer, null);
			var botRequest = this.GetDefaultTopicBotRequest(TopicStepState.Status.FindTopic, 0);
			botRequest.InputText = "A";

			sut.PipeIn(botRequest, ref this.NoNextPipe);

			var topicStepState = botRequest.DialogState.GetLastStep() as TopicStepState;
			Assert.AreEqual(TopicStepState.Status.FindTopic, topicStepState.CurrentStatus);
			Assert.AreEqual(3, botRequest.Choices.Count());
			Assert.AreEqual("", topicStepState.CurrentTopicInput);
		}

		private BotRequest GetDefaultTopicBotRequest(TopicStepState.Status status, int errorCount)
		{
			var step = new TopicStepState()
			{
				CurrentStatus = status,
				CurrentTopicInput = "",
				ErrorCount = errorCount,
			};
			var dialogState = new DialogState();
			dialogState.AddStep(step);
			dialogState.CurrentState = step;
			return new BotRequest(dialogState);
		}

		private IEnumerable<Tuple<Topic, int>> GetFakeIndexedTopics(int count)
		{
			var indexedTopics = new List<Tuple<Topic, int>>();
			for (var i = 0; i < count; i++)
			{
				yield return new Tuple<Topic, int>(
					new Topic(i.ToString(), i.ToString(), i.ToString()),
					100 - i);
			}
		}
	}
}
