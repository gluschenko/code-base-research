using System.Windows.Controls;
using CodeBase.Domain;
using CodeBase.Domain.Services;

namespace CodeBase.Client.Pages
{
    [PageDescriptor(PageLifetime.Transient)]
    public partial class ProjectFilesPage : Page
    {
        private readonly Context _context;

        public ProjectFilesPage(Context context)
        {
            InitializeComponent();

            _context = context;
        }
    }
}
