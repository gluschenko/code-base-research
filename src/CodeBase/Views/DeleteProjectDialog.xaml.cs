using System;
using System.Windows;
using CodeBase.Core;

namespace CodeBase
{
    /// <summary>
    /// Логика взаимодействия для DeleteProjectDialog.xaml
    /// </summary>
    public partial class DeleteProjectDialog : Window
    {
        private readonly Action _onDelete;

        public DeleteProjectDialog(Project project, Action onDelete)
        {
            InitializeComponent();

            Title = Title.Replace("{Title}", project.Title);

            _onDelete = onDelete;
        }

        private void YesButtonClick(object sender, RoutedEventArgs e)
        {
            _onDelete?.Invoke();
            Close();
        }

        private void NoButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
