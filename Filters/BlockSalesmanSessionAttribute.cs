using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace gpos.Filters
{
    public class BlockSalesmanSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var hasUserSession =
                !string.IsNullOrWhiteSpace(session.GetString("UserId")) ||
                !string.IsNullOrWhiteSpace(session.GetString("Username")) ||
                context.HttpContext.User.Identity?.IsAuthenticated == true;

            if (!hasUserSession)
            {
                context.Result = new RedirectToActionResult("Index", "SignIn", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
