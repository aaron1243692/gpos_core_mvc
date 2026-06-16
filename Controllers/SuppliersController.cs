using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class SuppliersController : Controller
    {
        public IActionResult Index() => View();
    }
}
