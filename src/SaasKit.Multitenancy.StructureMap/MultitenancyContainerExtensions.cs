using SaasKit.Multitenancy;
using SaasKit.Multitenancy.StructureMap;
using System;

namespace StructureMap
{
    public static class MultitenancyContainerExtensions
    {
        public static void ConfigureTenants<TTenant>(this IContainer container, Action<ConfigurationExpression> configure)
        {
            Ensure.Argument.NotNull(container, nameof(container));
            Ensure.Argument.NotNull(configure, nameof(configure));

            container.Configure(_ =>
                _.For<ITenantContainerBuilder<TTenant>>()
                    .Use(new StructureMapTenantContainerBuilder<TTenant>(container, (tenant, config) => configure(config)))
            );
        }

        public static void ConfigureTenants<TTenant>(this IContainer container, Action<TTenant, ConfigurationExpression> configure)
        {
            Ensure.Argument.NotNull(container, nameof(container));
            Ensure.Argument.NotNull(configure, nameof(configure));

            container.Configure(_ =>
                _.For<ITenantContainerBuilder<TTenant>>()
                    .Use(new StructureMapTenantContainerBuilder<TTenant>(container, configure))
            );
        }
    }
}
