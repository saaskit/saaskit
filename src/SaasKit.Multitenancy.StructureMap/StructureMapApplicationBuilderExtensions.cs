using SaasKit.Multitenancy.StructureMap;
using StructureMap;
using System;

namespace Microsoft.AspNet.Builder
{
    public static class StructureMapApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTenantContainer<TTenant>(this IApplicationBuilder app, Action<ConfigurationExpression> tenantConfiguration)
        {
            return app.UseMiddleware<MultitenantContainerMiddleware<TTenant>>(tenantConfiguration);
        }
    }
}
