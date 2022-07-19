using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
