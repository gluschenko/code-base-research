using System.Collections.Generic;

namespace CodeBase
{
    public class InspectorConfig
    {
        public static readonly List<string> CodeExtensions, FilesBlackList;

        static InspectorConfig() 
        {
            CodeExtensions = List(
                // Backend
                ".php", ".py", ".go", ".htaccess",
                // Frontend & UI
                ".html", ".htm", ".css", ".scss", ".xaml", ".cshtml",
                // JS
                ".js", ".jsx", ".ts", ".tsx",
                // .NET
                ".cs", ".vb",
                // Java
                ".java", ".kt",
                // Low-level
                ".c", ".h", ".cpp", ".hpp", ".s",
                // Shaders
                ".vs", ".fs", ".shader"
            );

            FilesBlackList = List(".i.g.cs", ".g.i.cs", ".i.cs", ".g.cs", ".Designer.cs", "AssemblyInfo.cs");
        }

        static List<T> List<T>(params T[] list) => new List<T>(list); 
    }
}
