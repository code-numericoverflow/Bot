using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using NumericOverflow.Bot.Models;

namespace NumericOverflow.Bot.Services
{
	public class MenuBot : BotBase
	{
		public List<MenuItem> Menus { get; set; }

		public MenuBot(List<MenuItem> menus)
		{
			this.Menus = menus;
		}

		public override void PipeIn(BotRequest botRequest, ref bool nextPipe)
		{
			if (botRequest.DialogState.NextState == null)
			{
				var foundMenu = this.GetSelectedMenuItem(botRequest);
				if (foundMenu != null)
				{
					botRequest.Choices = null;
					botRequest.SelectedChoice = null;
					foundMenu.Action.Invoke(botRequest);
				}
				else
				{
					botRequest.Choices = new List<ChoiceItem>(this.Menus.Select(m => m.ChoiceItem));
					botRequest.SelectedChoice = null;
					if (botRequest.InputText != "")
					{
						this.AddError(botRequest);
					}
					botRequest.OutText = "info";
				}
				nextPipe = false;
			}
		}

		public override void PipeOut(BotRequest botRequest, ref bool nextPipe)
		{
		}

		private MenuItem GetSelectedMenuItem(BotRequest botRequest)
		{
			var foundMenu = this.Menus.Where(m => m.ChoiceItem.Id == botRequest.InputText).FirstOrDefault();
			if (foundMenu == null && botRequest.SelectedChoice != null)
			{
				foundMenu = this.Menus.Where(m => m.ChoiceItem.Id == botRequest.SelectedChoice.Id).FirstOrDefault();
			}
			return foundMenu;
		}
	}
}
