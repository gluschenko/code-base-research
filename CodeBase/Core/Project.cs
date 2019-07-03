using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace CodeBase
{
    // Сущность кастуемая от Project для передачи в сеть
    public struct ProjectEntity
    {
        public string Title { get; set; }
        public string Color { get; set; }
        public long LastEdit { get; set; }
        public int IsPublic { get; set; }
        public int IsLocal { get; set; }
        public int IsNameHidden { get; set; }

        public int SLOC { get; set; }
        public int Lines { get; set; }
        public int Files { get; set; }

        public EntityVolume[] Extensions { get; set; }

        public static explicit operator ProjectEntity(Project proj)
        {
            return new ProjectEntity {
                Title = proj.Title,
                Color = proj.Color,
                LastEdit = proj.LastEdit,
                IsPublic = proj.IsPublic ? 1 : 0,
                IsLocal = proj.IsLocal ? 1 : 0,
                IsNameHidden = proj.IsNameHidden ? 1 : 0,

                SLOC = proj.Info.Volume.SLOC,
                Lines = proj.Info.Volume.Lines,
                Files = proj.Info.Volume.Files,

                Extensions = proj.Info.ExtensionsVolume.Select(p => new EntityVolume {
                    Title = p.Key,
                    SLOC = p.Value.SLOC,
                    Lines = p.Value.Lines,
                    Files = p.Value.Files,
                }).ToArray(),
            };
        }

        public struct EntityVolume
        {
            public string Title;
            public int SLOC;
            public int Lines;
            public int Files;
        }
    }

    [DataContract]
    public class Project
    {
        [DataMember]
        public string Path { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Color { get; set; }
        [DataMember]
        public List<string> Folders { get; set; }
        [DataMember]
        public List<string> IgnoredFolders { get; set; }
        [DataMember]
        public bool IsPublic { get; set; }
        [DataMember]
        public bool IsLocal { get; set; }
        [DataMember]
        public bool IsNameHidden { get; set; }
        [DataMember]
        public long LastEdit { get; set; }

        [DataMember]
        public ProjectInfo Info { get; set; }
        //
        public string TitleText {
            get => Title + (IsPublic ? " ✔" : "") + (IsLocal ? " [local]" : "") + (IsNameHidden ? " [hidden]" : "");
        }

        public Brush BrushColor {
            get => new SolidColorBrush((Color)ColorConverter.ConvertFromString(Color ?? RandomizeColor()));
        }
        //
        public Project(string path, string title)
        {
            Path = path;
            Title = title;
            Color = "#FFFF00";
            Folders = new List<string>();
            IgnoredFolders = new List<string>();
            Info = new ProjectInfo();
            IsPublic = false;
            IsNameHidden = false;
            IsLocal = true;
        }

        public Project() : this("", "") { }

        //

        public List<string> GetFiles(List<string> extensions, List<string> blackList)
        {
            List<string> files = new List<string>();

            if (Directory.Exists(Path))
            {
                List<string> subDirs;
                if (Folders.Count == 0)
                {
                    subDirs = new List<string>(Directory.GetDirectories(Path));
                }
                else
                {
                    subDirs = new List<string>();

                    foreach (string folder in Folders)
                    {
                        string dir = System.IO.Path.Combine(Path, folder);
                        if (Directory.Exists(dir) && !subDirs.Contains(dir))
                        {
                            subDirs.Add(dir);
                        }
                    }
                }
                //
                void getFiles(string[] dirs)
                {
                    foreach (string dir in dirs)
                    {
                        string[] _dirs = Directory.EnumerateDirectories(dir).Where(_dir =>
                        {
                            if (IgnoredFolders != null)
                            {
                                foreach (string folder in IgnoredFolders)
                                {
                                    string ignoredDir = System.IO.Path.Combine(Path, folder.Replace('/', '\\'));
                                    if (ignoredDir == _dir)
                                    {
                                        return false;
                                    }
                                }
                            }
                            return true;
                        }).ToArray();

                        //string[] _files = Directory.EnumerateFiles(dir).Where(p => extensions.Contains(System.IO.Path.GetExtension(p))).ToArray();

                        string[] _files = Directory.EnumerateFiles(dir).Where(_file => {
                            if (extensions.Contains(System.IO.Path.GetExtension(_file)))
                            {
                                foreach (string b in blackList)
                                {
                                    if (_file.EndsWith(b))
                                        return false;
                                }
                                return true;
                            }
                            return false;
                        }).ToArray();

                        files.AddRange(_files);
                        //
                        if (_dirs.Length > 0) getFiles(_dirs);
                    }
                }

                getFiles(subDirs.ToArray());
            }

            return files;
        }

        //

        private string RandomizeColor()
        {
            var random = new Random(GetHashCode());
            int rgb = random.Next(0, 0xFFFFFF);
            Color = "#" + Convert.ToString(rgb, 16).PadLeft(6, '0');
            return Color;
        }
    }

    public class ProjectInfo
    {
        public CodeVolume Volume { get; set; }
        //
        public Dictionary<string, CodeVolume> FilesVolume { get; set; } = new Dictionary<string, CodeVolume>();
        public Dictionary<string, CodeVolume> ExtensionsVolume { get; set; } = new Dictionary<string, CodeVolume>();
        //
        public List<string> Errors { get; set; } = new List<string>();
        //
        public string SourceLinesText {
            get => Volume + (Errors.Count > 0 ? $" ({Errors.Count} errors)" : "");
        }
        //
        public void Error(string text)
        {
            Errors.Add(text);
        }
    }

    public struct CodeVolume
    {
        public int SLOC;
        public int Lines;
        public int Files;

        public double Ratio { get => Lines != 0 ? Math.Round((double)SLOC / Lines, 4) : 1; }

        public CodeVolume(int SLOC, int Lines, int Files)
        {
            this.SLOC = SLOC;
            this.Lines = Lines;
            this.Files = Files;
        }

        public override string ToString()
        {
            return $"{SLOC}/{Lines}";
        }

        public static CodeVolume operator +(CodeVolume A, CodeVolume B)
        {
            return new CodeVolume(A.SLOC + B.SLOC, A.Lines + B.Lines, A.Files + B.Files);
        }

        public static CodeVolume operator -(CodeVolume A, CodeVolume B)
        {
            return new CodeVolume(A.SLOC - B.SLOC, A.Lines - B.Lines, A.Files - B.Files);
        }
    }
}
