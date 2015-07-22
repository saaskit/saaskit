using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy
{
    public abstract class UriTenantResolver<TTenant> : ITenantResolver<TTenant>
    {
        public abstract Task<TenantContext<TTenant>> ResolveAsync(Uri uri);

        Task<TenantContext<TTenant>> ITenantResolver<TTenant>.ResolveAsync(IDictionary<string, object> environment)
        {
            Ensure.Argument.NotNull(environment, "environment");

            var owinContext = new OwinContext(environment);
            return ResolveAsync(owinContext.Request.Uri);
        }
    }
}
