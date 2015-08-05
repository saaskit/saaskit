using SaasKit.Multitenancy;
using System;

namespace Owin
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseMultitenancy<TTenant>(this IAppBuilder app, ITenantResolver<TTenant> tenantResolver)
        {
            Ensure.Argument.NotNull(app, "app");
            Ensure.Argument.NotNull(tenantResolver, "tenantResolver");

            return app.UseMultitenancy(() => tenantResolver);
        }
        
        public static IAppBuilder UseMultitenancy<TTenant>(this IAppBuilder app, Func<ITenantResolver<TTenant>> tenantResolverFactory)
        {
            Ensure.Argument.NotNull(app, "app");
            Ensure.Argument.NotNull(tenantResolverFactory, "tenantResolverFactory");
            
            app.Use(typeof(TenantResolutionMiddleware<TTenant>), tenantResolverFactory);
            return app;
        }

        public static IAppBuilder RedirectIfTenantNotFound<TTenant>(this IAppBuilder app, string redirectLocation, bool permanentRedirect = false)
        {
            Ensure.Argument.NotNull(app, "app");
            Ensure.Argument.NotNullOrEmpty(redirectLocation, "redirectLocation");

            app.Use(typeof(TenantNotFoundRedirectMiddleware<TTenant>), redirectLocation, permanentRedirect);
            return app;
        }

        public static IAppBuilder RedirectToPrimaryHostname<TTenant>(this IAppBuilder app, Func<TTenant, string> primaryHostnameAccessor, bool permanentRedirect = true)
        {
            Ensure.Argument.NotNull(app, "app");
            Ensure.Argument.NotNull(primaryHostnameAccessor, "primaryHostnameAccessor");

            app.Use(typeof(PrimaryHostnameRedirectMiddleware<TTenant>), 
                primaryHostnameAccessor, permanentRedirect);

            return app;
        }

        public static IAppBuilder IfTenantNotFound<TTenant>(this IAppBuilder app, Func<TTenant> tenantFunc)
        {
           Ensure.Argument.NotNull(tenantFunc, "tenantFunc");
           app.Use(typeof (TenantNotFoundMiddleware<TTenant>), tenantFunc);

          return app;
       }
    }
}
