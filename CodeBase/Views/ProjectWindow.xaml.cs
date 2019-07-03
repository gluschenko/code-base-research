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
    public partial class ProjectWindow : Window
    {
        private Project[] Projects;

        public ProjectWindow(Project project)
            : this(new Project[] { project }) {}

        public ProjectWindow(Project[] Projects)
        {
            InitializeComponent();

            this.Projects = Projects;
            Title = Title.Replace("{Title}", Projects[0]?.Title ?? "");
            Width *= 2;
            Height *= 1.5f;

            OutputTextBox.Text = "";
            foreach (var proj in Projects) FillText(proj);

            if (Projects.Length == 1)
            {
                if (Projects[0] == null) Close();
            }
        }

        public void FillText(Project Project)
        {
            if (Project == null) return;

            string text = "";

            void push(string line) { text += line + Environment.NewLine; }
            //
            if(Project.Title != "")
                push(Project.Title);
            if(Project.Path != "")
                push($"Path: {Project.Path}");
            if(Project.LastEdit != 0)
                push($"Last edit: {UnixTime.ToDateTime(Project.LastEdit)}");

            push("");
            if (Project.Folders.Count > 0)
            {
                push($"Folders: ");

                int i = 0;
                foreach (string folder in Project.Folders)
                {
                    push($"{++i}. {folder}");
                }

                push("");
            }

            var info = Project.Info;

            push($"SLOC/lines: {info.Volume} ({info.Volume.Ratio * 100}%)");
            push("");

            if (info.ExtensionsVolume.Count > 0)
            {
                push($"Extensions ({info.ExtensionsVolume.Count}): ");

                int i = 0;
                var sorted = info.ExtensionsVolume.OrderBy(v => -v.Value.Lines);
                foreach (var pair in sorted)
                {
                    var vol = pair.Value;
                    push($"{++i}. {pair.Key} ".PadRight(40, '.') + $" {vol} ({vol.Ratio * 100}%) [{vol.Files} files]");
                }

                push("");
            }

            if (info.FilesVolume.Count > 0)
            {
                FileTree tree = new FileTree("root");

                push($"Files ({info.FilesVolume.Count}): ");
                foreach (var pair in info.FilesVolume)
                {
                    tree.Add(pair.Key, pair.Value);
                }
                push(tree.ToString());
                push("");
            }

            if (info.Errors.Count > 0)
            {
                push($"Errors ({info.Errors.Count}): ");
                int i = 0;
                foreach (var error in info.Errors)
                {
                    push($"{++i}. {error}");
                }
                push("");
            }

            push("".PadRight(50, '_'));
            push("");
            //
            OutputTextBox.Text += text;
        }

        class FileTree
        {
            Dictionary<string, FileTree> nodes = new Dictionary<string, FileTree>();

            public string Key = "";
            public CodeVolume Volume
            {
                get
                {
                    if (nodes.Count > 0)
                    {
                        CodeVolume volume = new CodeVolume();
                        foreach (var node in nodes) volume += node.Value.Volume;
                        return volume;
                    }
                    return _Volume;
                }
            }
            private CodeVolume _Volume;

            public FileTree(string key)
            {
                Key = key;
                _Volume = new CodeVolume();
            }

            public void Add(string path, CodeVolume volume)
            {
                Add(path.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries), volume);
            }

            public void Add(string[] path, CodeVolume volume)
            {
                if (path.Length > 0)
                {
                    string key = path[0];
                    path = path.Where((val, i) => i != 0).ToArray();

                    if (!nodes.ContainsKey(key))
                    {
                        nodes.Add(key, new FileTree(key));
                    }
                    nodes[key].Add(path, volume);
                }
                else
                {
                    _Volume = volume;
                }
            }

            public override string ToString()
            {
                return ToString(0);
            }

            public string ToString(int span)
            {
                string output = "";

                string _key = (nodes.Count > 0 ? "\\" : "") + Key + " ";
                output += $"{_key.PadLeft(span + _key.Length).PadRight(40, '.')} {Volume} ({Volume.Ratio * 100}%)" + Environment.NewLine;

                if (nodes.Count > 0)
                {
                    span += 2;

                    foreach (var node in nodes)
                    {
                        output += node.Value.ToString(span);
                    }
                }

                return output;
            }
        }
    }
}
