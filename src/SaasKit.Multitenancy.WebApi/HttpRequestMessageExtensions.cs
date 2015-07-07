using SaasKit.Multitenancy;

namespace System.Net.Http
{
    public static class HttpRequestMessageExtensions
    {
        public static TenantContext<TTenant> GetTenantContext<TTenant>(this HttpRequestMessage request)
        {
            Ensure.Argument.NotNull(request, "request");
            return request.GetOwinContext().Environment.GetTenantContext<TTenant>();
        }

        public static TTenant GetTenant<TTenant>(this HttpRequestMessage request)
        {
            Ensure.Argument.NotNull(request, "request");
            return request.GetOwinContext().Environment.GetTenant<TTenant>();
        }
    }
}
