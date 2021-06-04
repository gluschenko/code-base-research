using System.Linq;
using System.Windows;
using CodeBase.Domain.Services;

namespace CodeBase.Client.Views
{
    public partial class ProjectDeleteWindow : Window
    {
        private readonly Context _context;

        public ProjectDeleteWindow(Context context)
        {
            _context = context;
            Owner = _context.MainWindow;

            InitializeComponent();

            Title = Title.Replace("{Title}", _context.CurrentProject?.Title ?? "??");
        }

        private void YesButtonClick(object sender, RoutedEventArgs e)
        {
            _context.AppData.Projects = _context.AppData.Projects.Where(x => x != _context.CurrentProject).ToArray();
            _context.OnProjectDeleted(_context.CurrentProject);
            Close();
        }

        private void NoButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
