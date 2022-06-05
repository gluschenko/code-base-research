using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wishmaster.Services;

namespace Wishmaster.Views.Pages
{
    public partial class SpaceListPage : Page
    {
        private readonly ISpaceService _spaceService;

        public SpaceListPage(ISpaceService spaceService)
        {
            InitializeComponent();

            _spaceService = spaceService;
        }

        private void SpaceItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var uid = GetUidFromTag(sender);
            ;
        }

        private void SpaceEdit_Click(object sender, RoutedEventArgs e)
        {
            var uid = GetUidFromTag(sender);

        }

        private void SpaceDelete_Click(object sender, RoutedEventArgs e)
        {
            var uid = GetUidFromTag(sender);

        }

        private static Guid GetUidFromTag(object sender)
        {
            if (sender is FrameworkElement item)
            {
                return Guid.Parse(item.Tag?.ToString() ?? "");
            }

            throw new Exception();
        }
    }
}
