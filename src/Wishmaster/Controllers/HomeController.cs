using Microsoft.AspNetCore.Mvc;

namespace Wishmaster.Controllers
{
    [Route("api")]
    public class HomeController : Controller
    {
        public HomeController()
        {

        }

        public IActionResult Index()
        {
            return Content("OK");
        }
    }
}
