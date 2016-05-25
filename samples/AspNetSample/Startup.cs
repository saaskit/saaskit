using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using SaasKit.Multitenancy;

namespace AspNetSample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMultitenancy<AppTenant, CachingAppTenantResolver>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);
			
            app.Map(
                new PathString("/onboarding"),
                branch => branch.Run(async ctx =>
                {
                    await ctx.Response.WriteAsync("Onboarding");
                })
            );

            app.UseMultitenancy<AppTenant>();

            app.Use(async (ctx, next) =>
            {
                if (ctx.GetTenant<AppTenant>().Name == "Default")
                {
                    ctx.Response.Redirect("/onboarding");
                } else
                {
                    await next();
                }
            });

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

        public async Task Invoke(HttpContext context)
        {
            var tenantContext = context.GetTenantContext<AppTenant>();

            if (tenantContext != null)
            {
                var timestamp = ((DateTime)tenantContext.Properties["Created"]);

                await context.Response.WriteAsync(
                    $"Tenant \"{tenantContext.Tenant.Name}\" created at {timestamp.Ticks}");
            }
            else
            {
                await context.Response.WriteAsync("No matching tenant found.");
            }
        }
    }
}
