using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CodeBase.Domain;
using CodeBase.Domain.Services;

namespace CodeBase.Client.Pages
{
    [PageDescriptor(PageLifetime.Transient)]
    public partial class ProjectEditPage : Page
    {
        private readonly Context _context;

        public ProjectEditPage(Context context)
        {
            _context = context;

            InitializeComponent();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void PathTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }

        private void ProjectColor_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}
