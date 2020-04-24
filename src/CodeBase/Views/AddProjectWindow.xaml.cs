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
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace CodeBase
{
    public partial class AddProjectWindow : Window
    {
        private Action<Project> onCreate;

        public AddProjectWindow(Action<Project> onCreate)
        {
            InitializeComponent();

            Width *= 2;

            this.onCreate = onCreate;
        }

        private void PathTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditProjectWindow.OpenFolderDialog((TextBox)sender);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectPath.Text != "" && ProjectName.Text != "")
            {
                if (System.IO.Directory.Exists(ProjectPath.Text))
                {
                    onCreate?.Invoke(new Project(ProjectPath.Text, ProjectName.Text));
                }
                else
                {
                    MessageBox.Show($"Directory does not exist: {ProjectPath.Text}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Fill all fields", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
