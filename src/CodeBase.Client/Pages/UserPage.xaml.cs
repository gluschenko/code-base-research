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

            IsVisibleChanged += UserPage_IsVisibleChanged;

            InitializeComponent();
        }

        private void UserPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // TODO: ЗДЕСЬ ЗАПРОСЫ К АПИ
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
