using System.Windows;
using System.Windows.Input;
using CodeBase.Domain.Services;

namespace CodeBase.Client.Views
{
    public partial class AddProjectWindow : Window
    {
        private readonly Context _context;

        public AddProjectWindow(Context context)
        {
            InitializeComponent();

            _context = context;
        }

        private void PathTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
