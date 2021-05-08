using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using CodeBase.Domain;
using CodeBase.Domain.Models;
using CodeBase.Domain.Services;

namespace CodeBase.Client.Pages
{
    [PageDescriptor(PageLifetime.Transient)]
    public partial class ProjectPage : Page
    {
        private readonly Context _context;

        public ProjectPage(Context context)
        {
            InitializeComponent();

            _context = context;

            PageHeader.Title = _context.CurrentProject.Title;

            OutputTextBox.Text = GetText(_context.CurrentProject);
        }

        private static string GetText(Project project)
        {
            if (project == null)
            {
                return string.Empty;
            }

            var text = new StringBuilder();

            void Push(string line)
            {
                text.Append(line);
                text.Append(Environment.NewLine);
            }
            
            if (!string.IsNullOrWhiteSpace(project.Title))
            {
                Push(project.Title);
            }

            if (!string.IsNullOrWhiteSpace(project.Location))
            {
                Push($"Location: {project.Location}");
            }

            if (project.LastRevision > default(DateTime))
            {
                Push($"Last edit: {project.LastRevision}");
            }

            Push("");
            if (project.Folders?.Count > 0)
            {
                Push($"Folders: ");

                var i = 0;
                foreach (var folder in project.Folders)
                {
                    Push($"{++i}. {folder}");
                }

                Push("");
            }

            var info = project.Info;

            Push($"SLOC/lines: {info.Volume} ({info.Volume.Ratio * 100}%)");
            Push("");

            if (info.ExtensionsVolume?.Any() ?? false)
            {
                Push($"Extensions ({info.ExtensionsVolume.Count}): ");

                var i = 0;
                var sorted = info.ExtensionsVolume.OrderBy(v => -v.Value.Lines);
                foreach (var pair in sorted)
                {
                    var vol = pair.Value;
                    Push($"{++i}. {pair.Key} ".PadRight(40, '.') + $" {vol} ({vol.Ratio * 100}%) [{vol.Files} files]");
                }

                Push("");
            }

            if (info.FilesVolume?.Any() ?? false)
            {
                var tree = new FileTreeNode("root");

                Push($"Files ({info.FilesVolume.Count}): ");
                foreach (var pair in info.FilesVolume)
                {
                    tree.Add(pair.Key, pair.Value);
                }
                Push(tree.ToString());
                Push("");
            }

            if (info.Errors?.Any() ?? false)
            {
                Push($"Errors ({info.Errors.Count}): ");
                var i = 0;
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

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            _context.Navigate(typeof(ProjectEditPage));
        }

        private void FilesButton_Click(object sender, RoutedEventArgs e)
        {
            _context.Navigate(typeof(ProjectFilesPage));
        }

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
            private readonly Dictionary<string, FileTreeNode> _nodes;

            public FileTreeNode(string key)
            {
                Key = key;

                _nodes = new();
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
