using Microsoft.Owin;
using System;
using System.Threading.Tasks;
using SaasKit.Model;

namespace SaasKit
{
    public class SaasKitEngine<T> : ISaasKitEngine
        where T: class, ITenant
    {
        private readonly SaasKitConfiguration configuration;
        private readonly IInstanceStore<T> runningInstances;

        public SaasKitEngine(SaasKitConfiguration configuration)
            : this(configuration, new MemoryCacheInstanceStore<T>(InstanceLifetimeOptions.Default))
        {

        }

        public SaasKitEngine(SaasKitConfiguration configuration, IInstanceStore<T> instanceStore)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (instanceStore == null)
            {
                throw new ArgumentNullException("instanceStore");
            }

            if (!configuration.IsValid)
            {
                throw new InvalidOperationException("Configuration is invalid.");
            }

            this.configuration = configuration;
            this.runningInstances = instanceStore;
        }

        public async Task BeginRequest(IOwinContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            Log("Begin request.");

            var instance = await GetTenantInstance(context.Request);

            if (instance != null)
            {
                context.Set(Constants.OwinCurrentTenant, instance);
            }
            else
            {
                // TODO - support default instances (for single tenant / fallback scenarios)
            }
        }

        public Task EndRequest(IOwinContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            Log("End request.");

            return Task.FromResult(0);
        }

        private async Task<T> GetTenantInstance(IOwinRequest request)
        {
            var requestIdentifier = configuration.IdentificationStrategy(request);

            if (requestIdentifier == null)
            {
                throw new InvalidOperationException("The identification strategy did not return an identifier for the request.");
            }

            var instance = runningInstances.Get(requestIdentifier);

            if (instance != null)
            {
                Log("Instance for '{0}' is already running.", requestIdentifier);
                return instance;
            }

            Log("Instance for '{0}' not running. Resolving tenant.", requestIdentifier);

            var tenant = await configuration.TenantResolver.Resolve(requestIdentifier);

            if (tenant != null)
            {
                //Log("Tenant found with identifiers '{0}'", string.Join(",", tenant.RequestIdentifiers));
                instance = StartInstance(tenant as T);
            }

            return instance;
        }

        private T StartInstance(T tenant)
        {
            runningInstances.Add(tenant, ShutdownInstance);

            Log("Started instance '{0}'", tenant.Id);
            return tenant;
        }

        private void ShutdownInstance(string identifier, T instance)
        {
            if (instance == null)
            {
                return;
            }

            // Make sure all cache entries for this instance are removed (they are all dependent on tenant instance id)
            Log("Removing instance '{0}' identifier '{1}'", instance.Id, identifier);
            runningInstances.Remove(instance.Id.ToString());

            //lock (instance)
            //{
            //    instance.Dispose();
            //}
        }

        private void Log(string message, params object[] args)
        {
            configuration.Logger(string.Format("SaasKitEngine: " + message, args));
        }
    }
}
