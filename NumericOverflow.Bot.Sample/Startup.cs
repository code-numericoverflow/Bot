using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

using FakeItEasy;

using NumericOverflow.Bot.Data;
using NumericOverflow.Bot.Services;
using NumericOverflow.Bot.Indexers.Services;
using NumericOverflow.Bot.Sample.Data;
using NumericOverflow.Bot.Sample.Services;

namespace NumericOverflow.Bot.Sample
{
    public class Startup
    {

		public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();
            services.AddAuthorization();
            services.AddWebEncoders();

			services.AddMemoryCache();
			services.AddDotVVM(options =>
            {
                options.AddDefaultTempStorages("Temp");

				this.AddCustomServices(options.Services);
			});
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            var dotvvmConfiguration = app.UseDotVVM<DotvvmStartup>(env.ContentRootPath);
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(env.WebRootPath)
            });
		}

		private void AddCustomServices(IServiceCollection services)
		{
			services.AddTransient<IDialogStateRepository, MemoryCacheRepository>();
			services.AddTransient<IDialogTextRepository, VoidDialogTextRepository>();
			services.AddTransient<IRequestDispatcher, RequestDispatcher>();
			services.AddTransient<IResolver, Resolver>();
			services.AddSingleton<ITopicIndexer, TextSearchTopicIndexer>();
			this.AddBotServices(services);
			this.CreateFakeServices(services);
		}

		private void AddBotServices(IServiceCollection services)
		{
			services.AddTransient<DateParameterBot, DateParameterBot>();
			services.AddTransient<TopicBot, TopicBot>();
		}

		private void CreateFakeServices(IServiceCollection services)
		{
			services.AddSingleton<ITopicParameterRepository>(A.Fake<ITopicParameterRepository>());
		}
	}
}
