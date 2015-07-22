using Owin;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy.Samples.Mvc
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseMultitenancy(new AppTenantResolver())
                .RedirectIfTenantNotFound<AppTenant>("https://github.com/saaskit/saaskit/wiki/Handling-unresolved-Tenants/", permanentRedirect: false);
        }
    }

    public class AppTenant
    {
        public string Name { get; set; }
    }

    public class AppTenantResolverWithDefaultInstance : UriTenantResolver<AppTenant>
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

    public class AppTenantResolver : UriTenantResolver<AppTenant>
    {
        private Dictionary<string, string> mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "localhost", "Tenant 1"},
            { "dev.local", "Tenant 2"}
        };
        
        public override Task<TenantContext<AppTenant>> ResolveAsync(Uri uri)
        {
            string tenantName = null;
            TenantContext<AppTenant> tenantContext = null;

            if (mappings.TryGetValue(uri.Host, out tenantName))
            {
                tenantContext = new TenantContext<AppTenant>(new AppTenant { Name = tenantName });
            }

            return Task.FromResult(tenantContext);
        }
    }
}