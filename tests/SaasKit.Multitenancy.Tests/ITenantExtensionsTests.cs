using Xunit;

namespace SaasKit.Multitenancy.Tests
{
    public class ITenantExtensionsTests
    {
        [Fact]
        public void Can_Create_Local_Tenant()
        {
            var tenantName = "Tenant 1";

            ITenant<AppTenant> tenant = ITenant.Create(new AppTenant { Name = tenantName });

            Assert.Equal(tenantName, tenant.Value.Name);
        }
    }

    public class AppTenant
    {
        public string Name { get; set; }
    }
}
