using System;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wishmaster.DataAccess;
using Wishmaster.Services;
using Wishmaster.Views;

namespace Wishmaster
{
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = new HostBuilder()
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    configurationBuilder.SetBasePath(context.HostingEnvironment.ContentRootPath);
                    configurationBuilder.AddJsonFile("appsettings.json", optional: false);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<INavigationService, NavigationService>();
                    services.AddDbContext<Db>(options => 
                    {
                        var connectionString = context.Configuration.GetConnectionString("SQLite");
                        options.UseSqlite(connectionString);
                    });
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                })
                .Build();
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }
        }
    }
}
