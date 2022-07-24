using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Wishmaster.Mvc;
using Wishmaster.Services;
using Wishmaster.ViewModels;

namespace Wishmaster.Controllers
{
    [Route("api")]
    public class AppDataController : BaseController
    {
        private readonly INavigationService _navigationService;

        public AppDataController(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        [HttpGet("get-app-data")]
        public IActionResult GetAppData()
        {
            var links = _navigationService.GetSidebarLinks();

            var viewModel = new AppDataViewModel 
            {
                SidebarNavigation = links.Select(x => x.ToViewModel()).ToArray(),
            };

            return Success(viewModel);
        }
    }
}
