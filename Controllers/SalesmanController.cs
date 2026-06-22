using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace gpos.Controllers
{
    public class SalesmanController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (string.Equals(context.ActionDescriptor.RouteValues["action"], nameof(POS), StringComparison.OrdinalIgnoreCase))
            {
                base.OnActionExecuting(context);
                return;
            }

            context.Result = RedirectToAction("Index", "SignIn");
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
