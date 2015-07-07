using System;
using System.Collections.Generic;

namespace SaasKit.Multitenancy
{
    public static class OwinEnvironmentExtensions
    {
        private const string TenantContextKey = "saaskit:tenantContext";

        public static void SetTenantContext<TTenant>(this IDictionary<string, object> environment, TenantContext<TTenant> tenantContext)
        {
            Ensure.Argument.NotNull(environment, "environment");
            Ensure.Argument.NotNull(tenantContext, "tenantContext");

            environment.Add(TenantContextKey, tenantContext);
        }

        public static TenantContext<TTenant> GetTenantContext<TTenant>(this IDictionary<string, object> environment)
        {
            Ensure.Argument.NotNull(environment, "environment");

            object tenantContext;
            if (environment.TryGetValue(TenantContextKey, out tenantContext))
            {
                return tenantContext as TenantContext<TTenant>;
            }

            return null;
        }

        public static TTenant GetTenant<TTenant>(this IDictionary<string, object> environment)
        {
            Ensure.Argument.NotNull(environment, "environment");

            var tenantContext = GetTenantContext<TTenant>(environment);

            if (tenantContext != null)
            {
                return tenantContext.Tenant;
            }

            return default(TTenant);
        }
    }
}
