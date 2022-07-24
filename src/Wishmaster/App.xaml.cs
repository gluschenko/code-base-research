using System;
using System.Windows;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wishmaster.DataAccess;
using Wishmaster.Mvc;
using Wishmaster.Services;
using Wishmaster.Views;
using Wishmaster.Views.Pages;

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
                    services.AddSingleton<INavigationService, NavigationService>();
                    services.AddDbContext<Db>(options =>
                    {
                        var connectionString = context.Configuration.GetConnectionString("SQLite");
                        options.UseSqlite(connectionString);
                    });

                    services.AddSingleton<IAppDataProvider, AppDataProvider>();
                    services.AddScoped<ISpaceService, SpaceService>();

                    services.AddSingleton<MainWindow>();

                    services.AddScoped<MainPage>();
                    services.AddScoped<SettingsPage>();
                    services.AddScoped<SpaceListPage>();
                    services.AddScoped<BrowserPage>();
                    services.AddScoped<SolutionViewerPage>();

                    services.AddUrlHelperAccessor();
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var envName = Environments.Production;
#if DEBUG
                    envName = Environments.Development;
#endif

                    webBuilder
                        .UseKestrel(x => x.ListenLocalhost(5080))
                        .UseEnvironment(envName)
                        .UseContentRoot(AppContext.BaseDirectory)
                        .UseStartup<WebStartup>();
                })
                .Build();
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                await _host.StartAsync();

                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
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
