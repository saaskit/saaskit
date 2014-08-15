using System.Threading.Tasks;

namespace SaasKit.Model
{
    public interface ITenantResolver
    {
        Task<ITenant> Resolve(string requestIdentifier);
    }
}
