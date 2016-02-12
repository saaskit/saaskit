using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace AspNetMvcSample.Models
{
    // https://ef.readthedocs.org/en/latest/

    public class SqliteApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IApplicationEnvironment env;
        private readonly AppTenant tenant;

        public SqliteApplicationDbContext(IApplicationEnvironment env, AppTenant tenant)
        {
            this.env = env;
            this.tenant = tenant;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var tenantDbName = tenant.Name.Replace(" ", "-").ToLowerInvariant();
            var connectionString = $"FileName={Path.Combine(env.ApplicationBasePath, "App_Data", tenantDbName)}.db";
            optionsBuilder.UseSqlite(connectionString);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
