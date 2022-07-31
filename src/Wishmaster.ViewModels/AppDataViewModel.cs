using System;
using System.Collections.Generic;
using System.Text;

namespace Wishmaster.ViewModels
{
    public class AppDataViewModel
    {
        public IEnumerable<NavLinkViewModel>? SidebarNavigation { get; set; }
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
