using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy
{
    public class TenantNotFoundRedirectMiddleware<TTenant>
    {
        private readonly Func<IDictionary<string, object>, Task> next;
        private readonly string redirectLocation;
        private readonly bool permanentRedirect;

        public TenantNotFoundRedirectMiddleware(
            Func<IDictionary<string, object>, Task> next,
            string redirectLocation,
            bool permanentRedirect)
        {
            Ensure.Argument.NotNull(next, "next");
            Ensure.Argument.NotNull(redirectLocation, "redirectLocation");

            this.next = next;
            this.redirectLocation = redirectLocation;
            this.permanentRedirect = permanentRedirect;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            Ensure.Argument.NotNull(environment, "environment");
            
            var tenantContext = environment.GetTenantContext<TTenant>();

            if (tenantContext == null)
            {
                // Redirect to the specified location
                var owinContext = new OwinContext(environment);
                owinContext.Response.Redirect(redirectLocation);

                if (permanentRedirect)
                {
                    owinContext.Response.StatusCode = 301;
                }

                return;
            }

            // otherwise continue processing
            await next(environment);
        }
    }
}
