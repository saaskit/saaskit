using SaasKit.Multitenancy;

namespace System.Web
{
    public static class HttpContextBaseExtensions
    {
        public static TenantContext<TTenant> GetTenantContext<TTenant>(this HttpContextBase httpContext)
        {
            Ensure.Argument.NotNull(httpContext, "httpContext");
            return httpContext.GetOwinContext().Environment.GetTenantContext<TTenant>();
        }

        public static TTenant GetTenant<TTenant>(this HttpContextBase httpContext)
        {
            Ensure.Argument.NotNull(httpContext, "httpContext");
            return httpContext.GetOwinContext().Environment.GetTenant<TTenant>();
        }
    }
}
