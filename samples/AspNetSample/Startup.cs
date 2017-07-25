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
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMultitenancy<AppTenant, CachingAppTenantResolver>();
        }

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
