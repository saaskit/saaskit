using Microsoft.AspNet.Http;

namespace SaasKit.Multitenancy.AspNet5
{
	public static class HttpContextExtensions
	{
		private const string TenantContextKey = "saaskit:tenantContext";

		public static void SetTenantContext<TTenant>(this HttpContext context, TenantContext<TTenant> tenantContext)
		{
			Ensure.Argument.NotNull(context, nameof(context));
			Ensure.Argument.NotNull(tenantContext, nameof(tenantContext));
			
			context.Items.Add(TenantContextKey, tenantContext);
		}

		public static TenantContext<TTenant> GetTenantContext<TTenant>(this HttpContext context)
		{
			Ensure.Argument.NotNull(context, nameof(context));

			object tenantContext;
			if (context.Items.TryGetValue(TenantContextKey, out tenantContext))
			{
				return tenantContext as TenantContext<TTenant>;
			}

			return null;
		}

		public static TTenant GetTenant<TTenant>(this HttpContext context)
		{
			Ensure.Argument.NotNull(context, nameof(context));

			var tenantContext = GetTenantContext<TTenant>(context);

			if (tenantContext != null)
			{
				return tenantContext.Tenant;
			}

			return default(TTenant);
		}
	}
}
