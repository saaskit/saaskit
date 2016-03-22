using SaasKit.Multitenancy;
using SaasKit.Multitenancy.StructureMap.Internal;
using StructureMap;
using System;

namespace Microsoft.AspNet.Builder
{
    public static class StructureMapApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTenantContainers<TTenant>(
            this IApplicationBuilder app, 
            Action<TTenant, ConfigurationExpression> tenantContainerBuilder)
        {
            Ensure.Argument.NotNull(app, nameof(app));
            Ensure.Argument.NotNull(tenantContainerBuilder, nameof(tenantContainerBuilder));

            return app.UseMiddleware<MultitenantContainerMiddleware<TTenant>>(tenantContainerBuilder);
        }

        public static IApplicationBuilder UseTenantContainers<TTenant>(
            this IApplicationBuilder app)
        {
            Ensure.Argument.NotNull(app, nameof(app));

            return app.UseMiddleware<MultitenantContainerMiddleware<TTenant>>();
        }
    }
}
