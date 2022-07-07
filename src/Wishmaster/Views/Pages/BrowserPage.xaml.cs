using System;
using System.Windows.Controls;

namespace Wishmaster.Views.Pages
{
    public partial class BrowserPage : Page
    {
        public BrowserPage()
        {
            InitializeComponent();
            Loaded += BrowserPage_Loaded;
        }

        private void BrowserPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            WebView.Source = new Uri(AddressTextBox.Text);
        }

        private void GoButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                var url = AddressTextBox.Text;

                if (string.IsNullOrWhiteSpace(url))
                {
                    return;
                }

                WebView.Source = new Uri(AddressTextBox.Text);
            }
            catch
            {
                return;
            }
        }

        private void WebView_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            LoadingOverlay.Visibility = System.Windows.Visibility.Visible;
        }

        private void WebView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            LoadingOverlay.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
