using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy
{
    public interface ITenantResolver<TTenant>
    {
        Task<TenantContext<TTenant>> ResolveAsync(IDictionary<string, object> environment);
    }
}
