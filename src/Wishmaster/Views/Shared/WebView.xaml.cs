using System;
using System.Collections.Generic;
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

namespace Wishmaster.Views.Shared
{
    public partial class WebView : UserControl
    {
        public WebView()
        {
            InitializeComponent();
            Loaded += WebView_Loaded;
        }

        private async void WebView_Loaded(object sender, RoutedEventArgs e)
        {
            await WebViewElement.EnsureCoreWebView2Async();
        }
    }
}
