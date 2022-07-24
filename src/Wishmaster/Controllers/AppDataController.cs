using Microsoft.AspNetCore.Mvc;

namespace Wishmaster.Controllers
{
    [Route("api")]
    public class AppDataController : Controller
    {
        public AppDataController()
        {

        }

        [HttpGet("get-app-data")]
        public IActionResult GetAppData()
        {
            return Json(new
            {
                Asd = 1,
            });
        }
    }
}
