using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AspNetMvcAuthSample.Controllers
{
    public class AccountController : Controller
    {
        ILogger<AccountController> logger;

        public AccountController(ILogger<AccountController> logger)
        {
            this.logger = logger;
        }

        public IActionResult LogIn()
        {
            return View();
        }

        public IActionResult Google()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri  = "/home/about"
            };
            
            return new ChallengeResult("Google", props);
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.Authentication.SignOutAsync("Cookies");

            return RedirectToAction("index", "home");
        }
    }
}