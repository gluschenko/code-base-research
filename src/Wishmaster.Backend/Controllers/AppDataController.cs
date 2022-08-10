using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Wishmaster.Backend.Mvc;
using Wishmaster.Backend.Services;
using Wishmaster.ViewModels;

namespace Wishmaster.Backend.Controllers
{
    [Route("api/app-data")]
    public class AppDataController : BaseController
    {
        private readonly INavigationService _navigationService;

        public AppDataController(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        [HttpGet("get-app-data")]
        public ActionResult<AppDataViewModel> GetAppData()
        {
            var links = _navigationService.GetSidebarLinks();

            var viewModel = new AppDataViewModel
            {
                SidebarNavigation = links.Select(x => x.ToViewModel()).ToArray(),
            };

            return Json(viewModel);
        }
    }
}
