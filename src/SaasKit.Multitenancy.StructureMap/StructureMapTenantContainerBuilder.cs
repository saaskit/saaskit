using StructureMap;
using System;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy.StructureMap
{
    public class StructureMapTenantContainerBuilder<TTenant> : ITenantContainerBuilder<TTenant>
    {
        public StructureMapTenantContainerBuilder(IContainer container, Action<TTenant, ConfigurationExpression> configure)
        {
            Ensure.Argument.NotNull(container, nameof(container));
            Ensure.Argument.NotNull(configure, nameof(configure));

            Container = container;
            Configure = configure;
        }

        protected IContainer Container { get; }
        protected Action<TTenant, ConfigurationExpression> Configure { get; }

        public virtual Task<IContainer> BuildAsync(TTenant tenant)
        {
            Ensure.Argument.NotNull(tenant, nameof(tenant));

            var tenantContainer = Container.CreateChildContainer();
            tenantContainer.Configure(config => Configure(tenant, config));

            return Task.FromResult(tenantContainer);
        }
    }
}
