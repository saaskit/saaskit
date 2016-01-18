using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace SaasKit.Multitenancy.AspNet5
{
	public abstract class HttpRequestTenantResolver<TTenant> : ITenantResolver<TTenant>
	{
		public abstract Task<TenantContext<TTenant>> ResolveAsync(HttpRequest request);

		public Task<TenantContext<TTenant>> ResolveAsync(HttpContext context)
		{
			Ensure.Argument.NotNull(context, nameof(context));
			Ensure.Argument.NotNull(context.Request, nameof(context.Request));

			return ResolveAsync(context.Request);
		}
	}
}
