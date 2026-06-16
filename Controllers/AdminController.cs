using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
