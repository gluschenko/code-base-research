using System.Windows.Media;

namespace CodeBase.Domain.Models
{
    public class FilesListItem
    {
        public static Brush Default => GetBrush("#222");
        public static Brush Green => GetBrush("#0F0");
        public static Brush Red => GetBrush("#F00");

        private static Brush GetBrush(string rgb) 
            => new SolidColorBrush((Color)ColorConverter.ConvertFromString(rgb));

        //

        public string Text { get; set; }
        public Brush Color { get; set; }

        public FilesListItem(string text, Brush color)
        {
            Text = text;
            Color = color;
        }

        public FilesListItem() : this("", Green) { }
    }
}
