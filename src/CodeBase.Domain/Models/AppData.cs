using System.Windows;

namespace CodeBase.Domain.Models
{
    public class AppData
    {
        public double? WindowWidth { get; set; }
        public double? WindowHeight { get; set; }
        public WindowState? WindowState { get; set; }
        public bool AutoUpdate { get; set; } = true;
        public bool SendData { get; set; } = false;
        public int UpdateInterval { get; set; } = 60;
        public string ReceiverURL { get; set; } = "";
        public string UserID { get; set; } = "";
        public string Token { get; set; } = "";
        //public IEnumerable<> Projects = new Project[0];
    }
}
