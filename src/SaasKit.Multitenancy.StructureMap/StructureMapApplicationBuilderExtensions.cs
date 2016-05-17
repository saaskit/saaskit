using SaasKit.Multitenancy;
using SaasKit.Multitenancy.StructureMap.Internal;

namespace Microsoft.AspNetCore.Builder
{
    public static class StructureMapApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTenantContainers<TTenant>(
            this IApplicationBuilder app)
        {
            Ensure.Argument.NotNull(app, nameof(app));
            return app.UseMiddleware<MultitenantContainerMiddleware<TTenant>>();
        }
    }
}
