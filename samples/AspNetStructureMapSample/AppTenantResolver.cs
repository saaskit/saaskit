using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetStructureMapSample
{
    public class AppTenantResolver : MemoryCacheTenantResolver<AppTenant>
    {
        private readonly Dictionary<string, string> mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "localhost:60000", "Tenant 1"},
            { "localhost:60001", "Tenant 2"},
            { "localhost:60002", "Tenant 3"},
        };

        public AppTenantResolver(IMemoryCache cache, ILoggerFactory loggerFactory) 
            : base(cache, loggerFactory)
        {

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
            string tenantName = null;
            TenantContext<AppTenant> tenantContext = null;

            if (mappings.TryGetValue(context.Request.Host.Value, out tenantName))
            {
                tenantContext = new TenantContext<AppTenant>(
                    new AppTenant { Name = tenantName, Hostnames = new[] { context.Request.Host.Value } });
            }

            return Task.FromResult(tenantContext);
        }

        protected override MemoryCacheEntryOptions CreateCacheEntryOptions()
        {
            return base.CreateCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));
        }
    }
}
