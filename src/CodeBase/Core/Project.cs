using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;
using PathIO = System.IO.Path;

namespace CodeBase
{
    // Сущность кастуемая от Project для передачи по WebAPI
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
            return new ProjectEntity 
            {
                Title = proj.Title,
                Color = proj.Color,
                LastEdit = proj.LastEdit,
                IsPublic = proj.IsPublic ? 1 : 0,
                IsLocal = proj.IsLocal ? 1 : 0,
                IsNameHidden = proj.IsNameHidden ? 1 : 0,

                SLOC = proj.Info.Volume.SLOC,
                Lines = proj.Info.Volume.Lines,
                Files = proj.Info.Volume.Files,

                Extensions = proj.Info.ExtensionsVolume.Select(p => new EntityVolume 
                {
                    Title = p.Key,
                    SLOC = p.Value.SLOC,
                    Lines = p.Value.Lines,
                    Files = p.Value.Files,
                }).ToArray(),
            };
        }

        public struct EntityVolume
        {
            public string Title { get; set; }
            public int SLOC { get; set; }
            public int Lines { get; set; }
            public int Files { get; set; }
        }
    }

    [DataContract]
    public class Project
    {
        [DataMember] public string Path { get; set; }
        [DataMember] public string Title { get; set; }
        [DataMember] public string Color { get; set; }
        [DataMember] public List<string> Folders { get; set; }
        [DataMember] public List<string> IgnoredFolders { get; set; }
        [DataMember] public bool IsPublic { get; set; }
        [DataMember] public bool IsLocal { get; set; }
        [DataMember] public bool IsNameHidden { get; set; }
        [DataMember] public long LastEdit { get; set; }
        [DataMember] public ProjectInfo Info { get; set; }
        //
        public string TitleText { get => GetTitle(); }
        public Brush BrushColor { get => GetBrush(); }
        //
        public delegate void GetFilesUpdate(int files, int dirs, string currentDir);
        //
        public Project(string path, string title)
        {
            Path = path;
            Title = title;
            Color = RandomizeColor(); //"#FFFF00";
            Folders = new List<string>();
            IgnoredFolders = new List<string>();
            Info = new ProjectInfo();
            IsPublic = false;
            IsNameHidden = false;
            IsLocal = true;
        }

        public Project() : this("", "") { }

        //

        public string GetTitle() 
        {
            return string.Join(string.Empty, 
                Title, 
                IsPublic ? " ✔" : "", 
                IsLocal ? " [local]" : "", 
                IsNameHidden ? " [hidden]" : "");
        }

        public Brush GetBrush() 
        {
            var color = (Color)ColorConverter.ConvertFromString(Color ?? RandomizeColor());
            return new SolidColorBrush(color);
        }

        public List<FileItem> GetFiles(HashSet<string> extensions, HashSet<string> blackList, GetFilesUpdate onProgress = null)
        {
            string path = Path;
            var list = new List<FileItem>();

            if (Directory.Exists(path))
            {
                var allowedDirs = GetAllowedFolders();
                //
                var gitIgnores = GitIgnoreReader.Find(path, SearchOption.AllDirectories).Select(p => GitIgnoreReader.Load(p));
                var queue = new Queue<KeyValuePair<string, IEnumerable<GitIgnoreReader>>>();

                int dirsCount = 0;

                getFiles(path);

                void getFiles(string dir)
                {
                    var relevantGitFiles = gitIgnores.Where(f => GitIgnoreReader.IsChildedPath(f.BaseDir, dir));

                    var isAllowedDir = 
                        allowedDirs.Count > 0 ? allowedDirs.Any(p => GitIgnoreReader.IsChildedPath(p, dir)) : true;

                    var subs = Directory.EnumerateDirectories(dir)
                        .Where(p => !IsIgnoredForder(p))
                        .Select(s => s.Replace('\\', '/'));

                    var files = Directory.EnumerateFiles(dir)
                        .Where(s => isAllowedDir)
                        .Where(s => extensions.Contains(PathIO.GetExtension(s)))
                        .Where(s => blackList.All(end => !s.EndsWith(end)))
                        .Select(s => s.Replace('\\', '/'));

                    dirsCount += subs.Count();

                    onProgress?.Invoke(queue.Count, dirsCount, dir);

                    foreach (var file in files)
                    {
                        queue.Enqueue(new KeyValuePair<string, IEnumerable<GitIgnoreReader>>(file, relevantGitFiles));
                    }

                    foreach (var sub in subs.Select(s => s + '/'))
                    {
                        bool is_match = relevantGitFiles.All(r => r.IsMatch(sub));

                        if (is_match)
                        {
                            getFiles(sub);
                        }
                    }
                }

                while (queue.Count > 0)
                {
                    var pair = queue.Dequeue();
                    bool isMatch = pair.Value.All(r => r.IsMatch(pair.Key));
                    list.Add(new FileItem(pair.Key, isMatch));

                    onProgress?.Invoke(queue.Count, 0, pair.Key);
                }
            }

            return list;
        }

        public bool IsIgnoredForder(string path)
        {
            return IgnoredFolders?.Any(f => GitIgnoreReader.IsChildedPath(PathIO.Combine(Path, f), path)) ?? false;
        }

        public List<string> GetAllowedFolders() 
        {
            if (Folders?.Count == 0)
            {
                return new List<string>(Directory.GetDirectories(Path));
            }
            else
            {
                var dirs = Folders
                    .Select(folder => PathIO.Combine(Path, folder))
                    .Where(dir => Directory.Exists(dir));

                return dirs.ToList();
            }
        }

        //
        static Random _random;

        private string RandomizeColor()
        {
            _random ??= new Random(GetHashCode());

            int rgb = _random.Next(0, 0xFFFFFF);
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

        public void Clear() 
        {
            ExtensionsVolume?.Clear();
            FilesVolume?.Clear();
            Errors?.Clear();
        }
    }

    public struct CodeVolume
    {
        public int SLOC { get; set; }
        public int Lines { get; set; }
        public int Files { get; set; }

        public double Ratio { get => GetLineRatio(); }

        public CodeVolume(int sloc, int lines, int files)
        {
            SLOC = sloc;
            Lines = lines;
            Files = files;
        }

        public double GetLineRatio() 
        {
            return Lines != 0 ? Math.Round((double)SLOC / Lines, 4) : 1;
        }

        public override string ToString() => $"{SLOC}/{Lines}";

        public static CodeVolume operator +(CodeVolume a, CodeVolume b) => 
            new CodeVolume(a.SLOC + b.SLOC, a.Lines + b.Lines, a.Files + b.Files);

        public static CodeVolume operator -(CodeVolume a, CodeVolume b) => 
            new CodeVolume(a.SLOC - b.SLOC, a.Lines - b.Lines, a.Files - b.Files);
    }

    public struct FileItem
    {
        public string Path { get; private set; }
        public bool IsMatch { get; private set; }

        public FileItem(string path, bool isMatch)
        {
            Path = path;
            IsMatch = isMatch;
        }
    }
}
