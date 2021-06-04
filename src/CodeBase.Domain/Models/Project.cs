using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace CodeBase.Domain.Models
{
    public class Project
    {
        public Guid ID { get; set; }
        public string Location { get; set; }
        public string Title { get; set; }
        public string Color { get; set; }
        public List<string> AllowedFolders { get; set; }
        public List<string> ExcludedFolders { get; set; }
        public bool IsPublic { get; set; }
        public bool IsLocal { get; set; }
        public bool IsTitleHidden { get; set; }
        public DateTime LastRevision { get; set; }
        public ProjectInfo Info { get; set; }

        public string TitleText => GetTitle();
        public Brush BrushColor => GetBrush();

        static Random _random;

        public Project()
        {
            Color = RandomizeColor();
            AllowedFolders = new List<string>();
            ExcludedFolders = new List<string>();
            Info = new ProjectInfo();
            IsPublic = false;
            IsTitleHidden = false;
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
                IsTitleHidden ? " [hidden]" : "");
        }

        public Brush GetBrush()
        {
            var color = (Color)ColorConverter.ConvertFromString(Color ?? RandomizeColor());
            return new SolidColorBrush(color);
        }

        private string RandomizeColor()
        {
            _random ??= new Random(Guid.NewGuid().GetHashCode());

            var rgb = _random.Next(0x000000, 0xFFFFFF);
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
