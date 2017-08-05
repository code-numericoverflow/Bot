
namespace NumericOverflow.Bot.Data
{
	public class VoidDialogTextRepository : IDialogTextRepository
	{
		public string GetDialogTextFor(string dialogId)
		{
			return dialogId;
		}
	}
}
