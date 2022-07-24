using System;
using System.Collections.Generic;
using System.Text;
using TypeGen.Core.TypeAnnotations;

namespace Wishmaster.ViewModels
{
    [ExportTsInterface]
    public class AppDataViewModel
    {
        public IEnumerable<NavLinkViewModel>? SidebarNavigation { get; set; }
    }

    [ExportTsInterface]
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
