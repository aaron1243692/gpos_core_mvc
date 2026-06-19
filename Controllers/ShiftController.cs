using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class ShiftController : Controller
    {
        public IActionResult Schedule() => RedirectToAction("ShiftSchedule", "Configuration");
    }
}
