using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy
{
    public class TenantResolutionMiddleware<TTenant>
    {
        private readonly Func<IDictionary<string, object>, Task> next;
        private readonly Func<ITenantResolver<TTenant>> tenantResolverFactory;

        public TenantResolutionMiddleware(
            Func<IDictionary<string, object>, Task> next,
            Func<ITenantResolver<TTenant>> tenantResolverFactory)
        {
            Ensure.Argument.NotNull(next, "next");
            Ensure.Argument.NotNull(tenantResolverFactory, "tenantResolverFactory");
            
            this.next = next;
            this.tenantResolverFactory = tenantResolverFactory;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            Ensure.Argument.NotNull(environment, "environment");
            
            var tenantResolver = tenantResolverFactory();

            var tenantContext = await tenantResolver.ResolveAsync(environment);

            if (tenantContext != null)
            {
                environment.SetTenantContext(tenantContext);
            }

            await next(environment);
        }
    }
}
