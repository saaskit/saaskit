using System;
using System.Runtime.Caching;
using SaasKit.Model;

namespace SaasKit
{
    public class MemoryCacheInstanceStore<T> : IInstanceStore<T> 
        where T: class, ITenant
    {
        private const string CacheName = "SaasKit.MultiTenantEngine";
        private readonly MemoryCache cache = new MemoryCache(CacheName);

        private readonly InstanceLifetimeOptions lifetimeOptions;

        public MemoryCacheInstanceStore(InstanceLifetimeOptions lifetimeOptions)
        {
            if (lifetimeOptions == null)
            {
                throw new ArgumentNullException("lifetimeOptions");
            }

            this.lifetimeOptions = lifetimeOptions;
        }

        public T Get(string requestIdentifier)
        {
            if (string.IsNullOrEmpty(requestIdentifier))
            {
                throw new ArgumentNullException("requestIdentifier");
            }

            return cache.Get(requestIdentifier) as T;
        }

        public void Add(T instance, Action<string, T> removedCallback)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            if (removedCallback == null)
            {
                removedCallback = delegate { };
            }

            // Add a cache entry for the instance id that will be used as a cache dependency
            // for the running instances
            var cacheDependencyPolicy = new CacheItemPolicy
            {
                // Make the "MaxAge" configurable - when the dependency is removed so are the instances
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(12)
            };

            var cacheDependency = new CacheItem(instance.Id.ToString(), instance.Id);

            // We need to add the dependency before we set up the instance change monitors,
            // otherwise they'll be removed from the cache immediately!
            cache.Set(instance.Id.ToString(), instance.Id, cacheDependencyPolicy);

            // Now cache the running instance for each identifier
            // The policies must be unique since the RemovedCallback can only be called once-per-policy

            cache.Set(new CacheItem(instance.Id, instance), GetCacheItemPolicy(removedCallback, cacheDependency));
        }

        public void Remove(string instanceId)
        {
            if (string.IsNullOrEmpty(instanceId))
            {
                throw new ArgumentNullException("instanceId");
            }

            cache.Remove(instanceId);
        }

        private CacheItemPolicy GetCacheItemPolicy(Action<string, T> removedCallback, CacheItem cacheDependency)
        {
            var policy = new CacheItemPolicy();

            if (lifetimeOptions.UseSlidingExpiration)
            {
                policy.SlidingExpiration = lifetimeOptions.Lifetime;
            }
            else
            {
                policy.AbsoluteExpiration = DateTimeOffset.UtcNow.Add(lifetimeOptions.Lifetime);
            }

            policy.RemovedCallback = args => removedCallback(args.CacheItem.Key, args.CacheItem.Value as T);

            // make the cache item dependent on the provided cache dependency
            var changeMonitor = cache.CreateCacheEntryChangeMonitor(new[] { cacheDependency.Key });
            policy.ChangeMonitors.Add(changeMonitor);

            return policy;
        }
    }
}
