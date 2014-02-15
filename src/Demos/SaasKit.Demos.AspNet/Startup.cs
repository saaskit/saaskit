using Owin;
using System.Threading.Tasks;

namespace SaasKit.Demos.AspNet
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var engine = new SaasKitEngine(new SaasKitConfiguration
            {
                TenantResolver = new MyResolver()
            });

            app.UseSaasKit(engine);
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