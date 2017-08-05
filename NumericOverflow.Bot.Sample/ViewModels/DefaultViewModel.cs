using DotVVM.Framework.ViewModel;

namespace NumericOverflow.Bot.Sample.ViewModels
{
    public class DefaultViewModel : DotvvmViewModelBase
    {
        
        public string Title { get; set; }


        public DefaultViewModel()
        {
            Title = "Hello from DotVVM!";
        }

    }
}
