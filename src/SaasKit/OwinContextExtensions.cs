using Microsoft.Owin;
using System;

namespace SaasKit
{
    public static class OwinContextExtensions
    {
        public static TenantInstance GetTenantInstance(this IOwinContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return context.Get<TenantInstance>(Constants.OwinCurrentTenant);
        }
    }
}
