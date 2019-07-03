using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CodeBase
{
    public partial class MainWindow : Window
    {
        public Heart Heart;

        public DataManager<ApplicationData> DataManager = new DataManager<ApplicationData>("ApplicationData.json");
        public ApplicationData Data;

        private ObservableCollection<Project> Projects;
        private readonly Autorun Autorun = new Autorun();
        private readonly Inspector Inspector = new Inspector();

        private AddProjectWindow AddProjectWindow;
        private ServerAccessWindow ServerAccessWindow;

        private System.Windows.Forms.NotifyIcon NotifyIcon;

        public MainWindow()
        {
            InitializeComponent();
            //
            Load();
            //
            Projects = new ObservableCollection<Project>(Data.Projects);
            //
            listBox.Items.Clear();
            listBox.ItemsSource = Projects;
            listBox.SelectionChanged += ListBox_SelectionChanged;
            WakeUp();
            //
            AutorunCheckBox.IsChecked = Autorun.GetState();
            //
            Heart = new Heart(Data, () => StartInspector());
            //

            // Поведение иконки в tray
            NotifyIcon = new System.Windows.Forms.NotifyIcon();
            var Hicon = Properties.Resources.CodeBaseLogo.GetHicon();
            NotifyIcon.Icon = System.Drawing.Icon.FromHandle(Hicon);
            NotifyIcon.MouseClick += (sender, e) => {
                WindowState = WindowState.Normal;
                Activate(); // brings this window to forward
            };

            // Поведение окна
            KeyDown += (sender, e) => { if (e.Key == Key.Tab) SendData(); };

            Closing += (sender, e) => {
                Hide();
                ShowInTaskbar = false;
                NotifyIcon.Visible = true;
                e.Cancel = true;
            };

            StateChanged += (sender, e) => {
                if (WindowState == WindowState.Minimized)
                {
                    NotifyIcon.Visible = true;
                    ShowInTaskbar = false;
                }
                else
                if (WindowState == WindowState.Normal)
                {
                    NotifyIcon.Visible = false;
                    ShowInTaskbar = true;
                }
            };

        }

        public void Load()
        {
            Data = DataManager.Load(ex => {
                Error(ex.ToString(), ex.GetType().Name);
                Close();
            });

            WebMethods.ReceiverURL = Data.ReceiverURL ?? "";
        }

        public void Save()
        {
            Data.ReceiverURL = WebMethods.ReceiverURL;
            Data.Projects = Projects.ToArray();

            DataManager.Save(Data, ex => {
                Error(ex.ToString(), ex.GetType().Name);
            });
        }

        private void WakeUp()
        {
            Projects = new ObservableCollection<Project>(Projects.OrderBy(proj => -proj.LastEdit));
            listBox.ItemsSource = Projects;
            listBox.Items.Refresh();
            listBox.UpdateLayout();
        }

        //

        private void AutorunCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ch = (CheckBox)sender;
            ch.IsChecked = Autorun.SetState(ch.IsChecked.Value, ex => {
                Error(ex.ToString(), ex.GetType().Name);
            });
        }

        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddProjectWindow != null)
            {
                AddProjectWindow.Close();
                AddProjectWindow = null;
            }

            AddProjectWindow = new AddProjectWindow((project) =>
            {
                Projects.Insert(0, project);
                AddProjectWindow.Close();
                //
                WakeUp();
                //
                Save();
            })
            {
                Owner = this
            };

            AddProjectWindow.Show();
        }

        private void ServerAccessButton_Click(object sender, RoutedEventArgs e)
        {
            if (ServerAccessWindow != null)
            {
                ServerAccessWindow.Close();
                ServerAccessWindow = null;
            }

            ServerAccessWindow = new ServerAccessWindow(Data, () => Save()) { Owner = this };
            ServerAccessWindow.Show();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Project P = (Project)listBox.SelectedItem;
            ProjectWindow win = new ProjectWindow(P) { Owner = this };
            win.Show();
        }

        private void ProjectDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            string title = (string)((Button)sender).Tag;

            foreach (var proj in Projects)
            {
                if (proj.Title == title)
                {
                    DeleteProjectDialog dialog = new DeleteProjectDialog(proj, () => {
                        Projects.Remove(proj);
                        //
                        WakeUp();
                        //
                        Save();
                    });
                    dialog.Show();
                }
            }
        }

        private void ProjectEditButton_Click(object sender, RoutedEventArgs e)
        {
            string title = (string)((Button)sender).Tag;

            foreach (var proj in Projects)
            {
                if (proj.Title == title)
                {
                    EditProjectWindow win = new EditProjectWindow(proj, () => {
                        WakeUp();
                        //
                        Save();
                    });
                    win.Show();
                }
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            StartInspector();
        }

        public void StartInspector()
        {
            Inspector.Start(new List<Project>(Projects), (stage, info) => {
                Dispatcher.Invoke(() => {
                    switch (stage)
                    {
                        case InspectorStage.Progress:
                            ProgressBar.Visibility = Visibility.Visible;
                            (int progress, int total) = ((int, int))info;
                            ProgressBar.Maximum = total;
                            ProgressBar.Value = progress;
                            break;
                        case InspectorStage.Progress2:
                            ProgressBar2.Visibility = Visibility.Visible;
                            (progress, total) = ((int, int))info;
                            ProgressBar2.Maximum = total;
                            ProgressBar2.Value = progress;
                            break;
                        case InspectorStage.FetchingFiles:
                            StatusText.Content = $"Fetching files: {info}";
                            break;
                        case InspectorStage.FetchingLines:
                            StatusText.Content = $"Fetching lines: {info}";
                            break;
                        case InspectorStage.Completed:
                            StatusText.Content = "Complete";
                            WakeUp();
                            Save();
                            SendData();
                            //
                            ProgressBar.Visibility = Visibility.Hidden;
                            ProgressBar2.Visibility = Visibility.Hidden;
                            break;
                    }
                });
            });
        }

        public void SendData()
        {
            if (Data.SendData)
            {
                WebMethods.UpdateProjects(Data, Projects.Select(proj => (ProjectEntity)proj).ToArray(), result => 
                {
                    WebClient.ProcessResponse(result, data =>
                    {
                        if(data != "1")
                            MessageBox.Show(data);
                    });
                });
            }
        }

        private void SummaryButton_Click(object sender, RoutedEventArgs e)
        {
            Project 
                All     = new Project("", "All projects")     { Info = new ProjectInfo() },
                Public  = new Project("", "Public projects")  { Info = new ProjectInfo() },
                Private = new Project("", "Private projects") { Info = new ProjectInfo() };

            foreach (var proj in Projects)
            {
                All.Info.Volume += proj.Info.Volume;
                merge(All.Info.ExtensionsVolume, proj.Info.ExtensionsVolume);

                if (proj.IsPublic)
                {
                    Public.Info.Volume += proj.Info.Volume;
                    merge(Public.Info.ExtensionsVolume, proj.Info.ExtensionsVolume);
                }
                else
                {
                    Private.Info.Volume += proj.Info.Volume;
                    merge(Private.Info.ExtensionsVolume, proj.Info.ExtensionsVolume);
                }

                All.Info.Errors.AddRange(proj.Info.Errors.Select(t => $"{proj.Title} -> {t}"));
                //
                void merge(Dictionary<string, CodeVolume> A, Dictionary<string, CodeVolume> B)
                {
                    foreach (var pair in B)
                    {
                        if (!A.ContainsKey(pair.Key))
                            A.Add(pair.Key, pair.Value);

                        else
                            A[pair.Key] += pair.Value;
                    }
                }

            }

            ProjectWindow win = new ProjectWindow(new Project[] { All, Public, Private })
            {
                Title = "Summary",
                Owner = this
            };
            win.Show();
        }

        //

        public static void Error(string text, string title = "")
        {
            MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
