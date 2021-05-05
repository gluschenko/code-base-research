using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CodeBase.Client.Controls
{
    [ContentProperty(nameof(Children))]
    public partial class PageWrapper : UserControl
    {
        public static readonly DependencyProperty TitlePropery = 
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(PageWrapper), new PropertyMetadata(OnTitleChanged));

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = d as PageWrapper;
            item.TitleVisibility = !string.IsNullOrWhiteSpace(e.NewValue?.ToString()) ? Visibility.Visible : Visibility.Collapsed;
        }

        public string Title 
        {
            get => GetValue(TitlePropery)?.ToString();
            set => SetValue(TitlePropery, value); 
        }

        public Visibility TitleVisibility { get; set; } = Visibility.Visible;

        public UIElementCollection Children { get; set; }

        public PageWrapper()
        {
            InitializeComponent();

            Children = new UIElementCollection(this, this);
            Loaded += PageWrapper_Loaded;
        }

        private void PageWrapper_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Container.Children.Clear();

            foreach (var item in Children.Cast<UIElement>().ToList())
            {
                Children.Remove(item);
                Container.Children.Add(item);
            }
        }
    }
}
