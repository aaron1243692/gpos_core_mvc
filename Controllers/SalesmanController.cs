using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace gpos.Controllers
{
    public class SalesmanController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var employeeRole = HttpContext.Session.GetString("EmployeeRole");

            if (!string.Equals(employeeRole, "salesman", StringComparison.OrdinalIgnoreCase))
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
