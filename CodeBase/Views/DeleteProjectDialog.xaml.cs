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
