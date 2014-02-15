using Nancy;
using Nancy.Owin;

namespace SaasKit.Integration.Nancy
{
    public static class NancyContextExtensions
    {
        public static TenantInstance GetTenantInstance(this NancyContext context)
        {
            var owinEnvironment = context.GetOwinEnvironment();
            object tenant;
            return owinEnvironment.TryGetValue(Constants.OwinCurrentTenant, out tenant) ? (TenantInstance)tenant : null;
        }
    }
}
