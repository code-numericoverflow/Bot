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
	public class MenuBotTests
	{
		private IBot SUT { get; set; }
		private BotRequest BotRequest { get; set; }
		private bool NextPipe = false;
		private bool ActionExecuted = false;
		
		public MenuBotTests()
		{
			this.SUT = new MenuBot(this.GetFakeMenuItems());
			this.BotRequest = this.GetDefaultMenuBotRequest(0);
			this.NextPipe = true;
			this.ActionExecuted = false;
		}

		[TestMethod]
		public void ShouldRespondWithChoicesWhenNotSelectedChoiceAndNoNextBot()
		{
			this.BotRequest.DialogState.NextState = null;
			this.BotRequest.SelectedChoice = null;

			SUT.PipeIn(this.BotRequest, ref this.NextPipe);

			Assert.IsNotNull(this.BotRequest.Choices);
			Assert.AreEqual(false, this.NextPipe);
		}

		[TestMethod]
		public void ShouldAddErrorsWhenNotSelectedChoiceAndNoCorrectInput()
		{
			this.BotRequest.DialogState.NextState = null;
			this.BotRequest.SelectedChoice = null;
			this.BotRequest.InputText = "jh";

			SUT.PipeIn(this.BotRequest, ref this.NextPipe);

			Assert.IsNotNull(this.BotRequest.Choices);
			Assert.AreEqual(false, this.NextPipe);
			Assert.AreEqual(1, this.BotRequest.DialogState.CurrentState.ErrorCount);
		}

		[TestMethod]
		public void ShouldRespondWithNoChoicesWhenSelectedChoiceAndNoNextBot()
		{
			this.BotRequest.DialogState.NextState = null;
			this.BotRequest.SelectedChoice = new ChoiceItem("1", "");

			SUT.PipeIn(this.BotRequest, ref this.NextPipe);

			Assert.IsNull(this.BotRequest.Choices);
			Assert.AreEqual(false, this.NextPipe);
		}

		[TestMethod]
		public void ShouldExecuteActionWhenSelectedChoiceAndNoNextBot()
		{
			this.BotRequest.DialogState.NextState = null;
			this.BotRequest.SelectedChoice = new ChoiceItem("1", "");

			SUT.PipeIn(this.BotRequest, ref this.NextPipe);

			Assert.IsTrue(this.ActionExecuted);
		}

		[TestMethod]
		public void ShouldNotRespondWhenNextBot()
		{
			this.BotRequest.DialogState.NextState = new DialogStepState(this.GetType()) ;

			SUT.PipeIn(this.BotRequest, ref this.NextPipe);

			Assert.IsNull(this.BotRequest.Choices);
			Assert.AreEqual(true, this.NextPipe);
		}

		private BotRequest GetDefaultMenuBotRequest(int errorCount)
		{
			var step = new MenuStepState(typeof(MenuBot));
			var dialogState = new DialogState();
			dialogState.AddStep(step);
			dialogState.CurrentState = step;
			return new BotRequest(dialogState);
		}

		private List<MenuItem> GetFakeMenuItems()
		{
			return new List<MenuItem>()
			{
				new MenuItem("1", "des 1", (botRequest) => { this.ActionExecuted = true; }),
				new MenuItem("2", "des 2", (botRequest) => { }),
			};
		}

	}
}
