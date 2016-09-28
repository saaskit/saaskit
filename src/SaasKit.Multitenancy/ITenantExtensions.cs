using SaasKit.Multitenancy.Internal;

namespace SaasKit.Multitenancy
{
    public static class ITenant
    {
        public static ITenant<ITenant> Create<ITenant>(ITenant tenant) where ITenant : class, new()
        {
            return new TenantWrapper<ITenant>(tenant);
        }
    }
}
