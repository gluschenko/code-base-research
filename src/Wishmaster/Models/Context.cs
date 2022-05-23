using System;
using CodeBase.Domain.Services;

namespace Wishmaster.Models
{
    public class Context
    {
        public AppData AppData { get; set; }
        public DataManager<AppData> DataManager { get; set; }
        public Action<Type> Navigate { get; set; }
        public Action<bool> ToggleLoading { get; set; }
    }
}
