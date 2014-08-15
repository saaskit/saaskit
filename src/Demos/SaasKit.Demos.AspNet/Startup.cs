using Owin;
using SaasKit.Model;
using System.Threading.Tasks;

namespace SaasKit.Demos.AspNet
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var engine = new SaasKitEngine<Tenant>(new SaasKitConfiguration
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
                Id = tenantIdentifier
            };

            return Task.FromResult<ITenant>(tenant);
        }
    }
}