
using NumericOverflow.Bot.Models;

namespace NumericOverflow.Bot.Sample.Data
{
    public interface IDialogStateRepository
    {
		void Set(string id, DialogState dialogState);
		DialogState Get(string id);
	}
}
