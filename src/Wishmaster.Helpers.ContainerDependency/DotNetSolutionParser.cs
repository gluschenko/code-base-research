using System;
using System.Collections.Generic;
using System.Linq;
using Wishmaster.Helpers.ContainerDependency.Models;

namespace Wishmaster.Helpers.ContainerDependency
{
    public class DotNetSolutionParser
    {
        private readonly IEnumerable<string> _solutionFiles;

        public DotNetSolutionParser(IEnumerable<string> solutionFiles)
        {
            _solutionFiles = solutionFiles;
        }

        public Graph<NodeData, EdgeData> Parse()
        {
            return new Graph<NodeData, EdgeData>();
        }

        private void ParseProjectFile()
        {

        }

        /// <summary>
        /// Example: /abc/cde/fgh/../../xyz => /abc/xyz
        /// </summary>
        private string NormalizePath(string path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var words = path.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries).Reverse().ToArray();
            var list = new List<string>();
            var m = 0;

            foreach (var word in words)
            {
                if (word.Trim() == "..")
                {
                    m++;
                    continue;
                }

                if (word.Trim() == ".")
                {
                    continue;
                }

                if (m == 0)
                {
                    list.Add(word);
                }
                else
                {
                    m--;
                }
            }

            return string.Join("/", list.ToArray().Reverse());
        }
    }

    public record NodeData(
        string Project,
        string Solution,
        string Repository,
        string PackageId,
        string Platform,
        string LangVersion,
        int Size,
        string Color
    );

    public record EdgeData(EdgeType Type, string Text);

    public enum EdgeType
    {
        PackageReference = 1,
        ProjectReference = 2,
    }
}
