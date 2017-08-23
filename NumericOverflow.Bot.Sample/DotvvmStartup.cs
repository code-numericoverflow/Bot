using DotVVM.Framework;
using DotVVM.Framework.Configuration;
using DotVVM.Framework.ResourceManagement;
using DotVVM.Framework.Routing;

namespace NumericOverflow.Bot.Sample
{
    public class DotvvmStartup : IDotvvmStartup
    {
        // For more information about this class, visit https://dotvvm.com/docs/tutorials/basics-project-structure
        public void Configure(DotvvmConfiguration config, string applicationPath)
        {
            ConfigureRoutes(config, applicationPath);
            ConfigureControls(config, applicationPath);
            ConfigureResources(config, applicationPath);
        }

        private void ConfigureRoutes(DotvvmConfiguration config, string applicationPath)
        {
			config.RouteTable.Add("Default", "", "Views/default.dothtml");
			config.RouteTable.Add("Chat", "chat", "Views/Chat.dothtml");

			// Uncomment the following line to auto-register all dothtml files in the Views folder
			// config.RouteTable.AutoDiscoverRoutes(new DefaultRouteStrategy(config));    
		}

		private void ConfigureControls(DotvvmConfiguration config, string applicationPath)
        {
            // register code-only controls and markup controls
        }

        private void ConfigureResources(DotvvmConfiguration config, string applicationPath)
        {
			config.Resources.Register("chat", new StylesheetResource()
			{
				Location = new UrlResourceLocation("~/css/chat.css")
			});
		}
	}
}
