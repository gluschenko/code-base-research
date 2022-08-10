using System;
using Microsoft.AspNetCore.Mvc;
using Wishmaster.Backend.Mvc;

namespace Wishmaster.Backend.Controllers
{
    [Route("api/space")]
    public class SpaceController : BaseController
    {
        public SpaceController()
        {

        }

        [HttpGet("list")]
        public ActionResult<string> List()
        {
            return Json("HELLO");
        }
    }
}
