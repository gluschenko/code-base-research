using System;
using System.Windows;
using CodeBase.Client.Pages;
using CodeBase.Domain.Models;
using CodeBase.Domain.Services;

namespace CodeBase.Client
{
    public partial class MainWindow : Window
    {
        private readonly DataManager<AppData> _dataManager;
        private readonly AppData _appData;
        private readonly Context _context;

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
            };

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;

            PageFrame.Navigate(new MainPage(_context));
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Width = _appData.WindowWidth ?? Width;
            Height = _appData.WindowHeight ?? Height;
            WindowState = _appData.WindowState ?? WindowState;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _appData.WindowWidth = Width;
            _appData.WindowHeight = Height;
            _appData.WindowState = WindowState;

            try
            {
                _dataManager.Save(_appData);
            }
            catch (Exception ex)
            {
                MessageHelper.Error(ex.Message);
            }
        }
    }
}
