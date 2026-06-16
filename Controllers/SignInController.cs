using System.Security.Claims;
using gpos.Models;
using gpos.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    public class SignInController : Controller
    {
        private readonly UserAuthService _userAuthService;

        public SignInController(UserAuthService userAuthService)
        {
            _userAuthService = userAuthService;
        }

        public IActionResult Index()
        {
            return View(new SignInViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SignInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userAuthService.ValidateUserAsync(model.Login, model.Password);

            if (!result.Succeeded || result.User is null)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            var user = result.User;
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Name", user.Username);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", result.RoleCode);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, result.RoleCode),
                new("username", user.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe
                });

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Index));
        }
    }
}
