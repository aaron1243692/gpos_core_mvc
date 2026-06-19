using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace gpos.Controllers
{
    public class SalesmanController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var employeeId = HttpContext.Session.GetString("EmployeeId");

            if (string.IsNullOrWhiteSpace(employeeId))
            {
                context.Result = RedirectToAction("Login", "SalesmanAuth");
                return;
            }

            base.OnActionExecuting(context);
        }

        public IActionResult Dashboard() => View();
        public IActionResult OpenShift() => View();
        public IActionResult BeginningCash() => View();
        public IActionResult PumpTransaction() => View();
        public IActionResult POS() => View();
        public IActionResult CustomerSearch() => View();
        public IActionResult MySales() => View();
        public IActionResult CloseShift() => View();
    }
}
