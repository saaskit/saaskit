using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StructureMap;
using System;
using System.Threading.Tasks;

namespace AspNetStructureMapSample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMultitenancy<AppTenant, AppTenantResolver>();

            var container = new Container();

            container.Populate(services);

            container.Configure(c =>
            {
                // Application Services
                // c.For<ITenantContainerBuilder<AppTenant>>().Use(() => new AppTenantContainerBuilder(container));
            });

            container.ConfigureTenants<AppTenant>(c =>
            {
                // Tenant Scoped Services
                c.For<IMessageService>().Singleton().Use<MessageService>();
            });

            return container.GetInstance<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);
			

            app.UseMultitenancy<AppTenant>();
            app.UseTenantContainers<AppTenant>();

            app.UseMiddleware<LogTenantMiddleware>();
        }
		
    }

    public class LogTenantMiddleware
    {
        RequestDelegate next;
        private readonly ILogger log;

        public LogTenantMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;
            this.log = loggerFactory.CreateLogger<LogTenantMiddleware>();
        }

        public async Task Invoke(HttpContext context, IMessageService messageService)
        {
            var tenant = context.GetTenant<AppTenant>();

            if (tenant != null)
            {
                await context.Response.WriteAsync(
                    messageService.Format($"Tenant \"{tenant.Name}\"."));
            }
            else
            {
                await context.Response.WriteAsync("No matching tenant found.");
            }
        }
    }
}
