using System.Windows;
using System.Windows.Controls;

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
            WebViewElement.CoreWebView2.Navigate("http://localhost:5080/index.html");
        }
    }
}
