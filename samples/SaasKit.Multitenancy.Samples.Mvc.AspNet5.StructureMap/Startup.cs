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
using SaasKit.Multitenancy.AspNet5.StructureMap;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace SaasKit.Multitenancy.Samples.Mvc.AspNet5.StructureMap
{
	public class Startup
	{
		// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			// Add MVC services to the services container.
			services.AddMvc();

			// Uncomment the following line to add Web API services which makes it easier to port Web API 2 controllers.
			// You will also need to add the Microsoft.AspNet.Mvc.WebApiCompatShim package to the 'dependencies' section of project.json.
			// services.AddWebApiConventions();



			// do after adding other services
			var container = new Container();

			// Here we populate the container using the service collection.
			// This will register all services from the collection
			// into the container with the appropriate lifetime.
			container.Populate(services);

			// IMPORTANT: configuration needs to be done after calling the "container.Populate(services)" call above, so we can override default AspNet classes
			// configure StructureMap as per usual with registries / scans / manual configuration
			container.Configure(c =>
			{
				c.Scan(scan =>
				{
					scan.TheCallingAssembly();
					scan.WithDefaultConventions();
				});

				// instead of standard StructureMap.Dnx StructureMapServiceScopeFactory, need to use a multitenant version instead
				c.For<IServiceScopeFactory>().LifecycleIs(Lifecycles.Container).Use<AppMultiTenantStructureMapServiceScopeFactory>();
			});

			// Make sure we return an IServiceProvider, 
			// this makes DNX use the StructureMap container.
			return container.GetInstance<IServiceProvider>();
		}

		// Configure is called after ConfigureServices is called.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.MinimumLevel = LogLevel.Information;
			loggerFactory.AddConsole();
			loggerFactory.AddDebug();

			// Add the following to the request pipeline only in development environment.
			if (env.IsDevelopment())
			{
				#if DNX451
				app.UseBrowserLink();
				#endif
				app.UseDeveloperExceptionPage(new ErrorPageOptions { SourceCodeLineCount = 20 });
				app.UseRuntimeInfoPage(); // default path is /runtimeinfo
			}
			else
			{
				// Add Error handling middleware which catches all application specific errors and
				// sends the request to the following path or controller action.
				app.UseExceptionHandler("/Home/Error");
			}

			// Add static files to the request pipeline. (eg css, js files)
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


			// Add MVC and routes to the request pipeline.
			app.UseMvc(routes => AppStart.RouteConfig.Setup(routes));
		}

		// Entry point for the application.
		public static void Main(string[] args) => WebApplication.Run<Startup>(args);
	}


	public class AppTenantResolver : HttpRequestTenantResolver<AppTenant>
	{
		private readonly Dictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			// localhost port numbers
			{ "localhost:6001", "Tenant 1"},
			{ "localhost:6002", "Tenant 2"},
			{ "localhost:6003", "Tenant 3"}
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

	public class AppMultiTenantStructureMapServiceScopeFactory : MultiTenantStructureMapServiceScopeFactory<AppTenant>
	{
		public AppMultiTenantStructureMapServiceScopeFactory(IContainer container) : base(container) { }

		protected override string GetTenantProfileName(TenantContext<AppTenant> tenantContext)
		{
			return tenantContext.Tenant.Name;
		}

		protected override void ConfigureTenantProfileContainer(IContainer tenantProfile, TenantContext<AppTenant> tenantContext)
		{
			// NOTE: 
			// - Could configure profile as part of Container configuration, then just retrieve it using the tenant/profile name, but we are going to configure it here
			// - as we are doing tenant configuring here, could equally use "Container.CreateChildContainer();"
			// more info on Profiles and Child Containers -> http://structuremap.github.io/the-container/profiles-and-child-containers/


			// for Tenant 2, want to override IMessageService to use OtherMessageService
			if (tenantProfile.ProfileName.Equals("Tenant 2", StringComparison.InvariantCultureIgnoreCase))
			{
				tenantProfile.Configure(c =>
				{
					c.For<IMessageService>().Use<OtherMessageService>();
				});
			}

			// Tenant 2 -> IMessageService should use OtherMessageService
			// all other tenants -> IMessageService should use the default MessageService
			//var whatDoIHave = tenantProfile.WhatDoIHave();
		}
	}
}
