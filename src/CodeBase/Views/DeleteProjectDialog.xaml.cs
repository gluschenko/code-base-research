using System;
using System.Windows;

namespace CodeBase
{
    /// <summary>
    /// Логика взаимодействия для DeleteProjectDialog.xaml
    /// </summary>
    public partial class DeleteProjectDialog : Window
    {
        private Project Project;
        private Action onDelete;

        public DeleteProjectDialog(Project Project, Action onDelete)
        {
            InitializeComponent();

            Title = Title.Replace("{Title}", Project.Title);

            this.Project = Project;
            this.onDelete = onDelete;
        }

        private void YesButtonClick(object sender, RoutedEventArgs e)
        {
            onDelete?.Invoke();
            Close();
        }

        private void NoButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
