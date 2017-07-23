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
	public class RequestDispatcherTests
	{
		private IRequestDispatcher SUT { get; set; }
		private IResolver FakeResolver { get; set; }
		private MenuBot FakeMenuBot { get; set; }
		private IBot FakeIBot { get; set; }
		private bool TruePipe = true;
		private bool FalsePipe = false;

		public RequestDispatcherTests()
		{
			this.FakeResolver = A.Fake<IResolver>();
			this.SUT = new RequestDispatcher(this.FakeResolver);
			this.FakeMenuBot = A.Fake<MenuBot>();
			this.FakeIBot = A.Fake<IBot>();
		}

		[TestMethod]
		public void ShouldCallBotPipeForEachStateWhenDispatch()
		{
			int pipeCount = 0;
			var botRequest = GetDefaultBotRequest(new Type[] { typeof(MenuBot), typeof(MenuBot) });
			A.CallTo(() => this.FakeResolver.Resolve(typeof(MenuBot))).Returns(this.FakeMenuBot);
			A.CallTo(() => this.FakeMenuBot.PipeIn(botRequest, ref this.TruePipe)).Invokes(() => pipeCount++);

			this.SUT.Dispatch(botRequest);

			Assert.AreEqual(2, pipeCount);
		}

		[TestMethod]
		public void ShouldNotCallWhenExitNextPipe()
		{
			int pipeCount = 0;
			var botRequest = GetDefaultBotRequest(new Type[] { typeof(MenuBot), typeof(IBot) });
			A.CallTo(() => this.FakeResolver.Resolve(typeof(MenuBot))).Returns(this.FakeMenuBot);
			A.CallTo(() => this.FakeMenuBot.PipeIn(botRequest, ref this.FalsePipe)).Invokes(() => pipeCount++ );
			A.CallTo(() => this.FakeResolver.Resolve(typeof(IBot))).Returns(this.FakeIBot);
			A.CallTo(() => this.FakeIBot.PipeIn(botRequest, ref this.TruePipe)).Invokes(() => pipeCount++ );

			this.SUT.Dispatch(botRequest);

			Assert.AreEqual(1, pipeCount);
		}

		private BotRequest GetDefaultBotRequest(Type[] types)
		{
			var steps = types.Select(type => new DialogStepState(type) );
			var request = new BotRequest(new DialogState());
			request.DialogState.AddSteps(steps);
			return request;
		}

	}
}
