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

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void SourceFilesTab_Selected(object sender, RoutedEventArgs e)
        {
        }
    }
}
