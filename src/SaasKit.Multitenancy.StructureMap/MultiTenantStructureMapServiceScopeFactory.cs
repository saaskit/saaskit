using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using System;

namespace SaasKit.Multitenancy.StructureMap
{
    internal class MultitenantStructureMapServiceScopeFactory<TTenant> : IServiceScopeFactory
    {
        private Action<ConfigurationExpression> tenantContainerConfiguration;

        public MultitenantStructureMapServiceScopeFactory(
            IContainer container, 
            Action<ConfigurationExpression> tenantContainerConfiguration)
        {
            Container = container;
            this.tenantContainerConfiguration = tenantContainerConfiguration;
        }

        private IContainer Container { get; }

        public IServiceScope CreateScope()
        {
            var tenantContainer = GetTenantContainer();
            return new StructureMapServiceScope(tenantContainer.GetNestedContainer());
        }

        private IContainer GetTenantContainer()
        {
            var context = Container.GetInstance<IHttpContextAccessor>()?.HttpContext;

            var tenantContext = context?.GetTenantContext<TTenant>();

            if (tenantContext == null)
            {
                // Just return the root container
                return Container;
            }

            var tenantContainer = tenantContext.GetTenantContainer();
            if (tenantContainer == null)
            {
                tenantContainer = Container.CreateChildContainer();
                tenantContainer.Configure(tenantContainerConfiguration);
                tenantContext.SetTenantContainer(tenantContainer);
            }

            return tenantContainer;
        }

        private class StructureMapServiceScope : IServiceScope
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
