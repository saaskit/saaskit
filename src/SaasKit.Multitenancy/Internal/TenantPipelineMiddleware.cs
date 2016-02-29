using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy.Internal
{
    public class TenantPipelineMiddleware<TTenant>
    {
        private readonly RequestDelegate next;
        private readonly IApplicationBuilder rootApp;
        private readonly Action<TenantPipelineBuilderContext<TTenant>, IApplicationBuilder> configuration;

        private readonly ConcurrentDictionary<TTenant, RequestDelegate> pipelines
            = new ConcurrentDictionary<TTenant, RequestDelegate>();

        public TenantPipelineMiddleware(
            RequestDelegate next, 
            IApplicationBuilder rootApp, 
            Action<TenantPipelineBuilderContext<TTenant>, IApplicationBuilder> configuration)
        {
            Ensure.Argument.NotNull(next, nameof(next));
            Ensure.Argument.NotNull(rootApp, nameof(rootApp));
            Ensure.Argument.NotNull(configuration, nameof(configuration));

            this.next = next;
            this.rootApp = rootApp;
            this.configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            Ensure.Argument.NotNull(context, nameof(context));

            var tenantContext = context.GetTenantContext<TTenant>();

            if (tenantContext != null)
            {
                var tenantPipeline = pipelines.GetOrAdd(tenantContext.Tenant, tenant =>
                {
                    var branchBuilder = rootApp.New();

                    var builderContext = new TenantPipelineBuilderContext<TTenant>
                    {
                        RequestServices = context.RequestServices,
                        TenantContext = tenantContext,
                        Tenant = tenantContext.Tenant
                    };

                    configuration(builderContext, branchBuilder);
                    return branchBuilder.Build();
                });

                await tenantPipeline(context);
            }

            // Connect back to the root app pipeline
            await next(context);
        }
    }
}
