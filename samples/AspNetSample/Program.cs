using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetSample
{
    public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddLogging(cfg => cfg.AddConsole());
			builder.Services.AddMultitenancy<AppTenant, CachingAppTenantResolver>();

			var app = builder.Build();

			app.Map("/onboarding", branch => branch.Run(async ctx => await ctx.Response.WriteAsync("Onboarding")));
			app.UseMultitenancy<AppTenant>();
			app.Use(async (ctx, next) =>
			{
				if (ctx.GetTenant<AppTenant>().Name == "Default")
				{
					ctx.Response.Redirect("/onboarding");
				}
				else
				{
					await next();
				}
			});

			app.UseMiddleware<LogTenantMiddleware>();

            app.Run();
        }
	}
}