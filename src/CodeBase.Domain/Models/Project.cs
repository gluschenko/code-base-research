using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace CodeBase.Domain.Models
{
    public class Project
    {
        public Guid ID { get; set; }
        public string Location { get; set; }
        public string Title { get; set; }
        public string Color { get; set; }
        public List<string> Folders { get; set; }
        public List<string> IgnoredFolders { get; set; }
        public bool IsPublic { get; set; }
        public bool IsLocal { get; set; }
        public bool IsNameHidden { get; set; }
        public long LastEdit { get; set; }
        public ProjectInfo Info { get; set; }
        
        public string TitleText => GetTitle();
        public Brush BrushColor => GetBrush();
        
        public delegate void GetFilesUpdate(int files, int dirs, string currentDir);

        public Project()
        {
            Color = RandomizeColor();
            Folders = new List<string>();
            IgnoredFolders = new List<string>();
            Info = new ProjectInfo();
            IsPublic = false;
            IsNameHidden = false;
            IsLocal = true;
        }

        public static Project Create(string path, string title)
        {
            return new Project
            {
                ID = Guid.NewGuid(),
                Location = path,
                Title = title,
            };
        }

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
            var location = Location;
            var list = new List<FileItem>();

            /*if (Directory.Exists(location))
            {
                var allowedDirs = GetAllowedFolders();
                //
                var gitIgnores = GitIgnoreReader.Find(location, SearchOption.AllDirectories).Select(p => GitIgnoreReader.Load(p));
                var queue = new Queue<KeyValuePair<string, IEnumerable<GitIgnoreReader>>>();

                var dirsCount = 0;

                GetFiles(location);

                void GetFiles(string dir)
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
                        var isMatch = relevantGitFiles.All(r => r.IsMatch(sub));

                        if (isMatch)
                        {
                            GetFiles(sub);
                        }
                    }
                }

                while (queue.Count > 0)
                {
                    var pair = queue.Dequeue();
                    var isMatch = pair.Value.All(r => r.IsMatch(pair.Key));
                    list.Add(new FileItem(pair.Key, isMatch));

                    onProgress?.Invoke(queue.Count, 0, pair.Key);
                }
            }*/

            return list;
        }

        /*public bool IsIgnoredForder(string path)
        {
            return IgnoredFolders?.Any(f => GitIgnoreReader.IsChildedPath(PathIO.Combine(Path, f), path)) ?? false;
        }*/

        public List<string> GetAllowedFolders()
        {
            if (Folders?.Count == 0)
            {
                return new List<string>(Directory.GetDirectories(Location));
            }
            else
            {
                var dirs = Folders
                    .Select(folder => Path.Combine(Location, folder))
                    .Where(dir => Directory.Exists(dir));

                return dirs.ToList();
            }
        }

        //
        static Random _random;

        private string RandomizeColor()
        {
            _random ??= new Random(Guid.NewGuid().GetHashCode());

            var rgb = _random.Next(0, 0xFFFFFF);
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
        public string SourceLinesText
            => Volume + (Errors.Count > 0 ? $" ({Errors.Count} errors)" : "");
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

        public CodeVolume(int sloc, int lines, int files)
        {
            SLOC = sloc;
            Lines = lines;
            Files = files;
        }

        public override string ToString() => $"{SLOC}/{Lines}";

        public double Ratio
            => Lines != 0 ? Math.Round((double)SLOC / Lines, 4) : 1;

        public static CodeVolume operator +(CodeVolume a, CodeVolume b)
            => new(a.SLOC + b.SLOC, a.Lines + b.Lines, a.Files + b.Files);

        public static CodeVolume operator -(CodeVolume a, CodeVolume b)
            => new(a.SLOC - b.SLOC, a.Lines - b.Lines, a.Files - b.Files);
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
