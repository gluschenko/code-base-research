using System.Collections.Generic;
using System.Windows;
using WPFUI.Appearance;

namespace Wishmaster.Models
{
    public class AppData
    {
        public int? WindowWidth { get; set; }
        public int? WindowHeight { get; set; }
        public WindowState? WindowState { get; set; }
        public ThemeType? WindowThemeType { get; set; }

        public string AccessToken { get; set; } = "";
        public List<ScopeDataItem> Scopes { get; set; } = new List<ScopeDataItem>();
    }

    public class ScopeDataItem
    {
        public long ID { get; set; }
        public string AccessKey { get; set; }

        public ScopeDataItem(long id, string accessKey)
        {
            ID = id;
            AccessKey = accessKey;
        }
    }
}
