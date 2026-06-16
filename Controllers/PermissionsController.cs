using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class PermissionsController : Controller
    {
        public IActionResult Index() => View();
    }
}
