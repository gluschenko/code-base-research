using System;
using System.Windows;
using Wishmaster.Models;
using Wishmaster.Services;

namespace Wishmaster.Views
{
    public partial class MainWindow : Window
    {
        private readonly IAppDataProvider _appDataProvider;
        private AppData? _appData;

        public MainWindow(IAppDataProvider appDataProvider)
        {
            InitializeComponent();

            _appDataProvider = appDataProvider;

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void Start()
        {
            _appData = _appDataProvider.GetAppData();

            Width = _appData.WindowWidth ?? ActualWidth;
            Height = _appData.WindowHeight ?? ActualHeight;
            WindowState = _appData.WindowState ?? WindowState;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
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
                }

                _appDataProvider.SaveAppData(_appData!);
            }
            catch (Exception ex)
            {
                MessageHelper.Error(ex.Message);
            }
        }
    }
}
