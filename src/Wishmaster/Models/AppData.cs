using System.Collections.Generic;

namespace Wishmaster.Models
{
    public class AppData
    {
        public string AccessToken { get; set; }
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
