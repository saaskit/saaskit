using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.Caching.Memory;
using Microsoft.Framework.DependencyInjection;
using SaasKit.Multitenancy.AspNet5;

namespace SaasKit.Multitenancy.Samples.Mvc.AspNet5
{
	public class Startup
	{
		// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			// Add MVC services to the services container.
			services.AddMvc();

			// Uncomment the following line to add Web API services which makes it easier to port Web API 2 controllers.
			// You will also need to add the Microsoft.AspNet.Mvc.WebApiCompatShim package to the 'dependencies' section of project.json.
			// services.AddWebApiConventions();
		}

		// Configure is called after ConfigureServices is called.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			// Add the following to the request pipeline only in development environment.
			if (env.IsDevelopment())
			{
				app.UseBrowserLink();
				app.UseErrorPage(new ErrorPageOptions { SourceCodeLineCount = 20 });
				app.UseRuntimeInfoPage(); // default path is /runtimeinfo
			}
			else
			{
				// Add Error handling middleware which catches all application specific errors and
				// sends the request to the following path or controller action.
				app.UseErrorHandler("/Home/Error");
			}

			// Add static files to the request pipeline. (eg css, js files)
			app.UseStaticFiles();


			var cachedResolver = new CachedTenantResolver<AppTenant>(
				new AppTenantResolver(),
				t => t.Hostnames,
				RequestIdentification.FromHostname()
			)
			// extra options
			// .SetMemoryCache(new MemoryCache(new MemoryCacheOptions()))
			// .SetMemoryCacheEntryOptions(new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1))) // eg. sliding cache for 1 hour
			;
			

			//app.UseMultitenancy(new AppTenantResolver())
			app.UseMultitenancy(cachedResolver)
				.RedirectIfTenantNotFound<AppTenant>("https://github.com/saaskit/saaskit/wiki/Handling-unresolved-Tenants/", permanentRedirect: false);
			
			
			// Add MVC and routes to the request pipeline.
			app.UseMvc(routes => App_Start.RouteConfig.Setup(routes));
		}
	}


	public class AppTenantResolver : UriTenantResolver<AppTenant>
	{
		private readonly Dictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			{ "5000", "Tenant 1"},
			{ "5001", "Tenant 2"}
		};

		public override Task<TenantContext<AppTenant>> ResolveAsync(Uri uri)
		{
			string tenantName = null;
			TenantContext<AppTenant> tenantContext = null;

			if (_mappings.TryGetValue(uri.Port.ToString(), out tenantName))
			{
				tenantContext = new TenantContext<AppTenant>(new AppTenant { Name = tenantName, Hostnames = new[] { uri.Host } });
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
