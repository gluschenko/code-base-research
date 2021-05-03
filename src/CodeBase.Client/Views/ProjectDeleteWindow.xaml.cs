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
        }

        private void YesButtonClick(object sender, RoutedEventArgs e)
        {
        }

        private void NoButtonClick(object sender, RoutedEventArgs e)
        {
        }
    }
}
