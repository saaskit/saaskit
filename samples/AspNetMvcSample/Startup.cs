using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AspNetMvcSample.Models;
using AspNetMvcSample.Services;
using Microsoft.Extensions.Options;
using SaasKit.Multitenancy;
using Microsoft.Extensions.Logging.Console;
using Microsoft.AspNetCore.Mvc.Razor;

namespace AspNetMvcSample
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMultitenancy<AppTenant, CachingAppTenantResolver>();

			// Add framework services.
			//services.AddEntityFrameworkSqlServer().AddDbContext<SqlServerApplicationDbContext>();

			//services.AddIdentity<ApplicationUser, IdentityRole>()
			//	.AddEntityFrameworkStores<SqlServerApplicationDbContext>()
			//	.AddDefaultTokenProviders();

			services.AddEntityFrameworkSqlite().AddDbContext<SqliteApplicationDbContext>();

			services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<SqliteApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddOptions();

            services.AddMvc();
             
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new TenantViewLocationExpander());
            });

            services.Configure<MultitenancyOptions>(Configuration.GetSection("Multitenancy"));

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));           
            loggerFactory.AddDebug(LogLevel.Debug);

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                try
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                    {
                        serviceScope.ServiceProvider.GetService<SqlServerApplicationDbContext>()
                             .Database.Migrate();
                    }
                }
                catch { }
            }
			
            app.UseStaticFiles();

            app.UseMultitenancy<AppTenant>();

            app.UseIdentity();

            // To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
