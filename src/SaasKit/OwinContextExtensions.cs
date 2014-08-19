using Microsoft.Owin;
using System;
using SaasKit.Model;

namespace SaasKit
{
    public static class OwinContextExtensions
    {
        public static ITenant GetTenantInstance(this IOwinContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return context.Get<ITenant>(Constants.OwinCurrentTenant);
        }
    }
}
