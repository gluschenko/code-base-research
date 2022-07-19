using Wishmaster.Helpers.ContainerDependency.Models;

namespace Wishmaster.Helpers.ContainerDependency.Console
{
    public static class Program
    {
        public static int Main()
        {
            var graph = new Graph<NodeData, EdgeData>();

            var a = graph.AddNode(new NodeData("1", "1", "1", "1", "1", "1", 1, "1"));
            var b = graph.AddNode(new NodeData("2", "2", "2", "2", "2", "2", 2, "2"));
            var c = graph.AddNode(new NodeData("3", "3", "3", "3", "3", "3", 3, "3"));

            graph.SetEdge(a, b, new EdgeData(EdgeType.ProjectReference, ""));
            graph.SetEdge(b, c, new EdgeData(EdgeType.ProjectReference, ""));
            graph.SetEdge(c, a, new EdgeData(EdgeType.ProjectReference, ""));

            graph.RemoveNode(b);
            graph.RemoveEdge(c, a);

            return 0;
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
