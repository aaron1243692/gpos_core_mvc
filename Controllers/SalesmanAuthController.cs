using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [Route("Salesman")]
    public class SalesmanAuthController : Controller
    {
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return RedirectToAction("Index", "SignIn");
        }

        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public IActionResult LoginPost()
        {
            return RedirectToAction("Index", "SignIn");
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "SignIn");
        }
    }
}
