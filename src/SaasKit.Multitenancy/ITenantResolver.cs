using Microsoft.AspNetCore.Http;

namespace SaasKit.Multitenancy
{
    public interface ITenantResolver<TTenant>
    {
        Task<TenantContext<TTenant>> ResolveAsync(HttpContext context);
    }
}