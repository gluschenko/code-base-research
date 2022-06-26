using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CodeBase.Core;
using Microsoft.WindowsAPICodePack.Dialogs;


namespace CodeBase
{
    public partial class EditProjectWindow : Window
    {
        public Project Project { get; set; }
        public Action OnEdit { get; set; }

        public EditProjectWindow(Project project, Action onEdit)
        {
            InitializeComponent();

            Title = Title.Replace("{Title}", project.Title);
            Width *= 2;

            OnEdit = onEdit;
            Project = project;
            //
            ProjectName.Text = project.Title;
            ProjectColor.Text = project.Color;
            ProjectPath.Text = project.Path;
            ProjectFolders.Text = string.Join(", ", project.Folders ?? new List<string>());
            IgnoredProjectFolders.Text = string.Join(", ", project.IgnoredFolders ?? new List<string>());
            ProjectIsPublic.IsChecked = project.IsPublic;
            ProjectIsLocal.IsChecked = project.IsLocal;
            ProjectIsNameHidden.IsChecked = project.IsNameHidden;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            static List<string> ParseFolders(string text)
            {
                var textInner = text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var foldersInner = new List<string>();
                foreach (var folder in textInner)
                {
                    if (folder.Trim() != "")
                    {
                        foldersInner.Add(folder.Trim());
                    }
                }
                return foldersInner;
            }

            if (ProjectPath.Text != "" && ProjectName.Text != "")
            {
                if (System.IO.Directory.Exists(ProjectPath.Text))
                {
                    Project.Title = ProjectName.Text.Trim();
                    Project.Color = ProjectColor.Text.Trim();
                    Project.Path = ProjectPath.Text.Trim();
                    //
                    Project.Folders = ParseFolders(ProjectFolders.Text);
                    Project.IgnoredFolders = ParseFolders(IgnoredProjectFolders.Text);
                    //
                    Project.IsPublic = ProjectIsPublic.IsChecked.Value;
                    Project.IsLocal = ProjectIsLocal.IsChecked.Value;
                    Project.IsNameHidden = ProjectIsNameHidden.IsChecked.Value;
                    //
                    OnEdit?.Invoke();
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
