using Microsoft.AspNet.Http;

namespace SaasKit.Multitenancy.AspNet5
{
	public delegate string RequestIdentificationStrategy(HttpContext context);
}
