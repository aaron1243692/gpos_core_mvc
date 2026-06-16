using gpos.Models;
using gpos.Services;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [Route("Salesman")]
    public class SalesmanAuthController : Controller
    {
        private readonly EmployeeAuthService _employeeAuthService;

        public SalesmanAuthController(EmployeeAuthService employeeAuthService)
        {
            _employeeAuthService = employeeAuthService;
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View(new SalesmanLoginViewModel());
        }

        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(SalesmanLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _employeeAuthService.ValidateSalesmanAsync(model.Username, model.Password);

            if (!result.Succeeded || result.Employee is null)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            var employee = result.Employee;
            HttpContext.Session.SetString("EmployeeId", employee.Id.ToString());
            HttpContext.Session.SetString("EmployeeName", employee.Name);
            HttpContext.Session.SetString("EmployeeUsername", employee.Username);
            HttpContext.Session.SetString("EmployeeRole", employee.Role);

            return RedirectToAction("POS", "Salesman");
        }

        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("EmployeeId");
            HttpContext.Session.Remove("EmployeeName");
            HttpContext.Session.Remove("EmployeeUsername");
            HttpContext.Session.Remove("EmployeeRole");

            return RedirectToAction(nameof(Login));
        }
    }
}
