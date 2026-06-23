using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [NonController]
    [BlockSalesmanSession]
    public class ShiftController : Controller
    {
        public IActionResult Schedule() => RedirectToAction("EmployeeSchedules", "Configuration");
    }
}
