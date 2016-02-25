using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.TestHost;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace SaasKit.Multitenancy.Tests
{
    public class TenantPipelineMiddlewareTests
    {
        [Fact]
        public async Task Should_create_middleware_per_tenant()
        {
            var server = TestServer.Create(app =>
            {
                app.Use(async (ctx, next) =>
                {
                    var name = ctx.Request.Path == "/t1" ? "Tenant 1" : "Tenant 2";
                    ctx.SetTenantContext(new TenantContext<AppTenant>(new AppTenant { Name = name }));
                    await next();
                });

                app.UsePerTenant<AppTenant>((context, builder) =>
                {
                    builder.UseMiddleware<WriteNameMiddleware>(context.Tenant.Name);
                });

                app.Run(async ctx =>
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                    await ctx.Response.WriteAsync(": Test");
                });
            });

            var client = server.CreateClient();

            var content = await client.GetStringAsync("/t1");
            Assert.Equal("Tenant 1: Test", content);

            content = await client.GetStringAsync("/t2");
            Assert.Equal("Tenant 2: Test", content);
        }

        public class AppTenant
        {
            public string Name { get; set; }
        }

        public class WriteNameMiddleware
        {
            private RequestDelegate next;
            private string name;

            public WriteNameMiddleware(RequestDelegate next, string name)
            {
                this.next = next;
                this.name = name;
            }

            public async Task Invoke(HttpContext context)
            {
                await context.Response.WriteAsync(name);
                await next(context);
            }
        }
    }
}
