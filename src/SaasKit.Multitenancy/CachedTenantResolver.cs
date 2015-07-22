using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy
{
    public class CachedTenantResolver<TTenant> : ITenantResolver<TTenant>
    {
        private const string CacheName = "SaasKit.Tenants";
        private readonly MemoryCache cache = new MemoryCache(CacheName);
        
        private readonly ITenantResolver<TTenant> tenantResolver;
        private readonly Func<TTenant, IEnumerable<string>> tenantIdentification;
        private readonly RequestIdentificationStrategy requestIdentification;

        public CachedTenantResolver(
            ITenantResolver<TTenant> tenantResolver, 
            Func<TTenant, IEnumerable<string>> tenantIdentification,
            RequestIdentificationStrategy requestIdentification)
        {
            Ensure.Argument.NotNull(tenantResolver, "tenantResolver");
            Ensure.Argument.NotNull(tenantIdentification, "tenantIdentification");
            Ensure.Argument.NotNull(requestIdentification, "requestIdentification");
            
            this.tenantResolver = tenantResolver;
            this.tenantIdentification = tenantIdentification;
            this.requestIdentification = requestIdentification;
        }
        
        public async Task<TenantContext<TTenant>> ResolveAsync(IDictionary<string, object> environment)
        {
            Ensure.Argument.NotNull(environment, "environment");

            // Obtain the cache key from the environment
            var cacheKey = requestIdentification(environment);

            var tenantContext = cache.Get(cacheKey) as TenantContext<TTenant>;

            if (tenantContext == null)
            {
                tenantContext = await tenantResolver.ResolveAsync(environment);

                if (tenantContext != null)
                {
                    var tenantIdentifiers = tenantIdentification(tenantContext.Tenant);
                    var policy = new CacheItemPolicy(); // TODO

                    foreach (var identifier in tenantIdentifiers)
                    {
                        cache.Set(new CacheItem(identifier, tenantContext), policy);
                    }
                }
            }

            return tenantContext;
        }
    }
}
