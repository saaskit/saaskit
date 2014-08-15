using Owin;
using SaasKit.Model;
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

            var instanceStore = new MemoryCacheInstanceStore<Tenant>(
                new InstanceLifetimeOptions { 
                    Lifetime =  TimeSpan.FromSeconds(30),
                    UseSlidingExpiration = true
                }
            );

            return new SaasKitEngine<Tenant>(config, instanceStore);
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
                Id = tenantIdentifier
            };

            return Task.FromResult<ITenant>(tenant);
        }
    }
}