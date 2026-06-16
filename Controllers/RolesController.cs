using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class RolesController : Controller
    {
        public IActionResult Index() => View();
    }
}
