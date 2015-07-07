using Microsoft.Owin;

namespace SaasKit.Multitenancy
{
    public static class RequestIdentification
    {
        public static RequestIdentificationStrategy FromHostname()
        {
            return env => new OwinContext(env).Request.Uri.Host;
        }
    }
}
