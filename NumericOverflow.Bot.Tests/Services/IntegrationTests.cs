using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeItEasy;

using NumericOverflow.Bot.Models;
using NumericOverflow.Bot.Services;

namespace NumericOverflow.Bot.Tests.Services
{
	[TestClass]
	public class IntegrationTests
	{

		private IResolver Resolver { get; set; }
		private IRequestDispatcher RequestDispatcher { get; set; }

		public IntegrationTests()
		{
			this.Resolver = A.Fake<IResolver>();
			A.CallTo(() => this.Resolver.Resolve(typeof(FirstMenu))).Returns(new FirstMenu());
			A.CallTo(() => this.Resolver.Resolve(typeof(SecondMenu))).Returns(new SecondMenu());
			A.CallTo(() => this.Resolver.Resolve(typeof(DateParameterBot))).Returns(new DateParameterBot());
			this.RequestDispatcher = new RequestDispatcher(Resolver);
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

			Assert.AreEqual("PARAMETER1=" + DateTime.Today.ToString() + ";", botRequest.Bag);
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
