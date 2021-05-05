using System.Diagnostics;
using System.Linq;
using System.Windows;
using CodeBase.Domain.Services;

namespace CodeBase.Client
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (!IsRunningOnce())
            {
                var currentProcess = Process.GetCurrentProcess();
                MessageHelper.Warning($"{currentProcess.ProcessName} is already running! Close all instances and try again");
                Shutdown();
            }

            base.OnStartup(e);
        }

        private static bool IsRunningOnce()
        {
            var processList = Process.GetProcesses();
            var current = Process.GetCurrentProcess();

            return !processList.Any(x => x.ProcessName == current.ProcessName && x.Id != current.Id);
        }
    }
}
