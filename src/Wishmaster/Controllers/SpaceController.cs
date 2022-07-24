using System;
using Microsoft.AspNetCore.Mvc;
using Wishmaster.Mvc;

namespace Wishmaster.Controllers
{
    [Route("api/space")]
    public class SpaceController : BaseController
    {
        public SpaceController()
        {

        }

        [HttpGet("list")]
        public IActionResult List()
        {
            return Success("HELLO");
        }
    }
}
