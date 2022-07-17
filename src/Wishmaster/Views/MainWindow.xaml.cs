using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Wishmaster.Models;
using Wishmaster.Services;
using Wishmaster.Views.Pages;
using WPFUI.Appearance;

namespace Wishmaster.Views
{
    public partial class MainWindow : Window
    {
        private readonly IAppDataProvider _appDataProvider;
        private readonly IServiceProvider _serviceProvider;

        private AppData? _appData;
        private IServiceScope? _serviceScope;

        public MainWindow(IAppDataProvider appDataProvider, IServiceProvider serviceProvider)
        {
            InitializeComponent();

            _appDataProvider = appDataProvider;
            _serviceProvider = serviceProvider;

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void Start()
        {
            _appData = _appDataProvider.GetAppData();

            Width = _appData.WindowWidth ?? ActualWidth;
            Height = _appData.WindowHeight ?? ActualHeight;
            WindowState = _appData.WindowState ?? WindowState;
            ToggleTheme(_appData.WindowThemeType ?? ThemeType.Dark);

            var activeItem = (WPFUI.Controls.NavigationItem)RootNavigation.Items.First();
            UpdateNavigator(activeItem);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Navigate<MainPage>();
                ToggleLoading(false);

                Start();
            }
            catch (Exception ex)
            {
                MessageHelper.Error(ex.Message);
                Close();
            }
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (_appData is not null)
                {
                    if (WindowState == WindowState.Normal)
                    {
                        _appData.WindowWidth = (int)ActualWidth;
                        _appData.WindowHeight = (int)ActualHeight;
                        _appData.WindowState = WindowState;
                    }
                    else
                    {
                        _appData.WindowWidth = null;
                        _appData.WindowHeight = null;
                        _appData.WindowState = null;
                    }

                    _appData.WindowThemeType = Theme.GetAppTheme();
                }

                _appDataProvider.SaveAppData(_appData!);
            }
            catch (Exception ex)
            {
                MessageHelper.Error(ex.Message);
            }
        }

        private void OnNavigationClick(object sender, RoutedEventArgs e)
        {
            var item = sender as WPFUI.Controls.NavigationItem;
            UpdateNavigator(item!);

            var tag = item?.Tag?.ToString() ?? "";

            _ = tag switch
            {
                "main" => Navigate<MainPage>(),
                "spaces" => Navigate<SpaceListPage>(),
                "settings" => Navigate<SettingsPage>(),
                "browser" => Navigate<BrowserPage>(),
                "solution-viewer" => Navigate<SolutionViewerPage>(),
                _ => NotFound(tag),
            };

            static bool NotFound(string tag)
            {
                MessageHelper.Error($"No page found: {tag}");
                return false;
            }
        }

        private bool Navigate<T>() where T : Page
        {
            try
            {
                _serviceScope?.Dispose();
                _serviceScope = null;
                _serviceScope = _serviceProvider.CreateScope();

                var page = _serviceScope.ServiceProvider.GetRequiredService<T>();
                return PageFrame.Navigate(page);
            }
            catch (Exception ex)
            {
                MessageHelper.Error(ex.Message);
                return false;
            }
        }

        private void UpdateNavigator(WPFUI.Controls.NavigationItem item)
        {
            foreach (var x in RootNavigation.Items.Union(RootNavigation.Footer))
            {
                x.IsActive = false;
            }

            item.IsActive = true;
        }

        private void ToggleLoading(bool isActive)
        {
            Dispatcher.Invoke(() =>
            {
                LoadingOverlay.Visibility = isActive ? Visibility.Visible : Visibility.Collapsed;
                PageFrame.Effect = isActive
                    ? new System.Windows.Media.Effects.BlurEffect()
                    {
                        KernelType = System.Windows.Media.Effects.KernelType.Gaussian,
                        Radius = 10,
                    }
                    : null;
            });
        }

        private void OnToggleThemeClick(object sender, RoutedEventArgs e)
        {
            var themeType = Theme.GetAppTheme() == ThemeType.Dark
                ? ThemeType.Light
                : ThemeType.Dark;

            ToggleTheme(themeType);
        }

        private static void ToggleTheme(ThemeType type)
        {
            // SystemParameters.WindowGlassColor
            var mainColor = System.Windows.Media.Color.FromRgb(0xF9, 0x62, 0x09);
            var color1 = System.Windows.Media.Color.FromRgb(0xF9, 0x62, 0x09);
            var color2 = System.Windows.Media.Color.FromRgb(0xF4, 0x73, 0x0B);
            var color3 = System.Windows.Media.Color.FromRgb(0xF6, 0x8A, 0x06);

            Accent.Apply(mainColor, color1, color2, color3);
            Theme.Apply(type, BackgroundType.Mica, false, false);
        }
    }
}
