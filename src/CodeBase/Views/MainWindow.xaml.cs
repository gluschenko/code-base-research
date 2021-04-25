using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NotifyIcon = System.Windows.Forms.NotifyIcon;

namespace CodeBase
{
    public partial class MainWindow : Window
    {
        public DataManager<ApplicationData> DataManager { get; set; }
        public ApplicationData Data { get; set; }

        private readonly ObservableCollection<Project> _projects;

        private Heart _heart;
        private readonly Autorun _autorun;
        private readonly Inspector _inspector;

        private AddProjectWindow _addProjectWindow;
        private ServerAccessWindow _serverAccessWindow;

        private NotifyIcon _notifyIcon;

        private bool _isUpdateAssugned = false;

        public MainWindow()
        {
            InitializeComponent();
            CheckRunningOnce();
            //
            DataManager = new DataManager<ApplicationData>("ApplicationData.json");
            //
            Load();
            //
            _heart = new Heart(Data, () => StartInspector());
            _autorun = new Autorun();
            _inspector = new Inspector();
            //
            _projects = new ObservableCollection<Project>(Data.Projects);
            UpdateProjectsList();
            //
            CreateTrayIcon();
            BindWindowEvents();
            Activate();
        }

        ~MainWindow() 
        {
            _notifyIcon.Dispose();
            _heart = null;
        }

        #region Other Methods

        void BindWindowEvents()
        {
            AutorunCheckBox.IsChecked = _autorun.GetState();

            // Поведение окна
            KeyDown += (sender, e) => { if (e.Key == Key.Tab) SendData(); };

            Closing += (sender, e) => {
                Hide();
                ShowInTaskbar = false;
                _notifyIcon.Visible = true;
                e.Cancel = true;
            };

            StateChanged += (sender, e) => {
                if (WindowState == WindowState.Minimized)
                {
                    _notifyIcon.Visible = true;
                    ShowInTaskbar = false;
                }
                else
                if (WindowState == WindowState.Normal)
                {
                    _notifyIcon.Visible = false;
                    ShowInTaskbar = true;
                }
            };
        }

        void CheckRunningOnce() 
        {
            var processes = Process.GetProcesses();
            var currentProcess = Process.GetCurrentProcess();
            foreach (var process in processes)
            {
                if (process.ProcessName == currentProcess.ProcessName)
                {
                    if (process.Id != currentProcess.Id)
                    {
                        MessageHelper.Alert($"{process.ProcessName} is already running! Close all instances and try again", "Error");
                        Close();
                        return;
                    }
                }
            }
        }

        void CreateTrayIcon() 
        {
            // Поведение иконки в tray
            _notifyIcon = new NotifyIcon();
            var icon = Properties.Resources.CodeBaseLogo.GetHicon();
            _notifyIcon.Icon = System.Drawing.Icon.FromHandle(icon);
            _notifyIcon.MouseDoubleClick += (sender, e) =>
            {
                WindowState = WindowState.Normal;
                Activate(); // brings this window to forward
            };
            _notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripButton("Close", null, (s, e) => 
            {
                _notifyIcon.Visible = false;
                Application.Current.Shutdown();
            }));
        }

        #endregion

        #region Load & Save methods

        public void Load()
        {
            Data = DataManager.Load(ex => {
                MessageHelper.ThrowException(ex);
                Close();
            });
            //
            WebMethods.ReceiverURL = Data.ReceiverURL ?? "";
            
            if (Data.WindowWidth > 0)
                Width = Data.WindowWidth;
            if (Data.WindowHeight > 0)
                Height = Data.WindowHeight;

            WindowState = Data.WindowState;
        }

        public void Save()
        {
            Data.ReceiverURL = WebMethods.ReceiverURL;
            Data.Projects = _projects.ToArray();

            Data.WindowWidth = Width;
            Data.WindowHeight = Height;
            Data.WindowState = WindowState;
            //
            DataManager.Save(Data, MessageHelper.ThrowException);
        }

