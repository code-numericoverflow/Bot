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
	public class DateParameterBotTests
	{
		private IBot SUT { get; set; }
		private bool TrueNextPipe = true;

		public DateParameterBotTests()
		{
			this.SUT = new DateParameterBot();
		}

		[TestMethod]
		public void ShouldParseCorrectWhenValidDate()
		{
			var botRequest = this.GetDefaultRequest();
			botRequest.InputText = DateTime.Today.ToString();

			this.SUT.PipeIn(botRequest, ref this.TrueNextPipe);

			Assert.AreEqual(DateTime.Today, this.GetCurrentStateValue(botRequest));
		}

		[TestMethod]
		public void ShouldNotParseWhenNoValidDate()
		{
			var botRequest = this.GetDefaultRequest();
			botRequest.InputText = "abd";

			this.SUT.PipeIn(botRequest, ref this.TrueNextPipe);

			Assert.AreEqual(1,botRequest.DialogState.CurrentState.ErrorCount);
		}

		[TestMethod]
		public void ShouldParseCorrectWhenToday()
		{
			var botRequest = this.GetDefaultRequest();
			botRequest.InputText = "Today";

			this.SUT.PipeIn(botRequest, ref this.TrueNextPipe);

			Assert.AreEqual(DateTime.Today, this.GetCurrentStateValue(botRequest));
		}

		[TestMethod]
		public void ShouldParseCorrectWhenTomorrow()
		{
			var botRequest = this.GetDefaultRequest();
			botRequest.InputText = "TomorroW";

			this.SUT.PipeIn(botRequest, ref this.TrueNextPipe);

			Assert.AreEqual(DateTime.Today.AddDays(1), this.GetCurrentStateValue(botRequest));
		}

		[TestMethod]
		public void ShouldParseCorrectWhenYesterday()
		{
			var botRequest = this.GetDefaultRequest();
			botRequest.InputText = "Yesterday";

			this.SUT.PipeIn(botRequest, ref this.TrueNextPipe);

			Assert.AreEqual(DateTime.Today.AddDays(-1), this.GetCurrentStateValue(botRequest));
		}

		[TestMethod]
		public void ShoulFillBagWithSelectedValueWhenYesterday()
		{
			var botRequest = this.GetDefaultRequest();
			botRequest.InputText = "Yesterday";

			this.SUT.PipeIn(botRequest, ref this.TrueNextPipe);

			var topicParameter = botRequest.Bag as TopicParameter;
			Assert.AreEqual(DateTime.Today.AddDays(-1), topicParameter.Value);
		}

		private BotRequest GetDefaultRequest()
		{
			var dialogState = new DialogState();
			var currentStep = new DateParameterState() { Id = "PARAMETER1" };
			dialogState.AddStep(currentStep);
			dialogState.CurrentState = currentStep;
			return new BotRequest(dialogState);
		}

		private DateTime GetCurrentStateValue(BotRequest botRequest)
		{
			var dateParameterState = botRequest.DialogState.CurrentState as DateParameterState;
			return dateParameterState.Value;
		}
	}
}
