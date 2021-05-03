using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CodeBase.Domain;
using CodeBase.Domain.Models;
using CodeBase.Domain.Services;

namespace CodeBase.Client.Pages
{
    [PageDescriptor("Projects", 1)]
    public partial class MainPage : Page
    {
        private ObservableCollection<Project> _projects;
        private readonly Context _context;

        public MainPage(Context context)
        {
            _context = context;

            InitializeComponent();
            UpdateProjectsList();
        }

        private void UpdateProjectsList()
        {
            _projects ??= new ObservableCollection<Project>(_context.AppData.Projects);

            var list = _projects.OrderBy(proj => -proj.LastEdit).ToArray();

            _projects.Clear();
            foreach (var item in list)
            {
                _projects.Add(item);
            }

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
        }

        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ProjectOpenButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ProjectDeleteButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ProjectEditButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
        }

        #endregion
    }
}
