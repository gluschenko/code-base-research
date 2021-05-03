using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CodeBase.Domain.Models;
using CodeBase.Domain.Services;

namespace CodeBase.Client.Views
{
    public partial class ProjectCreateWindow : Window
    {
        private readonly Context _context;

        public ProjectCreateWindow(Context context)
        {
            _context = context;
            Owner = _context.MainWindow;

            InitializeComponent();
        }

        private void PathTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var textbox = sender as TextBox;
            DialogHelper.OpenFolderDialog(textbox.Text, text => textbox.Text = text);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var name = ProjectName.Text;
            var location = ProjectLocation.Text;

            if (string.IsNullOrWhiteSpace(location) || string.IsNullOrWhiteSpace(name))
            {
                MessageHelper.Warning("Fill all the fields");
                return;
            }

            if (!Directory.Exists(location))
            {
                MessageHelper.Error($"Directory does not exist: {location}");
                return;
            }

            var project = Project.Create(location, name);
            _context.AppData.Projects = _context.AppData.Projects.Append(project);
            _context.OnProjectCreated(project);
            Close();
        }
    }
}
