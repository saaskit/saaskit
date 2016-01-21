using Microsoft.AspNet.Http;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetSample
{
    public class AppTenantResolver : ITenantResolver<AppTenant>
    {
        private readonly Dictionary<string, string> mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "localhost:6000", "Tenant 1"},
            { "localhost:6001", "Tenant 2"},
            { "localhost:6002", "Tenant 3"},
        };

        public Task<TenantContext<AppTenant>> ResolveAsync(HttpContext context)
        {
            string tenantName = null;
            TenantContext<AppTenant> tenantContext = null;

            if (mappings.TryGetValue(context.Request.Host.Value, out tenantName))
            {
                tenantContext = new TenantContext<AppTenant>(
                    new AppTenant { Name = tenantName, Hostnames = new[] { context.Request.Host.Value } });

                tenantContext.Properties.Add("Created", DateTime.UtcNow);
            }

            return Task.FromResult(tenantContext);
        }
    }
}
