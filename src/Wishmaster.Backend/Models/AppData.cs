using System.Collections.Generic;
using System.Windows;

namespace Wishmaster.Backend.Models
{
    public class AppData
    {
        public int? WindowWidth { get; set; }
        public int? WindowHeight { get; set; }
        public int? WindowState { get; set; }

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
