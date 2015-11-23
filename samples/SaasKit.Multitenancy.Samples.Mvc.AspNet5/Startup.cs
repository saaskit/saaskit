using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaasKit.Multitenancy.AspNet5;

namespace SaasKit.Multitenancy.Samples.Mvc.AspNet5
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			// Add MVC services to the services container.
			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole();
			loggerFactory.AddDebug();

			//if (env.IsDevelopment())
			//{
				app.UseDeveloperExceptionPage(new ErrorPageOptions {SourceCodeLineCount = 20 });
			//}

			app.UseIISPlatformHandler();

			app.UseStaticFiles();


			var cachedResolver = new CachedTenantResolver<AppTenant>(
				new AppTenantResolver(),
				t => t.Hostnames,
				RequestIdentification.FromHostname()
			)
			// extra options around configuring CachedTenantResolver
			.SetMemoryCache(new MemoryCache(new MemoryCacheOptions()))
			.SetMemoryCacheEntryOptions(new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1))) // eg. sliding cache for 1 hour
			;


			//app.UseMultitenancy(new AppTenantResolver())
			app.UseMultitenancy(cachedResolver)
				.RedirectIfTenantNotFound<AppTenant>("https://github.com/saaskit/saaskit/wiki/Handling-unresolved-Tenants/", permanentRedirect: false);

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}

		// Entry point for the application.
		public static void Main(string[] args) => WebApplication.Run<Startup>(args);
	}





	public class AppTenantResolver : HttpRequestTenantResolver<AppTenant>
	{
		private readonly Dictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			{ "localhost:5000", "Tenant 1"},
			{ "localhost:5001", "Tenant 2"}
		};

		public override Task<TenantContext<AppTenant>> ResolveAsync(HttpRequest request)
		{
			string tenantName = null;
			TenantContext<AppTenant> tenantContext = null;

			if (_mappings.TryGetValue(request.Host.Value, out tenantName))
			{
				tenantContext = new TenantContext<AppTenant>(new AppTenant { Name = tenantName, Hostnames = new[] { request.Host.Value } });
				tenantContext.Properties.Add("Created", DateTime.UtcNow);
			}

			return Task.FromResult(tenantContext);
		}
	}
	public class AppTenant
	{
		public string Name { get; set; }
		public IEnumerable<string> Hostnames { get; set; }
	}
}
