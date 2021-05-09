using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using CodeBase.Client.Pages;
using CodeBase.Domain;
using CodeBase.Domain.Models;
using CodeBase.Domain.Services;

namespace CodeBase.Client.Views
{
    public partial class MainWindow : Window
    {
        private readonly DataManager<AppData> _dataManager;
        private readonly AppData _appData;
        private readonly Context _context;
        private readonly HashSet<Page> _activePages;
        private readonly Pulse _pulse;
        private readonly InspectorService _inspector;

        private bool _isUpdateAssigned;
        private Type _currentPageType;
        private System.Windows.Forms.NotifyIcon _notifyIcon;

        public MainWindow()
        {
            InitializeComponent();

            _activePages = new HashSet<Page>();
            _dataManager = new DataManager<AppData>("prefs.json");

            try
            {
                _appData = _dataManager.Load();
            }
            catch (Exception ex)
            {
                MessageHelper.Error(ex.Message);
                Close();
            }

            _context = CreateContext();
            _pulse = new Pulse(_context, () => StartInspector());
            _inspector = new InspectorService();

            Loaded += MainWindow_Loaded;
            StateChanged += MainWindow_StateChanged;
            Closing += MainWindow_Closing;

            Start();
        }

        ~MainWindow()
        {
            _notifyIcon?.Dispose();
        }

        private void Start()
        {
            _pulse.Start();
            Navigate(typeof(ProjectsPage));
        }

        private Context CreateContext()
        {
            return new Context
            {
                AppData = _appData,
                DataManager = _dataManager,
                MainWindow = this,
                Navigate = Navigate,
            };
        }

        private void CreateTrayIcon()
        {
            // Поведение иконки в tray
            _notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Icon = new System.Drawing.Icon(@".\Resources\CodeBaseLogo.ico"),
                Text = Title,
            };

            _notifyIcon.MouseClick += NotifyIcon_MouseClick;
            _notifyIcon.MouseDoubleClick += NotifyIcon_MouseClick;

            var menuStrip = new System.Windows.Forms.ContextMenuStrip();

            var closeItem = new System.Windows.Forms.ToolStripButton
            {
                Text = "Close",
                Image = System.Drawing.Image.FromFile(@".\Resources\Close.png"),
            };

            closeItem.Click += (s, e) => Application.Current.Shutdown();

            menuStrip.Items.Add(closeItem);
            menuStrip.Width = 200;

            _notifyIcon.ContextMenuStrip = menuStrip;
        }



        private void SaveData()
        {
            try
            {
                _appData.WindowWidth = Width;
                _appData.WindowHeight = Height;
                _appData.WindowState = WindowState;

                _dataManager.Save(_appData);
            }
            catch (Exception ex)
            {
                MessageHelper.Error(ex.Message);
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Width = _appData.WindowWidth ?? Width;
            Height = _appData.WindowHeight ?? Height;
            WindowState = _appData.WindowState ?? WindowState;

            CreateTrayIcon();

            ProgressBarPrimary.Visibility = Visibility.Hidden;
            ProgressBarSecondary.Visibility = Visibility.Hidden;
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (_context.AppData.IsTrayCollapseEnabled && _notifyIcon is not null)
            {
                _notifyIcon.Visible = WindowState == WindowState.Minimized;
                ShowInTaskbar = WindowState != WindowState.Minimized;
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveData();

            if (_context.AppData.IsTrayCollapseEnabled)
            {
                WindowState = WindowState.Minimized;
                e.Cancel = true;
            }
        }

        private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                WindowState = WindowState.Normal;
                Activate();
            }
        }

        private void UpdateSidebarMenu()
        {
            var pages = GetType().Assembly.GetTypes()
                .Where(x => x.IsSubclassOf(typeof(Page)))
                .Select(x => new
                {
                    Type = x,
                    Descriptor = x.GetCustomAttributes().OfType<PageDescriptorAttribute>().FirstOrDefault(),
                })
                .Where(x => x.Descriptor is not null)
                .Where(x => x.Descriptor.Title is not null)
                .OrderBy(x => x.Descriptor.Order)
                .ToArray();

            var links = pages
                .Select(x => new PageLink
                {
                    Title = x.Descriptor.Title,
                    PageTypeName = x.Type.Name,
                    IsActive = x.Type == _currentPageType,
                })
                .ToArray();

            if (SidebarMenu.ItemsSource != null)
            {
                SidebarMenu.ItemsSource = null;
            }

            SidebarMenu.Items.Clear();
            SidebarMenu.ItemsSource = links;
        }

        private void SidebarItem_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var pageTypeName = (sender as FrameworkElement).Tag?.ToString() ?? "";

            var pageType = GetType().Assembly.GetTypes().FirstOrDefault(x => x.Name == pageTypeName);

            if (pageType is null)
            {
                return;
            }

            Navigate(pageType);
        }

        private void Navigate(Type pageType)
        {
            var pageDescriptor = pageType.GetCustomAttributes()
                .OfType<PageDescriptorAttribute>().FirstOrDefault();

            if (pageDescriptor is null)
            {
                return;
            }

            var pageItem = _activePages.FirstOrDefault(x => x.GetType() == pageType);

            if (pageDescriptor.Lifetime == PageLifetime.Transient || pageItem == null)
            {
                _activePages.Remove(pageItem);
                pageItem = (Page)Activator.CreateInstance(pageType, args: _context);
                _activePages.Add(pageItem);
            }

            _currentPageType = pageType;
            PageFrame.Navigate(pageItem);
            UpdateSidebarMenu();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            StartInspector();
        }

        private void StartInspector()
        {
            if (!_isUpdateAssigned)
            {
                _isUpdateAssigned = true;

                _inspector.OnStart += () =>
                {
                    StatusText.Content = "Wait...";
                };

                _inspector.OnUpdate += (stage, state) =>
                {
                    if (stage == InspectorStage.ProgressPrimary)
                    {
                        ProgressBarPrimary.Visibility = Visibility.Visible;
                        ProgressBarPrimary.Maximum = state.All;
                        ProgressBarPrimary.Value = state.Used;
                    }

                    if (stage == InspectorStage.ProgressSecondary)
                    {
                        ProgressBarSecondary.Visibility = Visibility.Visible;
                        ProgressBarSecondary.Maximum = state.All;
                        ProgressBarSecondary.Value = state.Used;
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
                    Navigate(_currentPageType);
                    SaveData();
                    //
                    StatusText.Content = "Complete";
                    ProgressBarPrimary.Visibility = Visibility.Hidden;
                    ProgressBarSecondary.Visibility = Visibility.Hidden;
                };
            }

            _inspector.Start(_appData.Projects.ToList(), Dispatcher);
        }

    }
}
