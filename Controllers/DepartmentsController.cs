using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class DepartmentsController : Controller
    {
        public IActionResult Index() => View();
    }
}
