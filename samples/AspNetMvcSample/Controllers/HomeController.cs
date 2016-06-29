using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SaasKit.Multitenancy;

namespace AspNetMvcSample.Controllers
{

	public class HomeController : Controller
    {
        private readonly AppTenant tenant;

        public HomeController(ITenant<AppTenant> tenant)
        {
            this.tenant = tenant?.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = $"Your application description page for {tenant?.Name ?? "Default"}";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
