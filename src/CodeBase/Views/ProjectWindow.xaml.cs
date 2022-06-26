using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using CodeBase.Core;
using CodeBase.Models;

namespace CodeBase
{
    public partial class ProjectWindow : Window
    {
        private readonly Project[] _projects;

        public ProjectWindow(Project project) : this(new[] { project }) { }

        public ProjectWindow(Project[] projects)
        {
            InitializeComponent();

            _projects = projects;
            Title = Title.Replace("{Title}", projects[0]?.Title ?? "");
            Width *= 2.0;
            Height *= 1.5;

            OutputTextBox.Text = "";
            foreach (var proj in projects)
            {
                try
                {
                    OutputTextBox.Text += GetText(proj);
                }
                catch (Exception ex)
                {
                    OutputTextBox.Text += ex.ToString();
                }
            }

            if (projects.Length == 1)
            {
                if (projects[0] == null) Close();
            }

            SourceFilesList.Items.Clear();
            GitIgnoreList.Items.Clear();
        }

        public string GetText(Project project)
        {
            if (project == null) return string.Empty;

            var text = new StringBuilder();

            void Push(string line)
            {
                text.Append(line);
                text.Append(Environment.NewLine);
            }
            //
            if (project.Title != "")
                Push(project.Title);
            if (project.Path != "")
                Push($"Path: {project.Path}");
            if (project.LastEdit != 0)
                Push($"Last edit: {UnixTime.ToDateTime(project.LastEdit)}");

            Push("");
            if (project.Folders?.Count > 0)
            {
                Push($"Folders: ");

                int i = 0;
                foreach (string folder in project.Folders)
                {
                    Push($"{++i}. {folder}");
                }

                Push("");
            }

            var info = project.Info;

            Push($"SLOC/lines: {info.Volume} ({info.Volume.Ratio * 100}%)");
            Push("");

            if (info.ExtensionsVolume?.Count > 0)
            {
                Push($"Extensions ({info.ExtensionsVolume.Count}): ");

                int i = 0;
                var sorted = info.ExtensionsVolume.OrderBy(v => -v.Value.Lines);
                foreach (var pair in sorted)
                {
                    var vol = pair.Value;
                    Push($"{++i}. {pair.Key} ".PadRight(40, '.') + $" {vol} ({vol.Ratio * 100}%) [{vol.Files} files]");
                }

                Push("");
            }

            if (info.FilesVolume?.Count > 0)
            {
                FileTreeNode tree = new FileTreeNode("root");

                Push($"Files ({info.FilesVolume.Count}): ");
                foreach (var pair in info.FilesVolume)
                {
                    tree.Add(pair.Key, pair.Value);
                }
                Push(tree.ToString());
                Push("");
            }

            if (info.Errors?.Count > 0)
            {
                Push($"Errors ({info.Errors.Count}): ");
                int i = 0;
                foreach (var error in info.Errors)
                {
                    Push($"{++i}. {error}");
                }
                Push("");
            }

            Push("".PadRight(50, '_'));
            Push("");
            //
            return text.ToString();
        }

        public void Freeze(bool state, string message = "Wait...")
        {
            if (state)
            {
                FreezeOverlay.Visibility = Visibility.Visible;
                TabControl.Effect = new BlurEffect() { Radius = 10, KernelType = KernelType.Gaussian };
            }
            else
            {
                FreezeOverlay.Visibility = Visibility.Hidden;
                TabControl.Effect = null;
            }

            FreezeText.Text = message;
        }

        public void Freeze(string message = "Wait...") => Freeze(true, message);

