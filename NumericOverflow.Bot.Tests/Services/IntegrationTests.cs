using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeItEasy;

using NumericOverflow.Bot.Data;
using NumericOverflow.Bot.Models;
using NumericOverflow.Bot.Services;
using NumericOverflow.Bot.Tests.Helpers;

namespace NumericOverflow.Bot.Tests.Services
{
	[TestClass]
	public class IntegrationTests
	{

		private IResolver Resolver { get; set; }
		private IRequestDispatcher RequestDispatcher { get; set; }
		private ITopicIndexer TopicIndexer { get; set; }
		private ITopicParameterRepository TopicParameterRepository { get; set; }
		private IDialogTextRepository DialogTextRepository { get; set; }

		public IntegrationTests()
		{
			this.TopicIndexer = A.Fake<ITopicIndexer>();
			this.TopicParameterRepository = A.Fake<ITopicParameterRepository>();
			this.DialogTextRepository = A.Fake<IDialogTextRepository>();
			this.Resolver = A.Fake<IResolver>();
			A.CallTo(() => this.Resolver.Resolve(typeof(FirstMenu))).Returns(new FirstMenu());
			A.CallTo(() => this.Resolver.Resolve(typeof(SecondMenu))).Returns(new SecondMenu());
			A.CallTo(() => this.Resolver.Resolve(typeof(DateParameterBot))).Returns(new DateParameterBot());
			A.CallTo(() => this.Resolver.Resolve(typeof(TopicBot))).Returns(new TopicBot(this.TopicIndexer, this.TopicParameterRepository, this.DialogTextRepository));
			this.RequestDispatcher = new RequestDispatcher(this.Resolver);
		}

		[TestMethod]
		public void ShouldChainActionsWhenCorrectSelectionInMenuBot()
		{
			var state = new DialogState();
			state.AddStep(new MenuStepState(typeof(FirstMenu) ));
			var botRequest = new BotRequest(state);

			this.RequestDispatcher.Dispatch(botRequest);
			botRequest.InputText = "1";
			this.RequestDispatcher.Dispatch(botRequest);
			botRequest.InputText = "b";
			this.RequestDispatcher.Dispatch(botRequest);

			Assert.AreEqual("B", botRequest.OutText);
			Assert.AreEqual(2, botRequest.DialogState.GetSteps().Count());
		}

		[TestMethod]
		public void ShouldRemoveBotWhenFinalizeMenuBot()
		{
			var state = new DialogState();
			state.AddStep(new MenuStepState(typeof(FirstMenu)));
			var botRequest = new BotRequest(state);

			this.RequestDispatcher.Dispatch(botRequest);
			botRequest.InputText = "1";
			this.RequestDispatcher.Dispatch(botRequest);
			Assert.AreEqual(2, botRequest.DialogState.GetSteps().Count());
			botRequest.InputText = "c";
			this.RequestDispatcher.Dispatch(botRequest);

			Assert.AreEqual(1, botRequest.DialogState.GetSteps().Count());
		}

		[TestMethod]
		public void ShouldGetDateParameterWhenRedirectToDate()
		{
			var state = new DialogState();
			state.AddStep(new MenuStepState(typeof(FirstMenu)));
			var botRequest = new BotRequest(state);

			this.RequestDispatcher.Dispatch(botRequest);
			botRequest.InputText = "d";
			this.RequestDispatcher.Dispatch(botRequest);
			Assert.AreEqual(2, botRequest.DialogState.GetSteps().Count());
			botRequest.InputText = "Today";
			this.RequestDispatcher.Dispatch(botRequest);

			var topicParameter = botRequest.Bag as TopicParameter;
			Assert.AreEqual(DateTime.Today, topicParameter.Value);
		}

