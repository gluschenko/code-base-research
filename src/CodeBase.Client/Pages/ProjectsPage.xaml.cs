using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CodeBase.Client.Views;
using CodeBase.Domain;
using CodeBase.Domain.Models;
using CodeBase.Domain.Services;

namespace CodeBase.Client.Pages
{
    [PageDescriptor("Projects", 1, PageLifetime.Scoped)]
    public partial class ProjectsPage : Page
    {
        private readonly Context _context;
        private readonly AutorunService _autorunService;

        private ObservableCollection<Project> _projects;
        private ProjectCreateWindow _addProjectWindow;

        public ProjectsPage(Context context)
        {
            _context = context;
            _context.OnProjectCreated = OnProjectCreated;
            _context.OnProjectDeleted = OnProjectDeleted;
            _context.OnProjectChanged = OnProjectChanged;

            _autorunService = new AutorunService();

            InitializeComponent();
            UpdateProjectsList();

            AutorunCheckBox.IsChecked = _autorunService.GetState();
        }

        private void OnProjectCreated(Project project)
        {
            UpdateProjectsList();
            _context.DataManager.Save(_context.AppData);
        }

        private void OnProjectDeleted(Project project)
        {
            UpdateProjectsList();
            _context.DataManager.Save(_context.AppData);
        }

        private void OnProjectChanged(Project project)
        {
            UpdateProjectsList();
            _context.DataManager.Save(_context.AppData);
        }

        private void UpdateProjectsList()
        {
            _projects ??= new ObservableCollection<Project>();

            _projects.Clear();
            _context.AppData.Projects
                .OrderByDescending(proj => proj.LastEdit).ToList()
                .ForEach(x => _projects.Add(x));

            if (ProjectsListBox.ItemsSource == null)
            {
                ProjectsListBox.Items.Clear();
                ProjectsListBox.ItemsSource = _projects;
            }

            ProjectsListBox.Items.Refresh();
            ProjectsListBox.UpdateLayout();
        }

        #region Window Events

        private void AutorunCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            checkBox.IsChecked = _autorunService
                .SetState(checkBox.IsChecked.Value, ex => 
                {
                    MessageHelper.Error(ex.Message, ex.GetType().Name);
                });
        }

        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_addProjectWindow != null)
            {
                _addProjectWindow.Close();
                _addProjectWindow = null;
            }

            _addProjectWindow = new ProjectCreateWindow(_context);

            _addProjectWindow.Show();
        }

        private void ProjectOpenButton_Click(object sender, RoutedEventArgs e)
        {
            var projectId = (Guid)(sender as Button).Tag;
            var project = _context.AppData.Projects.FirstOrDefault(x => x.ID == projectId);

            if (project is null)
            {
                MessageHelper.Error("Project is already been deleted");
                return;
            }

            _context.CurrentProject = project;
            _context.Navigate(typeof(ProjectPage));
        }

        private void ProjectDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var projectId = (Guid)(sender as Button).Tag;
            var project = _context.AppData.Projects.FirstOrDefault(x => x.ID == projectId);

            if (project is null)
            {
                MessageHelper.Error("Project is already been deleted");
                return;
            }

            _context.CurrentProject = project;
            var win = new ProjectDeleteWindow(_context);
            win.Show();
        }

        private void ProjectEditButton_Click(object sender, RoutedEventArgs e)
        {
            var projectId = (Guid)(sender as Button).Tag;
            var project = _context.AppData.Projects.FirstOrDefault(x => x.ID == projectId);

            if (project is null)
            {
                MessageHelper.Error("Project is already been deleted");
                return;
            }

            _context.CurrentProject = project;
            _context.Navigate(typeof(ProjectEditPage));
        }

        #endregion
    }
}
