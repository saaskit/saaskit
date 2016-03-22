using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using StructureMap;
using System;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy.StructureMap.Internal
{
    internal class MultitenantContainerMiddleware<TTenant>
    {
        private readonly RequestDelegate next;
        private readonly IContainer appContainer;
        private readonly Action<TTenant, ConfigurationExpression> tenantContainerBuilder;

        public MultitenantContainerMiddleware(
            RequestDelegate next, 
            IContainer appContainer, 
            Action<TTenant, ConfigurationExpression> tenantContainerBuilder)
        {
            Ensure.Argument.NotNull(next, nameof(next));
            Ensure.Argument.NotNull(appContainer, nameof(appContainer));
            Ensure.Argument.NotNull(tenantContainerBuilder, nameof(tenantContainerBuilder));

            this.next = next;
            this.appContainer = appContainer;
            this.tenantContainerBuilder = tenantContainerBuilder;
        }

        public async Task Invoke(HttpContext context)
        {
            Ensure.Argument.NotNull(context, nameof(context));

            var tenantContext = context.GetTenantContext<TTenant>();

            if (tenantContext != null)
            {
                var tenantContainer = GetTenantContainer(tenantContext);
                
                using (var requestContainer = tenantContainer.GetNestedContainer())
                {
                    // Replace the request IServiceProvider created by IServiceScopeFactory
                    context.RequestServices = requestContainer.GetInstance<IServiceProvider>();
                    await next.Invoke(context);
                }
            }
        }

        private IContainer GetTenantContainer(TenantContext<TTenant> tenantContext)
        {
            var tenantContainer = tenantContext.GetTenantContainer();

            if (tenantContainer == null)
            {
                tenantContainer = appContainer.CreateChildContainer();
                tenantContainer.Configure(config => tenantContainerBuilder(tenantContext.Tenant, config));
                tenantContext.SetTenantContainer(tenantContainer);
            }

            return tenantContainer;
        }
    }
}
