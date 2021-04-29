using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Dialogs;


namespace CodeBase
{
    public partial class EditProjectWindow : Window
    {
        public Action onEdit;
        public Project Project;

        public EditProjectWindow(Project Project, Action onEdit)
        {
            InitializeComponent();

            Title = Title.Replace("{Title}", Project.Title);
            Width *= 2;

            this.onEdit = onEdit;
            this.Project = Project;
            //
            ProjectName.Text = Project.Title;
            ProjectColor.Text = Project.Color;
            ProjectPath.Text = Project.Path;
            ProjectFolders.Text = string.Join(", ", Project.Folders ?? new List<string>());
            IgnoredProjectFolders.Text = string.Join(", ", Project.IgnoredFolders ?? new List<string>());
            ProjectIsPublic.IsChecked = Project.IsPublic;
            ProjectIsLocal.IsChecked = Project.IsLocal;
            ProjectIsNameHidden.IsChecked = Project.IsNameHidden;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> parseFolders(string text)
            {
                string[] _text = text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var _folders = new List<string>();
                foreach (string folder in _text)
                {
                    if (folder.Trim() != "")
                    {
                        _folders.Add(folder.Trim());
                    }
                }
                return _folders;
            }
            //
            if (ProjectPath.Text != "" && ProjectName.Text != "")
            {
                if (System.IO.Directory.Exists(ProjectPath.Text))
                {
                    Project.Title = ProjectName.Text.Trim();
                    Project.Color = ProjectColor.Text.Trim();
                    Project.Path = ProjectPath.Text.Trim();
                    //
                    Project.Folders = parseFolders(ProjectFolders.Text);
                    Project.IgnoredFolders = parseFolders(IgnoredProjectFolders.Text);
                    //
                    Project.IsPublic = ProjectIsPublic.IsChecked.Value;
                    Project.IsLocal = ProjectIsLocal.IsChecked.Value;
                    Project.IsNameHidden = ProjectIsNameHidden.IsChecked.Value;
                    //
                    onEdit?.Invoke();
                    Close();
                }
                else
                {
                    MessageBox.Show($"Directory does not exist: {ProjectPath.Text}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("All fields are should be filled", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void PathTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenFolderDialog((TextBox)sender);
        }

        private void ProjectColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ProjectColor.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ProjectColor.Text));
            }
            catch
            { }
        }

        //

        public static void OpenFolderDialog(TextBox textBox)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = textBox.Text
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dialog.FileName;
                textBox.Text = folder;
            }
        }
    }
}
