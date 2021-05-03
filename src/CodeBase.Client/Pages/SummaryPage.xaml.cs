using System.Windows.Controls;
using CodeBase.Domain;
using CodeBase.Domain.Services;

namespace CodeBase.Client.Pages
{
    [PageDescriptor("Summary", 2)]
    public partial class SummaryPage : Page
    {
        private readonly Context _context;

        public SummaryPage(Context context)
        {
            _context = context;

            InitializeComponent();
        }
    }
}
