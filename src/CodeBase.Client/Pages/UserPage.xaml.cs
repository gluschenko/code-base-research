using System.Windows;
using System.Windows.Controls;
using CodeBase.Domain;
using CodeBase.Domain.Services;

namespace CodeBase.Client.Pages
{
    [PageDescriptor("User", 3, PageLifetime.Scoped)]
    public partial class UserPage : Page
    {
        private readonly Context _context;

        public UserPage(Context context)
        {
            _context = context;

            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
