using Microsoft.AspNetCore.Http;
using StructureMap;
using System;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy.StructureMap.Internal
{
    internal class MultitenantContainerMiddleware<TTenant>
    {
        private readonly RequestDelegate next;

        public MultitenantContainerMiddleware(RequestDelegate next)
        {
            Ensure.Argument.NotNull(next, nameof(next));
            this.next = next;
        }

        public async Task Invoke(HttpContext context, Lazy<ITenantContainerBuilder<TTenant>> builder)
        {
            Ensure.Argument.NotNull(context, nameof(context));

            var tenantContext = context.GetTenantContext<TTenant>();

            if (tenantContext != null)
            {
                var tenantContainer = await GetTenantContainerAsync(tenantContext, builder);

                using (var requestContainer = tenantContainer.GetNestedContainer())
                {
                    // Replace the request IServiceProvider created by IServiceScopeFactory
                    context.RequestServices = requestContainer.GetInstance<IServiceProvider>();
                    await next.Invoke(context);
                }
            }
        }

        private async Task<IContainer> GetTenantContainerAsync(
            TenantContext<TTenant> tenantContext, 
            Lazy<ITenantContainerBuilder<TTenant>> builder)
        {
            var tenantContainer = tenantContext.GetTenantContainer();

            if (tenantContainer == null)
            {
                tenantContainer = await builder.Value.BuildAsync(tenantContext.Tenant);
                tenantContext.SetTenantContainer(tenantContainer);
            }

            return tenantContainer;
        }
    }
}
