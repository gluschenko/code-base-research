using System.Windows;
using System.Windows.Controls;
using CodeBase.Domain;
using CodeBase.Domain.Services;

namespace CodeBase.Client.Pages
{
    [PageDescriptor(PageLifetime.Transient)]
    public partial class UserLoginPage : Page
    {
        private readonly Context _context;

        public UserLoginPage(Context context)
        {
            _context = context;

            IsVisibleChanged += UserLoginPage_IsVisibleChanged;

            InitializeComponent();
        }

        private void UserLoginPage_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            // TODO: ЗДЕСЬ ЗАПРОСЫ К АПИ
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
