using Microsoft.Owin;
using System.Threading.Tasks;

namespace SaasKit
{
    public interface ISaasKitEngine
    {
        Task BeginRequest(IOwinContext context);
        Task EndRequest(IOwinContext context);
    }
}
