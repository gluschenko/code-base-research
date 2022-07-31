using System.Collections.Generic;

namespace Wishmaster.ViewModels
{
    public class AppDataViewModel
    {
        public IEnumerable<NavLinkViewModel>? SidebarNavigation { get; set; }
        public NotViewModels.Test? Item { get; set; }
    }

    public class NavLinkViewModel
    {
        public string Text { get; set; }
        public string Url { get; set; }

        public NavLinkViewModel(string text, string url)
        {
            Text = text;
            Url = url;
        }
    }
}

namespace Wishmaster.NotViewModels
{
    public class Test
    {
        public string? Text { get; set; }
    }
}
