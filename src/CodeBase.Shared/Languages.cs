using System.Collections.Generic;

namespace CodeBase.Shared
{
    public static class Languages
    {
        public static IEnumerable<Language> Get() => _languages;

        private readonly static IEnumerable<Language> _languages = new[]
        {
            new Language("C#", "#ff9a00", ".cs"),
            new Language("XAML", "#8b9dc3", ".xaml"),
            new Language("VB", "#ff7400", ".vb"),
            new Language("PHP", "#b4c468", ".php"),
            new Language("Python", "#f4cd2a", ".py"),
            new Language("Go", "#f4cd22", ".go"),
            new Language("HTML", "#ed5555", ".html", ".htm"),
            new Language("Razor", "#ed5500", ".cshtml", ".razor"),
            new Language("CSS", "#59abe3", ".css"),
            new Language("SASS", "#59ab00", ".scss"),
            new Language("JavaScript", "#f7ca18", ".js", ".jsx"),
            new Language("TypeScript", "#f7ca18", ".ts", ".tsx"),
            new Language("Java", "#2e3131", ".java"),
            new Language("Kotlin", "#913d88", ".kt"),
            new Language("C/C++", "#00e640", ".h", ".cpp", ".hpp", ".c"),
            new Language("Assembly", "#67809f", ".s"),
            new Language("HLSL", "#db0a5b", ".shader"),
            new Language("OpenGL","#db0a5b", ".fs", ".vs", ".frag", ".vert", ".glsl"),
        };
    }

    public class Language
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public IEnumerable<string> Extensions { get; set; }

        public Language(string name, string color, params string[] extensions)
        {
            Name = name;
            Color = color;
            Extensions = extensions;
        }
    }
}
