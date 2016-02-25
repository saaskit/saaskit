using Microsoft.AspNet.Http;

namespace SaasKit.Multitenancy
{
    public class TenantPipelineBuilderContext<TTenant>
    {
        public HttpContext HttpContext { get; set; }
        public TenantContext<TTenant> TenantContext { get; set; }
        public TTenant Tenant { get; set; }
    }
}
