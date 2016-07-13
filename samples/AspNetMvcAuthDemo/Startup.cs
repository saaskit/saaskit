using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetMvcAuthSample
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            builder.AddUserSecrets();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMultitenancy<AppTenant, CachingAppTenantResolver>();

            // Add framework services.
            services.AddMvc();

            services.Configure<MultitenancyOptions>(Configuration.GetSection("Multitenancy"));
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
				builder.UseCookieAuthentication(new CookieAuthenticationOptions()
				{
					AuthenticationScheme = "Cookies",
					LoginPath = new PathString("/account/login"),
					AccessDeniedPath = new PathString("/account/forbidden"),
					AutomaticAuthenticate = true,
					AutomaticChallenge = true,
					CookieName = $"{ctx.Tenant.Id}.AspNet.Cookies"
				});


				// only register for google if ClientId and ClientSecret both exist
				var clientId = Configuration[$"{ctx.Tenant.Id}:GoogleClientId"];
				var clientSecret = Configuration[$"{ctx.Tenant.Id}:GoogleClientSecret"];

				if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(clientSecret))
				{
					builder.UseGoogleAuthentication(new GoogleOptions()
					{
						AuthenticationScheme = "Google",
						SignInScheme = "Cookies",

						ClientId = clientId,
						ClientSecret = clientSecret

					});
				}
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
