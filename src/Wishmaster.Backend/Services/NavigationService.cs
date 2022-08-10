using System.Collections.Generic;
using Wishmaster.Backend.Controllers;
using Wishmaster.Backend.Models.Navigation;
using Wishmaster.Backend.Mvc;

namespace Wishmaster.Backend.Services
{
    public interface INavigationService
    {
        IEnumerable<NavLink> GetSidebarLinks();
    }

    public class NavigationService : INavigationService
    {
        private readonly UrlHelperAccessor _urlHelperAccessor;

        public NavigationService(UrlHelperAccessor urlHelperAccessor)
        {
            _urlHelperAccessor = urlHelperAccessor;
        }

        public IEnumerable<NavLink> GetSidebarLinks()
        {
            var urlHelper = _urlHelperAccessor.GetHelper();

            return new[]
            {
                new NavLink("Spaces", urlHelper.Action<SpaceController>(x => nameof(x.List))),
            };
        }
    }
}
