using System.Windows;
using System.Windows.Controls;
using CodeBase.Domain;
using CodeBase.Domain.Services;

namespace CodeBase.Client.Pages
{
    [PageDescriptor("Settings", 4, PageLifetime.Scoped)]
    public partial class SettingsPage : Page
    {
        private readonly Context _context;
        private readonly AutorunService _autorunService;

        public SettingsPage(Context context)
        {
            InitializeComponent();

            _context = context;

            _autorunService = new AutorunService();

            AutorunCheckBox.IsChecked = _autorunService.GetState();
            TrayCheckBox.IsChecked = _context.AppData.IsTrayCollapseEnabled;
        }

        private void AutorunCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            checkbox.IsChecked = _autorunService
                .SetState(checkbox.IsChecked.Value, ex => MessageHelper.Error(ex.Message, ex.GetType().Name));
        }

        private void TrayCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            _context.AppData.IsTrayCollapseEnabled = checkbox.IsChecked.Value;
            _context.DataManager.Save(_context.AppData);
        }
    }
}
