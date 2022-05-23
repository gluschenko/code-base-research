using System;
using System.Linq;
using System.Windows;
using CodeBase.Domain.Services;
using Wishmaster.Models;
using Wishmaster.Views.Pages;
using Context = Wishmaster.Models.Context;

namespace Wishmaster.Views
{
    public partial class MainWindow : Window
    {
        private readonly Context _context;
        private readonly AppData _appData;
        private readonly DataManager<AppData> _dataManager;

        public MainWindow()
        {
            InitializeComponent();

            _dataManager = new DataManager<AppData>("prefs.json");

            try
            {
                _appData = _dataManager.Load();
            }
            catch (Exception ex)
            {
                MessageHelper.Error(ex.Message);
                Close();
            }

            _context = new Context
            {
                AppData = _appData,
                DataManager = _dataManager,
                Navigate = Navigate,
                ToggleLoading = ToggleLoading,
            };

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Navigate(typeof(MainPage));
            ToggleLoading(false);
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                _dataManager.Save(_appData);
            }
            catch (Exception ex)
            {
                MessageHelper.Error(ex.Message);
            }
        }

        private void Navigate(Type pageType)
        {
            var page = Activator.CreateInstance(pageType, _context);
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
    }
}
