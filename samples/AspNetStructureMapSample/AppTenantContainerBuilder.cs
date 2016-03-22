using SaasKit.Multitenancy.StructureMap;
using StructureMap;
using System.Threading.Tasks;

namespace AspNetStructureMapSample
{
    public class AppTenantContainerBuilder : ITenantContainerBuilder<AppTenant>
    {
        private IContainer container;

        public AppTenantContainerBuilder(IContainer container)
        {
            this.container = container;
        }

        public Task<IContainer> BuildAsync(AppTenant tenant)
        {
            var tenantContainer = container.CreateChildContainer();
            tenantContainer.Configure(config =>
            {
                if (tenant.Name == "Tenant 1")
                {
                    config.ForSingletonOf<IMessageService>().Use<OtherMessageService>();
                }
                else
                {
                    config.ForSingletonOf<IMessageService>().Use<MessageService>();
                }
            });

            return Task.FromResult(tenantContainer);
        }
    }
}
