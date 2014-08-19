using Nancy;
using Nancy.Owin;
using SaasKit.Model;

namespace SaasKit.Integration.Nancy
{
    public static class NancyContextExtensions
    {
        public static BaseTenant GetTenantInstance(this NancyContext context)
        {
            var owinEnvironment = context.GetOwinEnvironment();
            object tenant;
            return owinEnvironment.TryGetValue(Constants.OwinCurrentTenant, out tenant) ? (BaseTenant)tenant : null;
        }
    }
}
