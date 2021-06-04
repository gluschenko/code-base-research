using System.Windows;
using System.Windows.Controls;

namespace CodeBase.Client.Controls
{
    public partial class PageHeader : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty
            .Register(nameof(Title), typeof(string), typeof(PageHeader), new PropertyMetadata((d, e) =>
            {
                var item = d as PageHeader;
                var isVisible = !string.IsNullOrWhiteSpace(e.NewValue?.ToString());
                item.TitleVisibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            }));

        public string Title
        {
            get => GetValue(TitleProperty)?.ToString();
            set => SetValue(TitleProperty, value);
        }

        public Visibility TitleVisibility { get; set; } = Visibility.Visible;

        public PageHeader()
        {
            InitializeComponent();
        }
    }
}
