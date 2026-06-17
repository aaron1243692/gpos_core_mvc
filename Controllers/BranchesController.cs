using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class BranchesController : Controller
    {
        public IActionResult Index() => View();
    }
}
