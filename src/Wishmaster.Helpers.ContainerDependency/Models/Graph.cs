using System;
using System.Collections.Generic;
using System.Linq;

namespace Wishmaster.Helpers.ContainerDependency.Models
{
    public class Graph<TNode, TEdge>
    {
        public Guid Uid { get; private set; }
        private readonly Dictionary<NodeIdentity, Node<TNode>> _nodes;
        private readonly Dictionary<EdgeIdentity, Edge<TEdge>> _edges;

        public Graph()
        {
            Uid = Guid.NewGuid();
            _nodes = new Dictionary<NodeIdentity, Node<TNode>>();
            _edges = new Dictionary<EdgeIdentity, Edge<TEdge>>();
        }

        public Node<TNode> AddNode(TNode value)
        {
            return AddNode(new Node<TNode>(value));
        }

        public Node<TNode> AddNode(Node<TNode> node)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var identity = new NodeIdentity(node.Uid);

            if (_nodes.ContainsKey(identity))
            {
                throw new Exception($"Node {node.Uid} is already in Graph");
            }

            _nodes[identity] = node;
            return node;
        }

        public void RemoveNode(Node<TNode> node)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var identity = new NodeIdentity(node.Uid);

            if (!_nodes.ContainsKey(identity))
            {
                throw new Exception($"Node {node.Uid} is not found");
            }

            var edges = _edges
                .Where(x => x.Key.From == node.Uid || x.Key.To == node.Uid)
                .Select(x => x.Key)
                .ToArray();

            foreach (var edge in edges)
            {
                RemoveEdge(edge.From, edge.To);
            }

            _nodes.Remove(identity);
        }

        public void SetEdge(Node<TNode> from, Node<TNode> to, TEdge value)
        {
            if (from is null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (to is null)
            {
                throw new ArgumentNullException(nameof(to));
            }

            SetEdge(from.Uid, to.Uid, value);
        }

        public void SetEdge(Guid from, Guid to, TEdge value)
        {
            var identity = new EdgeIdentity(from, to);
            _edges[identity] = new Edge<TEdge>(value, from, to);
        }

        public void RemoveEdge(Node<TNode> from, Node<TNode> to)
        {
            if (from is null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (to is null)
            {
                throw new ArgumentNullException(nameof(to));
            }

            RemoveEdge(from.Uid, to.Uid);
        }

        public void RemoveEdge(Guid from, Guid to)
        {
            var identity = new EdgeIdentity(from, to);

            if (!_edges.ContainsKey(identity))
            {
                throw new Exception($"Edge {identity} is not found");
            }

            _edges.Remove(identity);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();

            foreach (var item in _nodes.Values)
            {
                hash.Add(item);
            }

            foreach (var item in _edges.Values)
            {
                hash.Add(item);
            }

            return hash.ToHashCode();
        }
    }

    internal record struct NodeIdentity(Guid Uid)
    {
        public static implicit operator NodeIdentity(Guid uid)
        {
            return new NodeIdentity(uid);
        }

        public static implicit operator Guid(NodeIdentity id)
        {
            return id.Uid;
        }
    }

    internal record struct EdgeIdentity(Guid From, Guid To);
}
