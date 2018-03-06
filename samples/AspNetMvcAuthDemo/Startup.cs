using AspNetMvcAuthDemo;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetMvcAuthSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        { 
            services.AddMultitenancy<AppTenant, CachingAppTenantResolver>();

            // Add framework services.
            services.AddMvc();

            services.Configure<MultitenancyOptions>(Configuration.GetSection("Multitenancy"));

            services.AddAuthentication("Cookies")
                .AddCookie("Cookies", options =>
                {
                    ////notes: cookie name set in AddTransient
                    //options.Cookie.Name = $"{ctx.Tenant.Id}.AspNet.Cookies";

                    options.LoginPath = new PathString("/account/login");
                    options.AccessDeniedPath = new PathString("/account/forbidden");
                })
                .AddGoogle("Google", options =>
                {
                    options.SignInScheme = "Cookies";

                    ////notes: clientId & clientSecret are set in AddTransient
                    //options.ClientId = Configuration[$"{ctx.Tenant.Id}:GoogleClientId"];
                    //options.ClientSecret = Configuration[$"{ctx.Tenant.Id}:GoogleClientSecret"];
                });


            services.AddSingleton(Configuration);

            services.AddTransient<IConfigureOptions<CookieAuthenticationOptions>, ConfigureMyCookie>();
            services.AddTransient<IConfigureOptions<GoogleOptions>, ConfigureMyGoogleCookie>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseDeveloperExceptionPage();
			
            app.UseStaticFiles();

            app.UseMultitenancy<AppTenant>();

            app.UsePerTenant<AppTenant>((ctx, builder) =>
			{

                builder.UseAuthentication();

                /////////////--------- to delete
                //var x = builder.ApplicationServices.GetService<IConfigureOptions<CookieAuthenticationOptions>>();

                //var y = builder.ApplicationServices.GetService<IConfigureOptions<GoogleOptions>>();
                //y.Configure(new GoogleOptions()
                //{
                //    ClientId = ctx.Tenant.GoogleClientId,
                //    ClientSecret = ctx.Tenant.GoogleClientSecret
                //});
                /////////////--------- to delete




                ////notes:-----------------------moved up
                //builder.UseCookieAuthentication(new CookieAuthenticationOptions()
                //{
                //	AuthenticationScheme = "Cookies",
                //	LoginPath = new PathString("/account/login"),
                //	AccessDeniedPath = new PathString("/account/forbidden"),
                //	AutomaticAuthenticate = true,
                //	AutomaticChallenge = true,
                //	CookieName = $"{ctx.Tenant.Id}.AspNet.Cookies"
                //});


                ////notes:-----------------------moved up but it's no longer conditional
                //// only register for google if ClientId and ClientSecret both exist
                //var clientId = Configuration[$"{ctx.Tenant.Id}:GoogleClientId"];
                //var clientSecret = Configuration[$"{ctx.Tenant.Id}:GoogleClientSecret"];

                //if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(clientSecret))
                //{
                //	builder.UseGoogleAuthentication(new GoogleOptions()
                //	{
                //		AuthenticationScheme = "Google",
                //		SignInScheme = "Cookies",

                //		ClientId = clientId,
                //		ClientSecret = clientSecret

                //	});
                //}
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
