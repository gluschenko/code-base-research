using System;
using System.Collections.Generic;
using System.Windows;

namespace CodeBase.Domain.Models
{
    public class AppData
    {
        public double? WindowWidth { get; set; }
        public double? WindowHeight { get; set; }
        public WindowState? WindowState { get; set; }
        public bool IsAutoUpdateEnabled { get; set; } = true;
        public int AutoUpdateInterval { get; set; } = 60;
        public bool IsSendDataEnabled { get; set; } = false;
        public bool IsTrayCollapseEnabled { get; set; } = true;
        public string UserID { get; set; } = "";
        public string UserToken { get; set; } = "";
        public IEnumerable<Project> Projects { get; set; } = Array.Empty<Project>();
    }
}
