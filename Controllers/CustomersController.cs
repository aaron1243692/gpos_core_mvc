using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class CustomersController : Controller
    {
        public IActionResult Index() => View();
    }
}
