using System.Collections.Generic;
using Wishmaster.Controllers;
using Wishmaster.Models.Navigation;
using Wishmaster.Mvc;

namespace Wishmaster.Services
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
