using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Extensions;

namespace SaasKit.Multitenancy.AspNet5
{
	public abstract class UriTenantResolver<TTenant> : ITenantResolver<TTenant>
	{
		public abstract Task<TenantContext<TTenant>> ResolveAsync(Uri uri);

		public Task<TenantContext<TTenant>> ResolveAsync(HttpContext context)
		{
			Ensure.Argument.NotNull(context, nameof(context));
			var builder = new UriBuilder(context.Request.GetEncodedUrl());

			return ResolveAsync(builder.Uri);
		}
	}
}
