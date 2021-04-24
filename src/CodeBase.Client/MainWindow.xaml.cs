using System;
using System.Windows;
using CodeBase.Client.Pages;
using CodeBase.Domain.Models;
using CodeBase.Domain.Services;

namespace CodeBase.Client
{
    public partial class MainWindow : Window
    {
        public DataManager<AppData> DataManager { get; set; }
        public AppData AppData { get; set; }
        public Context Context { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            DataManager = new DataManager<AppData>("prefs.json");

            try
            {
                AppData = DataManager.Load();
            }
            catch (Exception ex) 
            {
                MessageHelper.Error(ex.Message);
                Close();
            }

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;

            PageFrame.Navigate(Activator.CreateInstance<MainPage>());
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Width = AppData.WindowWidth ?? Width;
            Height = AppData.WindowHeight ?? Height;
            WindowState = AppData.WindowState ?? WindowState;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AppData.WindowWidth = Width;
            AppData.WindowHeight = Height;
            AppData.WindowState = WindowState;

            try
            {
                DataManager.Save(AppData);
            }
            catch (Exception ex)
            {
                MessageHelper.Error(ex.Message);
            }
        }
    }
}