        private void UpdateProjectsList()
        {
            var list = new ObservableCollection<Project>(_projects.OrderBy(proj => -proj.LastEdit));
            _projects.Clear();
            foreach (var proj in list) 
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

        public void StartInspector()
        {
            if (!_isUpdateAssugned) 
            {
                _isUpdateAssugned = true;

                _inspector.OnStart += () => 
                {
                    StatusText.Content = "Wait...";
                };

                _inspector.OnUpdate += (stage, state) =>
                {
                    if (stage == InspectorStage.Progress) 
                    {
                        ProgressBar.Visibility = Visibility.Visible;
                        ProgressBar.Maximum = state.All;
                        ProgressBar.Value = state.Used;
                    }

                    if (stage == InspectorStage.Progress2) 
                    {
                        ProgressBar2.Visibility = Visibility.Visible;
                        ProgressBar2.Maximum = state.All;
                        ProgressBar2.Value = state.Used;
                    }

                    if (stage == InspectorStage.FetchingFiles) 
                    {
                        StatusText.Content = $"Fetching files: {state.All}";
                    }

                    if (stage == InspectorStage.FetchingLines) 
                    {
                        StatusText.Content = $"Fetching lines: {state.All}";
                    }
                };

                _inspector.OnComplete += () =>
                {
                    UpdateProjectsList();
                    Save();
                    SendData();
                    //
                    StatusText.Content = "Complete";
                    ProgressBar.Visibility = Visibility.Hidden;
                    ProgressBar2.Visibility = Visibility.Hidden;
                };
            }

            _inspector.Start(_projects.ToList(), Dispatcher);
        }

        public void SendData()
        {
            if (Data.SendData)
            {
                WebMethods.UpdateProjects(Data, _projects.Select(proj => (ProjectEntity)proj).ToArray(), result =>
                {
                    WebClient.ProcessResponse(result, data =>
                    {
                        if (data != "1")
                            MessageHelper.Error(data);
                    });
                });
            }
        }

        #endregion

        #region Window Events

        private void AutorunCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            checkBox.IsChecked = _autorun.SetState(checkBox.IsChecked.Value, ex => {
                MessageHelper.Error(ex.ToString(), ex.GetType().Name);
            });
        }

        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_addProjectWindow != null)
            {
                _addProjectWindow.Close();
                _addProjectWindow = null;
            }

            _addProjectWindow = new AddProjectWindow((project) =>
            {
                _projects.Insert(0, project);
                _addProjectWindow.Close();
                //
                UpdateProjectsList();
                //
                Save();
            })
            {
                Owner = this
            };

            _addProjectWindow.Show();
        }

        private void ServerAccessButton_Click(object sender, RoutedEventArgs e)
        {
            if (_serverAccessWindow != null)
            {
                _serverAccessWindow.Close();
                _serverAccessWindow = null;
            }

            _serverAccessWindow = new ServerAccessWindow(Data, () => Save()) { Owner = this };
            _serverAccessWindow.Show();
        }

        private void ProjectOpenButton_Click(object sender, RoutedEventArgs e)
        {
            var title = (string)((Button)sender).Tag;

            foreach (var proj in _projects)
            {
                if (proj.Title == title)
                {
                    var win = new ProjectWindow(proj) { Owner = this };
                    win.Show();
                }
            }
        }

        private void ProjectDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var title = (string)((Button)sender).Tag;

            foreach (var proj in _projects)
            {
                if (proj.Title == title)
                {
                    var dialog = new DeleteProjectDialog(proj, () => 
                    {
                        _projects.Remove(proj);
                        UpdateProjectsList();
                        Save();
                    });

                    dialog.Show();
                }
            }
        }

        private void ProjectEditButton_Click(object sender, RoutedEventArgs e)
        {
            var title = (string)((Button)sender).Tag;

            foreach (var proj in _projects)
            {
                if (proj.Title == title)
                {
                    var win = new EditProjectWindow(proj, () => 
                    {
                        UpdateProjectsList();
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

        private void SummaryButton_Click(object sender, RoutedEventArgs e)
        {
            Project 
                all      = new Project("", "All projects")     { Info = new ProjectInfo() },
                @public  = new Project("", "Public projects")  { Info = new ProjectInfo() },
                @private = new Project("", "Private projects") { Info = new ProjectInfo() };

            foreach (var proj in _projects)
            {
                all.Info.Volume += proj.Info.Volume;
                Merge(all.Info.ExtensionsVolume, proj.Info.ExtensionsVolume);

                if (proj.IsPublic)
                {
                    @public.Info.Volume += proj.Info.Volume;
                    Merge(@public.Info.ExtensionsVolume, proj.Info.ExtensionsVolume);
                }
                else
                {
                    @private.Info.Volume += proj.Info.Volume;
                    Merge(@private.Info.ExtensionsVolume, proj.Info.ExtensionsVolume);
                }

                all.Info.Errors.AddRange(proj.Info.Errors.Select(t => $"{proj.Title} -> {t}"));
                //
                static void Merge(Dictionary<string, CodeVolume> a, Dictionary<string, CodeVolume> b)
                {
                    foreach (var pair in b)
                    {
                        if (!a.ContainsKey(pair.Key))
                        {
                            a[pair.Key] = pair.Value;
                        }
                        else
                        {
                            a[pair.Key] += pair.Value;
                        }
                    }
                }
            }

            var win = new ProjectWindow(new Project[] { all, @public, @private })
            {
                Title = "Summary",
                Owner = this
            };

            win.Show();
        }

        #endregion
    }
}
