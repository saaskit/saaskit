using System;
using Microsoft.AspNet.Builder;

namespace SaasKit.Multitenancy.AspNet5
{
	public static class AppBuilderExtensions
	{

		public static IApplicationBuilder UseMultitenancy<TTenant>(this IApplicationBuilder app, ITenantResolver<TTenant> tenantResolver)
		{
			Ensure.Argument.NotNull(app, nameof(app));
			Ensure.Argument.NotNull(tenantResolver, nameof(tenantResolver));

			return app.UseMultitenancy(() => tenantResolver);
		}
		
		public static IApplicationBuilder UseMultitenancy<TTenant>(this IApplicationBuilder app, Func<ITenantResolver<TTenant>> tenantResolverFactory)
		{
			Ensure.Argument.NotNull(app, nameof(app));
			Ensure.Argument.NotNull(tenantResolverFactory, nameof(tenantResolverFactory));
			
			app.Use(next => new TenantResolutionMiddleware<TTenant>(next, tenantResolverFactory).Invoke);
			return app;
		}


		public static IApplicationBuilder RedirectIfTenantNotFound<TTenant>(this IApplicationBuilder app, string redirectLocation, bool permanentRedirect = false)
		{
			Ensure.Argument.NotNull(app, nameof(app));
			Ensure.Argument.NotNullOrEmpty(redirectLocation, nameof(redirectLocation));
			
			app.Use(next => new TenantNotFoundRedirectMiddleware<TTenant>(next, redirectLocation, permanentRedirect).Invoke);
			return app;
		}

		public static IApplicationBuilder RedirectToPrimaryHostname<TTenant>(this IApplicationBuilder app, Func<TTenant, string> primaryHostnameAccessor, bool permanentRedirect = true)
		{
			Ensure.Argument.NotNull(app, nameof(app));
			Ensure.Argument.NotNull(primaryHostnameAccessor, nameof(primaryHostnameAccessor));
			
			app.Use(next => new PrimaryHostnameRedirectMiddleware<TTenant>(next, primaryHostnameAccessor, permanentRedirect).Invoke);
			return app;
		}
	}
}
