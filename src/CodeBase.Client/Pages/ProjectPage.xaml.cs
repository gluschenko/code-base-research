using System.Windows;
using System.Windows.Controls;
using CodeBase.Domain;
using CodeBase.Domain.Services;

namespace CodeBase.Client.Pages
{
    [PageDescriptor(PageLifetime.Transient)]
    public partial class ProjectPage : Page
    {
        private readonly Context _context;

        public ProjectPage(Context context)
        {
            _context = context;

            InitializeComponent();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            _context.Navigate(typeof(ProjectEditPage));
        }

        private void FilesButton_Click(object sender, RoutedEventArgs e)
        {
            _context.Navigate(typeof(ProjectFilesPage));
        }
    }
}
