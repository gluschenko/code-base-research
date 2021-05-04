using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CodeBase.Domain;
using CodeBase.Domain.Services;

namespace CodeBase.Client.Pages
{
    [PageDescriptor(PageLifetime.Transient)]
    public partial class ProjectEditPage : Page
    {
        private readonly Context _context;

        public ProjectEditPage(Context context)
        {
            _context = context;

            InitializeComponent();

            IsVisibleChanged += ProjectEditPage_IsVisibleChanged;
        }

        private void ProjectEditPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var project = _context.CurrentProject;

            Title = Title.Replace("{Title}", project.Title);

            ProjectName.Text = project.Title;
            ProjectColor.Text = project.Color;
            ProjectLocation.Text = project.Location;
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
                return text
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .ToList();
            }

            var project = _context.CurrentProject;

            if (string.IsNullOrWhiteSpace(ProjectLocation.Text) || string.IsNullOrWhiteSpace(ProjectName.Text))
            {
                MessageHelper.Error("All fields are should be filled");
                return;
            }

            if (!Directory.Exists(ProjectLocation.Text))
            {
                MessageHelper.Error($"Directory does not exist: {ProjectLocation.Text}");
                return;
            }

            project.Title = ProjectName.Text.Trim();
            project.Color = ProjectColor.Text.Trim();
            project.Location = ProjectLocation.Text.Trim();

            project.Folders = ParseFolders(ProjectFolders.Text);
            project.IgnoredFolders = ParseFolders(IgnoredProjectFolders.Text);

            project.IsPublic = ProjectIsPublic.IsChecked.Value;
            project.IsLocal = ProjectIsLocal.IsChecked.Value;
            project.IsNameHidden = ProjectIsNameHidden.IsChecked.Value;

            _context.OnProjectChanged(project);
            _context.Navigate(typeof(ProjectsPage));
        }

        private void PathTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var textbox = sender as TextBox;
            DialogHelper.OpenFolderDialog(textbox.Text, text => textbox.Text = text);
        }

        private void ProjectColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(ProjectColor.Text);
                ProjectColor.Background = new SolidColorBrush(color);
            }
            catch
            { 
            }
        }
    }
}
