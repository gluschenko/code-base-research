using System.Collections.Generic;

namespace CodeBase.Core
{
    public class InspectorConfig
    {
        public static readonly HashSet<string> CodeExtensions, FilesBlackList;

        static InspectorConfig()
        {
            CodeExtensions = List(
                // Backend
                ".php", ".py", ".go", ".htaccess",
                // Frontend & UI
                ".html", ".htm", ".css", ".scss", ".xaml", ".cshtml", ".razor",
                // JS
                ".js", ".jsx", ".ts", ".tsx",
                // .NET
                ".cs", ".vb",
                // Java
                ".java", ".kt", ".dart",
                // Low-level
                ".c", ".h", ".cpp", ".hpp", ".s",
                // Shaders
                ".vs", ".fs", ".shader"
            );

            FilesBlackList = List(".i.g.cs", ".g.i.cs", ".i.cs", ".g.cs", ".Designer.cs", "AssemblyInfo.cs");
        }

        static HashSet<T> List<T>(params T[] list) => new HashSet<T>(list);
    }
}
