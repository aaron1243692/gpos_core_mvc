using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class LogsController : Controller
    {
        public IActionResult ActivityLogs() => View();
    }
}
