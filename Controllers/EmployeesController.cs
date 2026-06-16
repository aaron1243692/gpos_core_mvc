using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class EmployeesController : Controller
    {
        public IActionResult Index() => View();
    }
}
