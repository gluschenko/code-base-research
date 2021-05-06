using System.Windows;
using System.Windows.Controls;

namespace CodeBase.Client.Controls
{
    public partial class PageHeader : UserControl
    {
        public static readonly DependencyProperty TitlePropery = 
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(PageHeader), new PropertyMetadata(OnTitleChanged));

        public string Title 
        {
            get => GetValue(TitlePropery)?.ToString();
            set => SetValue(TitlePropery, value); 
        }

        public Visibility TitleVisibility { get; set; } = Visibility.Visible;

        public PageHeader()
        {
            InitializeComponent();
        }

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = d as PageHeader;
            item.TitleVisibility = !string.IsNullOrWhiteSpace(e.NewValue?.ToString()) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
