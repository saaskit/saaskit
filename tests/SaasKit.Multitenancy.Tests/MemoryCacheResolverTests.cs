using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy.Tests
{
	using System.Threading;

	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Http.Internal;
	using Microsoft.Extensions.Caching.Memory;
	using Microsoft.Extensions.Internal;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Primitives;

	using Xunit;

	public class MemoryCacheTenantResolverTests
	{
		private HttpContext CreateContext(string requestPath)
		{
			var context = new DefaultHttpContext();
			context.Request.Path = requestPath;

			return context;
		}

		[Fact]
		public async Task Can_retrieve_tenant_from_resolver()
		{
			var harness = new TestHarness();
			var context = CreateContext("/apple");

			var tenantContext = await harness.Resolver.ResolveAsync(context);

			Assert.NotNull(tenantContext);
			Assert.Equal("fruit", tenantContext.Tenant.Id);
		}


		[Fact]
		public async Task Can_retrieve_tenant_from_cache()
		{
			var harness = new TestHarness();
			var context = CreateContext("/apple");

			var tenantContext = await harness.Resolver.ResolveAsync(context);

			TenantContext<TestTenant> cachedTenant;

			Assert.True(harness.Cache.TryGetValue("/apple", out cachedTenant));

			Assert.Equal(tenantContext.Tenant.Id, cachedTenant.Tenant.Id);
		}

		[Fact]
		public async Task Can_retrieve_tenant_from_cache_with_different_key()
		{
			var harness = new TestHarness();
			var context = CreateContext("/apple");

			var tenantContext = await harness.Resolver.ResolveAsync(context);

			TenantContext<TestTenant> cachedTenant;

			Assert.True(harness.Cache.TryGetValue("/pear", out cachedTenant));

			Assert.Equal(tenantContext.Tenant.Id, cachedTenant.Tenant.Id);
		}

		[Fact]
		public async Task Tenant_expires_from_cache()
		{
			var harness = new TestHarness(cacheExpirationInSeconds: 1, disposeOnExpiration: true);
			var context = CreateContext("/apple");

			var tenantContext = await harness.Resolver.ResolveAsync(context);

			TenantContext<TestTenant> cachedTenant;

			Thread.Sleep(3 * 1000);

			Assert.False(harness.Cache.TryGetValue("/pear", out cachedTenant));

			Assert.Null(cachedTenant);
		}


		[Fact]
		public async Task Tenant_expires_from_cache_for_only_its_identifier()
		{
			TenantContext<TestTenant> cachedTenant;
			var harness = new TestHarness(cacheExpirationInSeconds: 2, disposeOnExpiration: false);
			var context = CreateContext("/apple");
			
			// first request for apple
			await harness.Resolver.ResolveAsync(CreateContext("/apple"));

			// wait 1 second
			Thread.Sleep(1000);

			// second request for pear
			await harness.Resolver.ResolveAsync(CreateContext("/pear"));

			// wait 1 second
			Thread.Sleep(1000);

			// apple is expired
			Assert.False(harness.Cache.TryGetValue("/apple", out cachedTenant), "Apple Exists");

			// pear is not expired
			Assert.True(harness.Cache.TryGetValue("/pear", out cachedTenant), "Pear Does Not Exist");
		}
		

		[Fact]
		public async Task Tenant_expires_from_cache_for_all_of_its_identifiers_start()
		{
			var harness = new TestHarness(cacheExpirationInSeconds: 10, disposeOnExpiration: true);
			
			// first request for apple
			await harness.Resolver.ResolveAsync(CreateContext("/apple"));

			// cache should have all 3 entries
			Assert.NotNull(harness.Cache.Get("/apple"));
			Assert.NotNull(harness.Cache.Get("/pear"));
			Assert.NotNull(harness.Cache.Get("/grape"));
			
			TenantContext<TestTenant> cachedTenant;

			// expire apple
			harness.Cache.Remove("/apple");

			// look it up again so it registers
			harness.Cache.TryGetValue("/apple", out cachedTenant);
			
			// need to spin up a new task as "long running"
			// so that MemoryCache can fire the eviction callbacks first
			await Task.Factory.StartNew(state => {
				Thread.Sleep(500);

				// pear is expired - because apple is
				Assert.False(harness.Cache.TryGetValue("/pear", out cachedTenant), "Pear Exists");
			}, this, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.FromCurrentSynchronizationContext());
		}

		class TestTenant : IDisposable
		{
			private bool disposed;

			public string Id { get; set; }

			public List<string> Paths { get; set; }

			public CancellationTokenSource Cts = new CancellationTokenSource();

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (disposed)
				{
					return;
				}

				if (disposing)
				{
					Cts.Cancel();
				}

				disposed = true;
			}
		}

		class TestTenantMemoryCacheResolver : MemoryCacheTenantResolver<TestTenant>
		{
			readonly List<TestTenant> tenants = new List<TestTenant>()
										   {
											   new TestTenant() { Id = "fruit", Paths = new List<string>() { "/apple","/pear","/grape" }},
											   new TestTenant() { Id = "vegetable", Paths = new List<string>() { "/lettuce","/carrot","/onion" }}
										   };

			private readonly int cacheExpirationInSeconds;

			public TestTenantMemoryCacheResolver(IMemoryCache cache, ILoggerFactory loggerFactory, bool disposeOnExpiration = true, int cacheExpirationInSeconds = 10)
				: base(cache, loggerFactory)
			{
				this.DisposeTenantOnExpiration = disposeOnExpiration;
				this.cacheExpirationInSeconds = cacheExpirationInSeconds;
			}

			protected override bool DisposeTenantOnExpiration { get; }

			protected override MemoryCacheEntryOptions CreateCacheEntryOptions()
			{
				return new MemoryCacheEntryOptions()
					.SetSlidingExpiration(TimeSpan.FromSeconds(cacheExpirationInSeconds));
			}

			protected override string GetContextIdentifier(HttpContext context)
			{
				return context.Request.Path;
			}

			protected override IEnumerable<string> GetTenantIdentifiers(TenantContext<TestTenant> context)
			{
				return context?.Tenant?.Paths;
			}

			protected override Task<TenantContext<TestTenant>> ResolveAsync(HttpContext context)
			{
				var tenant = tenants.FirstOrDefault(testTenant => testTenant.Paths.Contains(context.Request.Path));

				var tenantContext = new TenantContext<TestTenant>(tenant);

				tenantContext.Properties.Add("Created", DateTime.UtcNow);

				return Task.FromResult(tenantContext);
			}
		}

		class TestHarness
		{
			static ILoggerFactory loggerFactory = new LoggerFactory().AddConsole();

			public IMemoryCache Cache = new MemoryCache(new MemoryCacheOptions()
				                                            {
																// for testing purposes, we'll scan every 100 milliseconds
					                                            ExpirationScanFrequency = TimeSpan.FromMilliseconds(100),
																Clock = new SystemClock()
				                                            });

			public TestHarness(bool disposeOnExpiration = false, int cacheExpirationInSeconds = 10)
			{
				Resolver = new TestTenantMemoryCacheResolver(Cache, loggerFactory, disposeOnExpiration, cacheExpirationInSeconds);
			}

			public ITenantResolver<TestTenant> Resolver { get; }
		}
	}
}
