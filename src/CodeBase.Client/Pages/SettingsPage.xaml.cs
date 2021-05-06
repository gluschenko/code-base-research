using System.Windows.Controls;
using CodeBase.Domain;
using CodeBase.Domain.Services;

namespace CodeBase.Client.Pages
{
    [PageDescriptor("Settings", 4, PageLifetime.Scoped)]
    public partial class SettingsPage : Page
    {
        private readonly Context _context;

        public SettingsPage(Context context)
        {
            InitializeComponent();

            _context = context;
        }
    }
}
