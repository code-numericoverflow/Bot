using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeItEasy;

using NumericOverflow.Bot.Models;
using NumericOverflow.Bot.Data;
using NumericOverflow.Bot.Services;
using NumericOverflow.Bot.Tests.Helpers;

namespace NumericOverflow.Bot.Tests.Services
{
	[TestClass]
	public class TopicBotTests
	{
		private bool NoNextPipe = false;
		private ITopicIndexer TopicIndexer { get; set; }
		private ITopicParameterRepository TopicParameterRepository { get; set; }
		private IDialogTextRepository DialogTextRepository { get; set; }
		private IBot SUT { get; set; }

		public TopicBotTests()
		{
			this.TopicIndexer = A.Fake<ITopicIndexer>();
			this.DialogTextRepository = A.Fake<IDialogTextRepository>();
			this.TopicParameterRepository = A.Fake<ITopicParameterRepository>();
			this.DialogTextRepository = A.Fake<IDialogTextRepository>();
			this.SUT = new TopicBot(this.TopicIndexer, this.TopicParameterRepository, this.DialogTextRepository);
		}

		[TestMethod]
		public void ShouldRespondWithInitialResponseWhenInitialStatus()
		{
			var botRequest = this.GetDefaultTopicBotRequest(TopicStepState.Status.Initialized, 1);
			botRequest.InputText = "A";

			SUT.PipeIn(botRequest, ref this.NoNextPipe);

			var topicStepState = botRequest.DialogState.GetLastStep() as TopicStepState;
			Assert.AreEqual(TopicStepState.Status.FindTopic, topicStepState.CurrentStatus);
			Assert.AreEqual(0, topicStepState.ErrorCount);
			Assert.AreEqual("", topicStepState.CurrentTopicInput);
		}

		[TestMethod]
		public void ShouldRespondWithErrorWhenNoTopicIsIndexed()
		{
			var botRequest = this.GetDefaultTopicBotRequest(TopicStepState.Status.FindTopic, 0);
			botRequest.InputText = "A";

			SUT.PipeIn(botRequest, ref this.NoNextPipe);

			var topicStepState = botRequest.DialogState.GetLastStep() as TopicStepState;
			Assert.AreEqual(TopicStepState.Status.FindTopic, topicStepState.CurrentStatus);
			Assert.AreEqual(1, topicStepState.ErrorCount);
			Assert.AreEqual("", topicStepState.CurrentTopicInput);
		}

		[TestMethod]
		public void ShouldRespondWithSelectionWhenOnlyOneTopicIsIndexed()
		{
			A.CallTo(() => this.TopicIndexer.GetBestScoredTopicsFor("A")).Returns(FakeModelsHelper.GetFakeIndexedTopics(1));
			var botRequest = this.GetDefaultTopicBotRequest(TopicStepState.Status.FindTopic, 0);
			botRequest.InputText = "A";

			SUT.PipeIn(botRequest, ref this.NoNextPipe);

			var topicStepState = botRequest.DialogState.GetLastStep() as TopicStepState;
			Assert.IsTrue(TopicStepState.Status.FindTopicParameter == topicStepState.CurrentStatus
							|| TopicStepState.Status.Initialized == topicStepState.CurrentStatus);
			Assert.AreEqual(0, topicStepState.ErrorCount);
			Assert.AreEqual("A", topicStepState.CurrentTopicInput);
		}

		[TestMethod]
		public void ShouldRespondWithChoicesWhenTwoOrMoreTopicsAreIndexed()
		{
			A.CallTo(() => this.TopicIndexer.GetBestScoredTopicsFor("A")).Returns(FakeModelsHelper.GetFakeIndexedTopics(3));
			var botRequest = this.GetDefaultTopicBotRequest(TopicStepState.Status.FindTopic, 0);
			botRequest.InputText = "A";

			SUT.PipeIn(botRequest, ref this.NoNextPipe);

			var topicStepState = botRequest.DialogState.GetLastStep() as TopicStepState;
			Assert.AreEqual(TopicStepState.Status.FindTopic, topicStepState.CurrentStatus);
			Assert.AreEqual(3, botRequest.Choices.Count());
			Assert.AreEqual("", topicStepState.CurrentTopicInput);
		}

		[TestMethod]
		public void ShouldAddErrorWhenFindTopicParameterWithNoTopicParameterInBag()
		{
			var botRequest = this.GetDefaultTopicBotRequest(TopicStepState.Status.FindTopicParameter, 0);
			botRequest.Bag = null;

			SUT.PipeOut(botRequest, ref this.NoNextPipe);

			Assert.AreEqual(1, botRequest.DialogState.CurrentState.ErrorCount);
		}

		[TestMethod]
		public void ShouldResolveParameterWhenFindTopicParameterWithTopicParameterInBag()
		{
			var botRequest = this.GetDefaultTopicBotRequest(TopicStepState.Status.FindTopicParameter, 0);
			var topicStepState = botRequest.DialogState.GetLastStep() as TopicStepState;
			topicStepState.TopicParameters.Add(FakeModelsHelper.GetSampleParameter("1"));
			botRequest.Bag = FakeModelsHelper.GetSampleParameter("1");

			SUT.PipeOut(botRequest, ref this.NoNextPipe);

			Assert.AreEqual(0, topicStepState.ErrorCount);
			Assert.AreEqual(1, topicStepState.ResolvedParameters.Count);
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

	}
}
