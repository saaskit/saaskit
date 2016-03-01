using Microsoft.AspNet.Http;
using System;

namespace SaasKit.Multitenancy
{
    public class TenantPipelineBuilderContext<TTenant>
    {
        public TenantContext<TTenant> TenantContext { get; set; }
        public TTenant Tenant { get; set; }
    }
}
