using System.Windows;
using CodeBase.Domain.Models;
using CodeBase.Domain.Services;

namespace CodeBase.Client
{
    public partial class MainWindow : Window
    {
        public ConfigLoader<AppData> ConfigLoader { get; set; }
        public AppData AppData { get; set; }
        public Context Context { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            ConfigLoader = new ConfigLoader<AppData>("prefs.json");

            try
            {
                AppData = ConfigLoader.Load();
            }
            catch (Exception ex) 
            {
                MessageBox.Show(typeof(ex).Name, ex.Message);
                Close();
            }

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Width = AppData.Width;
            Height = AppData.Height;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AppData.Width = Width;
            AppData.Height = Height;

            try
            {
                ConfigLoader.Save(AppData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(typeof(ex).Name, ex.Message);
            }
        }
    }
}
