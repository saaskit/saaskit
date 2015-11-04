using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace SaasKit.Multitenancy.AspNet5
{
	public class TenantResolutionMiddleware<TTenant>
	{
		private readonly Func<ITenantResolver<TTenant>> tenantResolverFactory;
		
		private readonly RequestDelegate next;

		public TenantResolutionMiddleware(
			RequestDelegate next,
			Func<ITenantResolver<TTenant>> tenantResolverFactory)
		{
			Ensure.Argument.NotNull(next, nameof(next));
			Ensure.Argument.NotNull(tenantResolverFactory, nameof(tenantResolverFactory));

			this.next = next;
			this.tenantResolverFactory = tenantResolverFactory;
		}


		public async Task Invoke(HttpContext context)
		{
			Ensure.Argument.NotNull(context, nameof(context));

			var tenantResolver = tenantResolverFactory();

			var tenantContext = await tenantResolver.ResolveAsync(context);

			if (tenantContext != null)
			{
				context.SetTenantContext(tenantContext);
			}

			await next.Invoke(context);
		}
	}
}
