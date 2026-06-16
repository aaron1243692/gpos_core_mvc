using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class UsersController : Controller
    {
        public IActionResult Index() => View();
    }
}
