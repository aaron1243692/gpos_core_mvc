using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class OperationsController : Controller
    {
        public IActionResult Index() => View();
    }
}
