using System.Web;
using System.Web.Mvc;

namespace SaasKit.Multitenancy.Samples.Mvc.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var tenant = Request.GetTenant<AppTenant>();
            return View(tenant);
        }
    }
}