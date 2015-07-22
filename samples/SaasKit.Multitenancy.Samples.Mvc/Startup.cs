using Owin;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy.Samples.Mvc
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseMultitenancy(new AppTenantResolver());
        }
    }

    public class AppTenant
    {
        public string Name { get; set; }
    }

    public class AppTenantResolver : UriTenantResolver<AppTenant>
    {
        public override Task<TenantContext<AppTenant>> ResolveAsync(Uri uri)
        {
            AppTenant tenant = null;

            if (uri.Host.Equals("dev.local", StringComparison.OrdinalIgnoreCase))
            {
                tenant = new AppTenant { Name = "Tenant 2" };
            }
            else // Default Tenant
            {
                tenant = new AppTenant { Name = "Tenant 1" };
            }

            return Task.FromResult(new TenantContext<AppTenant>(tenant));
        }
    }
}