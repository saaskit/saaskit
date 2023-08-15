using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AspNetSample
{
    public class LogTenantMiddleware
    {
        RequestDelegate next;
        private readonly ILogger log;

        public LogTenantMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;
            this.log = loggerFactory.CreateLogger<LogTenantMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            var tenantContext = context.GetTenantContext<AppTenant>();

            if (tenantContext != null)
            {
                var timestamp = ((DateTime)tenantContext.Properties["Created"]);

                await context.Response.WriteAsync(
                    $"Tenant \"{tenantContext.Tenant.Name}\" created at {timestamp.Ticks}");
            }
            else
            {
                await context.Response.WriteAsync("No matching tenant found.");
            }
        }
    }
}
