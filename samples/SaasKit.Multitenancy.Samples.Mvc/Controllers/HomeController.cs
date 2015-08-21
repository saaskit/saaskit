using System.Web;
using System.Web.Mvc;

namespace SaasKit.Multitenancy.Samples.Mvc.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var tenantContext = Request.GetTenantContext<AppTenant>();
            return View(tenantContext);
        }
    }
}