using AspNetMvcAuthSample;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AspNetMvcAuthDemo
{
    internal class ConfigureMyCookie : IConfigureNamedOptions<CookieAuthenticationOptions>
    {
        private readonly HttpContext _httpContext;

        public ConfigureMyCookie(IHttpContextAccessor contextAccessor)
        {
            _httpContext = contextAccessor.HttpContext;
        }

        public void Configure(string name, CookieAuthenticationOptions options)
        {
            var tenantContext = _httpContext.GetTenantContext<AppTenant>();

            if (tenantContext == null)
            {
                options.Cookie.Name = "AspNet.Cookies";
            }
            else
            {
                options.Cookie.Name = $"{tenantContext.Tenant.Id}.AspNet.Cookies";
            }
        }

        public void Configure(CookieAuthenticationOptions options)
            => Configure(Options.DefaultName, options);
    }

    internal class ConfigureMyGoogleCookie : IConfigureNamedOptions<GoogleOptions>
    {
        private readonly HttpContext _httpContext;
        private readonly IList<AppTenant> tenants;
        public GoogleOptions myCurrentOptions;

        public ConfigureMyGoogleCookie(
            IHttpContextAccessor contextAccessor, 
            IOptions<MultitenancyOptions> multitenancyOptions)
        {
            _httpContext = contextAccessor.HttpContext;
            tenants = multitenancyOptions.Value.Tenants;
        }

        public void Configure(string name, GoogleOptions options)
        {
            var tenantContext = _httpContext.GetTenantContext<AppTenant>();

            if (tenantContext == null)
                throw new Exception("Google Auth not set.");

            var currentTenant = tenants.Single(t => t.Id == tenantContext.Tenant.Id);

            options.ClientId = currentTenant.GoogleClientId;
            options.ClientSecret = currentTenant.GoogleClientSecret;

            myCurrentOptions = options;
        }

        public void Configure(GoogleOptions options)
            => Configure(Options.DefaultName, options);
    }
}
