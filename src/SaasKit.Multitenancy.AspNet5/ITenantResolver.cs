using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace SaasKit.Multitenancy.AspNet5
{
	public interface ITenantResolver<TTenant>
	{
		Task<TenantContext<TTenant>> ResolveAsync(HttpContext context);
	}
}