		[TestMethod]
		public void ShouldGetParameterWhenTopicHasParameter()
		{
			A.CallTo(() => this.TopicIndexer.GetBestScoredTopicsFor("d")).Returns(FakeModelsHelper.GetFakeIndexedTopics(1));
			var parameters = new[] { FakeModelsHelper.GetSampleParameter("0") };
			A.CallTo(() => this.TopicParameterRepository.GetTopicParameters("0")).Returns(parameters);
			var state = new DialogState();
			var topicStepState = new TopicStepState();
			state.AddStep(topicStepState);
			var botRequest = new BotRequest(state);

			botRequest.InputText = "";
			this.RequestDispatcher.Dispatch(botRequest);
			botRequest.InputText = "d";
			this.RequestDispatcher.Dispatch(botRequest);
			botRequest.InputText = "Yesterday";
			this.RequestDispatcher.Dispatch(botRequest);

			var topicParameter = botRequest.Bag as TopicParameter;
			Assert.AreEqual(DateTime.Today.AddDays(-1), topicParameter.Value);
			Assert.AreEqual(0, botRequest.DialogState.GetSteps().Count());
		}

		[TestMethod]
		public void ShouldGetAllParametersWhenTopicHasParameters()
		{
			A.CallTo(() => this.TopicIndexer.GetBestScoredTopicsFor("d")).Returns(FakeModelsHelper.GetFakeIndexedTopics(1));
			var parameters = new[] { FakeModelsHelper.GetSampleParameter("0"), FakeModelsHelper.GetSampleParameter("1") };
			A.CallTo(() => this.TopicParameterRepository.GetTopicParameters("0")).Returns(parameters);
			var state = new DialogState();
			var topicStepState = new TopicStepState();
			state.AddStep(topicStepState);
			var botRequest = new BotRequest(state);
			List<TopicParameter> resolvedParameters = null;
			this.RequestDispatcher.Finalizing += (sender, args) => 
			{
				if (sender.GetType() == typeof(TopicBot))
				{
					var currentState = args.BotRequest.DialogState.CurrentState as TopicStepState;
					resolvedParameters = currentState.ResolvedParameters;
				}
			};

			botRequest.InputText = "";
			this.RequestDispatcher.Dispatch(botRequest);
			botRequest.InputText = "d";
			this.RequestDispatcher.Dispatch(botRequest);
			botRequest.InputText = "Yesterday";
			this.RequestDispatcher.Dispatch(botRequest);
			var firstTopicParameter = botRequest.Bag as TopicParameter;
			botRequest.InputText = "Tomorrow";
			this.RequestDispatcher.Dispatch(botRequest);
			var secondTopicParameter = botRequest.Bag as TopicParameter;

			Assert.AreEqual(DateTime.Today.AddDays(-1), firstTopicParameter.Value);
			Assert.AreEqual(DateTime.Today.AddDays(1), secondTopicParameter.Value);
			Assert.AreEqual(0, botRequest.DialogState.GetSteps().Count());
			Assert.AreEqual(2, resolvedParameters.Count());
		}


		private class FirstMenu : MenuBot
		{
			public FirstMenu() : base(null)
			{
				this.Menus = new List<MenuItem>()
				{
					new MenuItem("1", "Des 1", (botRequest) => { this.OnRedirect(new MenuStepState(typeof(SecondMenu)), botRequest); }),
					new MenuItem("2", "Des 2", (botRequest) => { Console.WriteLine("=>2"); }),
					new MenuItem("d", "Test Date", (botRequest) => { this.OnRedirect(new DateParameterState() { Id ="PARAMETER1" } , botRequest); }),
				};
			}
		}

		private class SecondMenu : MenuBot
		{
			public SecondMenu() : base(null)
			{
				this.Menus = new List<MenuItem>()
				{
					new MenuItem("a", "Des a", (botRequest) => { this.OnRedirect(new MenuStepState(typeof(FirstMenu)), botRequest); }),
					new MenuItem("b", "Des b", (botRequest) => { botRequest.OutText = "B"; }),
					new MenuItem("c", "Cancel", (botRequest) => { this.OnFinalize(botRequest); }),
				};
			}
		}
	}
}
