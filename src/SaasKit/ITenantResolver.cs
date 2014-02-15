using System.Threading.Tasks;

namespace SaasKit
{
    public interface ITenantResolver
    {
        Task<ITenant> Resolve(string requestIdentifier);
    }
}
