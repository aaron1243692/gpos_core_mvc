using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace gpos.Filters
{
    public class BlockSalesmanSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var employeeRole = session.GetString("EmployeeRole");
            var adminRole = session.GetString("Role");

            if (string.Equals(employeeRole, "salesman", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new RedirectToActionResult("POS", "Salesman", null);
                return;
            }

            if (!string.Equals(adminRole, "admin", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new RedirectToActionResult("Index", "SignIn", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
