using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy
{
    public class PrimaryHostnameRedirectMiddleware<TTenant>
    {
        private readonly Func<IDictionary<string, object>, Task> next;
        private readonly Func<TTenant, string> primaryHostnameAccessor;
        private readonly bool permanentRedirect;

        public PrimaryHostnameRedirectMiddleware(
            Func<IDictionary<string, object>, Task> next,
            Func<TTenant, string> primaryHostnameAccessor,
            bool permanentRedirect)
        {
            Ensure.Argument.NotNull(next, "next");
            Ensure.Argument.NotNull(primaryHostnameAccessor, "primaryHostnameAccessor");

            this.next = next;
            this.primaryHostnameAccessor = primaryHostnameAccessor;
            this.permanentRedirect = permanentRedirect;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            Ensure.Argument.NotNull(environment, "environment");
            
            var tenantContext = environment.GetTenantContext<TTenant>();

            if (tenantContext != null)
            {
                var primaryHostname = primaryHostnameAccessor(tenantContext.Tenant);

                if (!string.IsNullOrEmpty(primaryHostname))
                {
                    var owinContext = new OwinContext(environment);

                    if (!owinContext.Request.Uri.Host.Equals(primaryHostname, StringComparison.OrdinalIgnoreCase))
                    {
                        Redirect(owinContext, primaryHostname);
                        return;
                    }
                }

            }

            // otherwise continue processing
            await next(environment);
        }

        private void Redirect(IOwinContext owinContext, string primaryHostname)
        {
            var builder = new UriBuilder(owinContext.Request.Uri);
            builder.Host = primaryHostname;

            owinContext.Response.Redirect(builder.Uri.AbsoluteUri);

            if (permanentRedirect)
            {
                owinContext.Response.StatusCode = 301;
            }
        }
    }
}