        #region HANDLERS

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void SourceFilesTab_Selected(object sender, RoutedEventArgs e)
        {
            if (_projects == null) return;

            if (SourceFilesList.Items.Count == 0)
            {
                foreach (var proj in _projects)
                {
                    if (Path.IsPathRooted(proj.Path))
                    {
                        Freeze();

                        Task.Run(() =>
                        {
                            var files = proj.GetFiles(InspectorConfig.CodeExtensions, InspectorConfig.FilesBlackList,
                                (subFiles, subDirs, currentDir) =>
                                {
                                    Dispatcher.Invoke(() => Freeze($"{currentDir}\nForlders: {subDirs} | Files: {subFiles}"));
                                });

                            var projectString = $"Project: {proj.Path} ({files.Count} files)";

                            Dispatcher.Invoke(() =>
                            {
                                SourceFilesList.Items.Add(new FilesListItem(projectString, FilesListItem.Default));

                                foreach (var file in files)
                                {
                                    var filePath = file.Path.Substring(proj.Path.Length);
                                    SourceFilesList.Items.Add(new FilesListItem(filePath,
                                        file.IsMatch ? FilesListItem.Green : FilesListItem.Red));
                                }

                                Freeze(false);
                            });
                        });
                    }
                }
            }
        }

        private void GitIgnoreTab_Selected(object sender, RoutedEventArgs e)
        {
            if (_projects == null) return;

            if (GitIgnoreList.Items.Count == 0)
            {
                foreach (var proj in _projects)
                {
                    if (Path.IsPathRooted(proj.Path))
                    {
                        Freeze();

                        var task = Task.Run(() =>
                        {
                            var files = new List<FilesListItem>();
                            var git_files = GitIgnoreReader
                                .Find(proj.Path, SearchOption.AllDirectories)
                                .Select(p => GitIgnoreReader.Load(p));

                            Dispatcher.Invoke(() =>
                            {
                                foreach (var git_file in git_files)
                                {
                                    GitIgnoreList.Items.Add(new FilesListItem(git_file.Path, FilesListItem.Default));

                                    foreach (var rule in git_file.Rules)
                                    {
                                        var item = new FilesListItem($"{rule.Source} -> {rule.Pattern}" + (rule.IsNegative ? " [NEGATIVE]" : ""),
                                            rule.IsValid ? FilesListItem.Green : FilesListItem.Red);
                                        GitIgnoreList.Items.Add(item);
                                    }
                                }

                                Freeze(false);
                            });
                        });

                        Closing += (ss, ee) => task.Dispose();
                    }
                }
            }
        }

        #endregion

        //

        private class FileTreeNode
        {
            public string Key { get; set; } = "";
            public CodeVolume Volume
            {
                get
                {
                    if (_nodes.Count > 0)
                    {
                        var volume = new CodeVolume();
                        foreach (var node in _nodes) volume += node.Value.Volume;
                        return volume;
                    }
                    return _volume;
                }
            }

            private CodeVolume _volume;
            private readonly Dictionary<string, FileTreeNode> _nodes = new Dictionary<string, FileTreeNode>();

            public FileTreeNode(string key)
            {
                Key = key;
                _volume = new CodeVolume();
            }

            public void Add(string path, CodeVolume volume)
            {
                Add(path.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries), volume);
            }

            public void Add(string[] path, CodeVolume volume)
            {
                if (path.Length > 0)
                {
                    var key = path[0];
                    path = path.Where((val, i) => i != 0).ToArray();

                    if (!_nodes.ContainsKey(key))
                    {
                        _nodes.Add(key, new FileTreeNode(key));
                    }
                    _nodes[key].Add(path, volume);
                }
                else
                {
                    _volume = volume;
                }
            }

            public string ToString(int span)
            {
                var output = "";

                var key = (_nodes.Count > 0 ? "\\" : "") + Key + " ";
                output += $"{key.PadLeft(span + key.Length).PadRight(40, '.')} {Volume} ({Volume.Ratio * 100}%)" + Environment.NewLine;

                if (_nodes.Count > 0)
                {
                    span += 2;

                    foreach (var node in _nodes)
                    {
                        output += node.Value.ToString(span);
                    }
                }

                return output;
            }

            public override string ToString() => ToString(0);
        }
    }

}
