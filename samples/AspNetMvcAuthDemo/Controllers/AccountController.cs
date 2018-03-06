using AspNetMvcAuthDemo;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
            await HttpContext.SignOutAsync("Cookies");

            return RedirectToAction("index", "home");
        }
    }
}