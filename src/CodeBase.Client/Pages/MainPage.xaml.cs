using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CodeBase.Domain.Models;
using CodeBase.Domain.Services;

namespace CodeBase.Client.Pages
{
    public partial class MainPage : Page
    {
        private ObservableCollection<Project> _projects;
        private readonly Context _context;

        public MainPage(Context context)
        {
            InitializeComponent();

            _context = context;

            UpdateProjectsList();
        }

        private void UpdateProjectsList()
        {
            _projects ??= new ObservableCollection<Project>(_context.AppData.Projects);

            _projects.Clear();

            foreach (var proj in _projects.OrderBy(x => -x.LastEdit))
            {
                _projects.Add(proj);
            }

            if (listBox.ItemsSource == null)
            {
                listBox.Items.Clear();
                listBox.ItemsSource = _projects;
            }
            listBox.Items.Refresh();
            listBox.UpdateLayout();
        }

        #region Window Events

        private void AutorunCheckBox_Click(object sender, RoutedEventArgs e)
        {
        }

        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ServerAccessButton_Click(object sender, RoutedEventArgs e)
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

        private void SummaryButton_Click(object sender, RoutedEventArgs e)
        {
        }

        #endregion
    }
}
