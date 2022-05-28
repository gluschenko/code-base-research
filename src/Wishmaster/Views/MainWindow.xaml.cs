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

        public MainWindow(IAppDataProvider appDataProvider, IServiceProvider serviceProvider)
        {
            InitializeComponent();

            _appDataProvider = appDataProvider;
            _serviceProvider = serviceProvider;

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _appData = _appDataProvider.GetAppData();
            }
            catch (Exception ex)
            {
                MessageHelper.Error(ex.Message);
                Close();
            }

            Navigate<MainPage>();
            ToggleLoading(false);
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
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

            switch (tag)
            {
                case "main":
                    Navigate<MainPage>();
                    break;
                case "settings":
                    Navigate<SettingsPage>();
                    break;
                default:
                    MessageHelper.Error("No page found");
                    break;
            }
        }

        private void Navigate<T>() where T : Page
        {
            using var scope = _serviceProvider.CreateScope();

            var page = scope.ServiceProvider.GetRequiredService<T>();
            PageFrame.Navigate(page);
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

            Theme.Apply(themeType, BackgroundType.Mica, false, false);
        }
    }
}
