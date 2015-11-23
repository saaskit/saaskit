using System;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;

namespace SaasKit.Multitenancy.AspNet5.StructureMap
{
	/// <summary>
	/// Get the Container profile for the current tenant
	/// Adapted from https://github.com/structuremap/structuremap.dnx/blob/master/src/StructureMap.Dnx/StructureMapServiceScopeFactory.cs
	/// </summary>
	/// <typeparam name="TTenant"></typeparam>
	public abstract class MultiTenantStructureMapServiceScopeFactory<TTenant> : IServiceScopeFactory
				where TTenant : class
	{
		protected MultiTenantStructureMapServiceScopeFactory(IContainer container)
		{
			Container = container;
		}

		protected IContainer Container { get; }

		public virtual IServiceScope CreateScope()
		{
			var tenantContext = GetTenantContext(Container.GetInstance<IHttpContextAccessor>().HttpContext);
			var tenantProfileName = GetTenantProfileName(tenantContext);

			var tenantProfileContainer = Container.GetProfile(tenantProfileName);
			ConfigureTenantProfileContainer(tenantProfileContainer, tenantContext);

			return new StructureMapServiceScope(tenantProfileContainer.GetNestedContainer());
		}

		/// <summary>
		/// Get the TenantContext from HttpContext
		/// </summary>
		/// <param name="httpContext"></param>
		/// <returns></returns>
		protected virtual TenantContext<TTenant> GetTenantContext(HttpContext httpContext)
		{
			return httpContext.GetTenantContext<TTenant>();
		}

		/// <summary>
		/// Work out the StructureMap profile name for the current tenant (suggest some kind of "Tenant.Name" value)
		/// </summary>
		/// <param name="tenantContext"></param>
		/// <returns></returns>
		protected abstract string GetTenantProfileName(TenantContext<TTenant> tenantContext);

		/// <summary>
		/// Optionally: add/edit configuration to the current tenant's Container Profile for this request only
		/// Can also setup tenant profile's as part of the Startup configuration and just return the profile as is (ie don't need to configure for every request)
		/// </summary>
		/// <param name="tenantProfile"></param>
		/// <param name="tenantContext"></param>
		protected virtual void ConfigureTenantProfileContainer(IContainer tenantProfile, TenantContext<TTenant> tenantContext) { }
		
		protected class StructureMapServiceScope : IServiceScope
		{
			public StructureMapServiceScope(IContainer container)
			{
				Container = container;
				ServiceProvider = container.GetInstance<IServiceProvider>();
			}

			private IContainer Container { get; }

			public IServiceProvider ServiceProvider { get; }

			public void Dispose() => Container.Dispose();
		}
	}
}
