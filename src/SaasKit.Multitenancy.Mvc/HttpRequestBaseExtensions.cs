using SaasKit.Multitenancy;

namespace System.Web
{
    public static class HttpRequestBaseExtensions
    {
        public static TenantContext<TTenant> GetTenantContext<TTenant>(this HttpRequestBase request)
        {
            Ensure.Argument.NotNull(request, "request");
            return request.GetOwinContext().Environment.GetTenantContext<TTenant>();
        }

        public static TTenant GetTenant<TTenant>(this HttpRequestBase request)
        {
            Ensure.Argument.NotNull(request, "request");
            return request.GetOwinContext().Environment.GetTenant<TTenant>();
        }
    }
}
