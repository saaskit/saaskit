using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace SaasKit.Multitenancy.AspNet5
{
	public abstract class CurrentUserTenantResolver<TTenant> : ITenantResolver<TTenant>
	{
		public abstract Task<TenantContext<TTenant>> ResolveAsync(IPrincipal user);

		Task<TenantContext<TTenant>> ITenantResolver<TTenant>.ResolveAsync(HttpContext context)
		{
			Ensure.Argument.NotNull(context, "context");
			
			return ResolveAsync(context.User);
		}
	}
}
