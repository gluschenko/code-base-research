using System;
using System.Collections.Generic;
using System.Text;
using Wishmaster.ViewModels;

namespace Wishmaster.Backend.Models.Navigation
{
    public class NavLink
    {
        public string Text { get; set; }
        public string Url { get; set; }

        public NavLink(string text, string url)
        {
            Text = text;
            Url = url;
        }

        public NavLinkViewModel ToViewModel()
        {
            return new NavLinkViewModel(Text, Url);
        }
    }
}
