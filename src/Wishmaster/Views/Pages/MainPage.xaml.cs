using System.Windows.Controls;
using Wishmaster.Models;

namespace Wishmaster.Views.Pages
{
    public partial class MainPage : Page
    {
        private readonly Context _context;

        public MainPage(Context context)
        {
            InitializeComponent();

            _context = context;
        }
    }
}
