using StructureMap;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy.StructureMap
{
    public interface ITenantContainerBuilder<TTenant>
    {
        Task<IContainer> BuildAsync(TTenant tenant);
    }
}
