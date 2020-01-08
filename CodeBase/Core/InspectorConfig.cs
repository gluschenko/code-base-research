using System.Collections.Generic;

namespace CodeBase
{
    public class InspectorConfig
    {
        public static readonly List<string> CodeExtensions, FilesBlackList, DirsBlackList;

        static InspectorConfig() 
        {
            CodeExtensions = List(
                //Backend
                ".php", ".py", ".go", ".htaccess",
                // Frontend & UI
                ".html", ".htm", ".css", ".xaml", ".js", ".cshtml",
                // .NET
                ".cs", ".vb",
                // Java
                ".java", ".kt",
                // Low-level
                ".h", ".cpp", ".hpp", ".c", ".s",
                // Chaders
                ".vs", ".fs", ".shader"
            );

            FilesBlackList = List(".i.g.cs", ".g.i.cs", ".i.cs", ".g.cs", ".Designer.cs", "AssemblyInfo.cs");

            DirsBlackList = List(".git", ".vs");
        }

        static List<T> List<T>(params T[] list) => new List<T>(list); 
    }
}
