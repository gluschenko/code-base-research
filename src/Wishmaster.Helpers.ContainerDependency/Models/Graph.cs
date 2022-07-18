using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wishmaster.Helpers.ContainerDependency.Models
{
    public class Graph<TNode, TEdge>
    {
        public Guid Uid { get; private set; }
        private readonly List<Node<TNode>> _nodes;
        private readonly List<Edge<TEdge>> _edges;

        public Graph()
        {
            Uid = Guid.NewGuid();
            _nodes = new List<Node<TNode>>();
            _edges = new List<Edge<TEdge>>();
        }

        public void AddNode(Node<TNode> node)
        {

        }

        public void SetEdge()
        {

        }

        public override int GetHashCode()
        {
            var hash = new HashCode();

            _nodes.ForEach(x => hash.Add(x));
            _edges.ForEach(x => hash.Add(x));

            return hash.ToHashCode();
        }
    }
}
