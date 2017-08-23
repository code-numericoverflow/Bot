using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;

using NumericOverflow.Bot.Models;
using NumericOverflow.Bot.Services;
using NumericOverflow.Bot.Sample.Data;

namespace NumericOverflow.Bot.Sample.ViewModels
{
	public class Chat : DotvvmViewModelBase
	{
		public virtual string Id { get; set; }
		public virtual int LastItemId { get; set; }
		public virtual List<ChatItem> ChatItems { get; set; }
		public virtual String InputText { get; set; }
		//[Bind(Direction.ServerToClient)]
		public virtual List<ChoiceItem> ChoiceItems { get; set; }

		private IRequestDispatcher RequestDispatcher { get; set; }
		private IDialogStateRepository DialogStateRepository { get; set; }

		public Chat(IRequestDispatcher requestDispatcher, IDialogStateRepository dialogStateRepository)
		{
			this.RequestDispatcher = requestDispatcher;
			this.DialogStateRepository = dialogStateRepository;
		}

		public override Task Init()
		{
			if (!Context.IsPostBack)
			{
				this.ChatItems = new List<ChatItem>();
				var dialogState = new DialogState();
				var topicStepState = new TopicStepState()
				{
					CurrentStatus = TopicStepState.Status.Initialized,
				};
				dialogState.AddStep(topicStepState);
				this.Id = Guid.NewGuid().ToString();
				this.DialogStateRepository.Set(this.Id, dialogState);
				this.Submit();
			}
			return base.Init();
		}

		public virtual void Submit()
		{
			var DialogState = this.DialogStateRepository.Get(this.Id);
			this.AddChatItem(this.InputText, false);
			var botRequest = new BotRequest(DialogState)
			{
				InputText = this.InputText,
				OutText = "",
				Choices = null,
				SelectedChoice = null,
				Bag = null,
			};
			this.ProcessRequest(botRequest);
		}

		protected void ProcessRequest(BotRequest botRequest)
		{
			this.RequestDispatcher.Dispatch(botRequest);
			this.AddChatItem(botRequest.OutText, true);
			this.InputText = "";
			this.LimitChatItems(10, 5);
			this.ChoiceItems = botRequest.Choices;
		}

		public virtual void ChoiceSelected(ChoiceItem choiceItem)
		{
			var DialogState = this.DialogStateRepository.Get(this.Id);
			this.AddChatItem(choiceItem.Description, false);
			var botRequest = new BotRequest(DialogState)
			{
				InputText = choiceItem.Description,
				OutText = "",
				Choices = null,
				SelectedChoice = choiceItem,
				Bag = null,
			};
			this.ProcessRequest(botRequest);
		}

		protected void AddChatItem(string text, bool isBot)
		{
			lock (this.ChatItems)
			{
				this.LastItemId++;
				this.ChatItems.Add(new ChatItem()
				{
					Id = this.LastItemId,
					IsBot = isBot,
					Text = text,
				});
			}
		}

		protected void LimitChatItems(int maxCount, int resetCount)
		{
			if (this.ChatItems.Count > maxCount)
			{
				Enumerable.Range(0, resetCount - 1).ToList().ForEach(x => this.ChatItems.RemoveAt(0));
			}
		}
	}
}
