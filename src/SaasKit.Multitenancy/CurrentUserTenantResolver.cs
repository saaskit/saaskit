using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy
{
    public abstract class CurrentUser<TTenant> : ITenantResolver<TTenant>
    {
        public abstract Task<TenantContext<TTenant>> ResolveAsync(IPrincipal user);

        Task<TenantContext<TTenant>> ITenantResolver<TTenant>.ResolveAsync(IDictionary<string, object> environment)
        {
            Ensure.Argument.NotNull(environment, "environment");

            var owinContext = new OwinContext(environment);
            return ResolveAsync(owinContext.Request.User);
        }
    }
}
