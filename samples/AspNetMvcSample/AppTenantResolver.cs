using Microsoft.AspNet.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetMvcSample
{
    public class CachingAppTenantResolver : MemoryCacheTenantResolver<AppTenant>
    {
        private readonly IEnumerable<AppTenant> tenants;

        public CachingAppTenantResolver(IMemoryCache cache, ILoggerFactory loggerFactory, IOptions<MultitenancyOptions> options)
            : base(cache, loggerFactory)
        {
            this.tenants = options.Value.Tenants;
        }

        protected override string GetContextIdentifier(HttpContext context)
        {
            return context.Request.Host.Value.ToLower();
        }

        protected override IEnumerable<string> GetTenantIdentifiers(TenantContext<AppTenant> context)
        {
            return context.Tenant.Hostnames;
        }

        protected override Task<TenantContext<AppTenant>> ResolveAsync(HttpContext context)
        {
            TenantContext<AppTenant> tenantContext = null;

            var tenant = tenants.FirstOrDefault(t => 
                t.Hostnames.Any(h => h.Equals(context.Request.Host.Value.ToLower())));

            if (tenant != null)
            {
                tenantContext = new TenantContext<AppTenant>(tenant);
            }

            return Task.FromResult(tenantContext);
        }
    }
}
