using Owin;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaasKit.Demos.Nancy
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseSaasKit(ConfigureSaasKit())
               .UseWebApi(ConfigureWebApi())
               .UseNancy();
        }

        private ISaasKitEngine ConfigureSaasKit()
        {
            var config = new SaasKitConfiguration
            {
                TenantResolver = new MyResolver(),
                Logger = msg => Console.WriteLine(msg)
            };

            var instanceStore = new MemoryCacheInstanceStore(
                new InstanceLifetimeOptions { 
                    Lifetime =  TimeSpan.FromSeconds(30),
                    UseSlidingExpiration = true
                }
            );

            return new SaasKitEngine(config, instanceStore);
        }

        private HttpConfiguration ConfigureWebApi()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("Default", "api/{controller}", new { controller = "home" });

            return config;
        }
    }

    public class MyResolver : ITenantResolver
    {
        public Task<ITenant> Resolve(string tenantIdentifier)
        {           
            var tenant = new Tenant
            {
                Name = "Tenant1",
                RequestIdentifiers = new[] { "localhost", "dev.local" }
            };

            return Task.FromResult<ITenant>(tenant);
        }
    }
}