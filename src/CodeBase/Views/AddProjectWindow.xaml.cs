using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CodeBase
{
    public partial class AddProjectWindow : Window
    {
        private readonly Action<Project> _onCreate;

        public AddProjectWindow(Action<Project> onCreate)
        {
            InitializeComponent();

            Width *= 2;

            _onCreate = onCreate;
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
                    _onCreate?.Invoke(new Project(ProjectPath.Text, ProjectName.Text));
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
