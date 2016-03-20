using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using StructureMap;
using System;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy.StructureMap
{
    public class MultitenantContainerMiddleware<TTenant>
    {
        RequestDelegate next;
        IContainer appContainer;
        Action<ConfigurationExpression> tenantConfiguration;

        public MultitenantContainerMiddleware(RequestDelegate next, IContainer appContainer, Action<ConfigurationExpression> tenantConfiguration)
        {
            this.next = next;
            this.appContainer = appContainer;
            this.tenantConfiguration = tenantConfiguration;
        }

        public async Task Invoke(HttpContext context)
        {
            var tenantContext = context.GetTenantContext<TTenant>();

            if (tenantContext != null)
            {
                var tenantContainer = GetTenantContainer(tenantContext);
                
                using (var requestContainer = tenantContainer.GetNestedContainer())
                {
                    // Replace the request services created by StructureMapServiceScopeFactory
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
                tenantContainer.Configure(tenantConfiguration);
                tenantContext.SetTenantContainer(tenantContainer);
            }

            return tenantContainer;
        }
    }
}
