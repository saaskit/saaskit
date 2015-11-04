using Microsoft.AspNet.Mvc;
using SaasKit.Multitenancy.AspNet5;

namespace SaasKit.Multitenancy.Samples.Mvc.AspNet5.Controllers
{
	public class HomeController : Controller
	{
		// GET: /<controller>/
		public IActionResult Index()
		{
			var tenantContext = Context.GetTenantContext<AppTenant>();
			return View(tenantContext);
		}
	}
}
