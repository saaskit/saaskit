using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Caching.Memory;

namespace SaasKit.Multitenancy.AspNet5
{
	public class CachedTenantResolver<TTenant> : ITenantResolver<TTenant>
	{
		private readonly ITenantResolver<TTenant> _tenantResolver;
		private readonly Func<TTenant, IEnumerable<string>> _tenantIdentification;
		private readonly RequestIdentificationStrategy _requestIdentification;
		

		public CachedTenantResolver(
			ITenantResolver<TTenant> tenantResolver, 
			Func<TTenant, IEnumerable<string>> tenantIdentification,
			RequestIdentificationStrategy requestIdentification)
		{
			Ensure.Argument.NotNull(tenantResolver, "tenantResolver");
			Ensure.Argument.NotNull(tenantIdentification, "tenantIdentification");
			Ensure.Argument.NotNull(requestIdentification, "requestIdentification");
			
			this._tenantResolver = tenantResolver;
			this._tenantIdentification = tenantIdentification;
			this._requestIdentification = requestIdentification;
		}

		public async Task<TenantContext<TTenant>> ResolveAsync(HttpContext context)
		{
			Ensure.Argument.NotNull(context, "context");

			// Obtain the cache key from the environment
			var cacheKey = _requestIdentification(context);

			var tenantContext = MemoryCache().Get(cacheKey) as TenantContext<TTenant>;

			if (tenantContext == null)
			{
				tenantContext = await _tenantResolver.ResolveAsync(context);

				if (tenantContext != null)
				{
					var tenantIdentifiers = _tenantIdentification(tenantContext.Tenant);

					foreach (var identifier in tenantIdentifiers)
					{
						MemoryCache().Set(identifier, tenantContext, MemoryCacheEntryOptions());
					}
				}
			}

			return tenantContext;
		}

		#region IMemoryCache

		private IMemoryCache _cache;
		public CachedTenantResolver<TTenant> SetMemoryCache(IMemoryCache cache)
		{
			this._cache = cache;
			return this;
		}
		private IMemoryCache MemoryCache()
		{
			if (this._cache == null)
			{
				this._cache = new MemoryCache(new MemoryCacheOptions());
			}
			return _cache;
		}

		#endregion

		#region MemoryCacheEntryOptions

		private MemoryCacheEntryOptions _cacheOptions;
		public CachedTenantResolver<TTenant> SetMemoryCacheEntryOptions(MemoryCacheEntryOptions cacheOptions)
		{
			this._cacheOptions = cacheOptions;
			return this;
		}
		private MemoryCacheEntryOptions MemoryCacheEntryOptions()
		{
			if (this._cacheOptions == null)
			{
				// set default to cache for 1 hour
				this._cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(new TimeSpan(1, 0, 0));
			}
			return _cacheOptions;
		}

		#endregion
	}
}
