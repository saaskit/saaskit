
namespace SaasKit
{
    public static class RequestIdentificationStategies
    {
        public static RequestIdentificationStrategy FromHostname
        {
            get
            {
                return req => req.Uri.Host.ToLowerInvariant();
            }
        }
    }
}
