using Adevent2017.Utils;
using System;
using System.Collections.Generic;

namespace Adevent2017.DataStructures
{
    class Graph
    {
        readonly Dictionary<int, GraphNode> _nodes = new Dictionary<int, GraphNode>();

        public Graph()
        {

        }

        public IEnumerable<GraphNode> Nodes => _nodes.Values;
        public int Size => _nodes.Count;
        public bool Contains(int id) => _nodes.ContainsKey(id);
        public bool TryGetNode(int id, out GraphNode node) =>_nodes.TryGetValue(id, out node);

        public GraphNode AddNode(int id)
        {
            if (_nodes.ContainsKey(id)) throw new InvalidOperationException($"Node {id} is already in the graph");
            var node = new GraphNode(id);
            _nodes[id] = node;
            return node;
        }

        public GraphNode GetNode(int id)
        {
            return _nodes[id];
        }

        public GraphNode GetOrCreateNode(int id)
        {
            GraphNode node;
            if (_nodes.TryGetValue(id, out node))
                return node;

            return AddNode(id);
        }

        public GraphNode RemoveNode(int id)
        {
            var node = GetNode(id);

            foreach (var link in node.Links)
                GetNode(link).Links.Remove(id);

            _nodes.Remove(id);

            return node;
        }

        public void AddOneWayLink(int fromId, int toId)
        {
            var fromNode = GetNode(fromId);
            if (!fromNode.Links.Contains(toId))
                fromNode.Links.Add(toId);
        }

        public void AddTwoWayLink(int fromId, int toId)
        {
            AddOneWayLink(fromId, toId);
            AddOneWayLink(toId, fromId);
        }

        public bool IsLink(int fromId, int toId)
        {
            var node = GetNode(fromId);
            return node.Links.Contains(toId);
        }

        public void DepthFirstWalk(int fromId, Action<GraphNode> onNode)
        {
            var seen = new HashSet<int>();
            try
            {
                InternalDepthFirstWalk(fromId, onNode, seen);
            }
            catch (StopIteration) { }
        }

        private void InternalDepthFirstWalk(int fromId, Action<GraphNode> onNode, HashSet<int> seen)
        {
            if (seen.Contains(fromId)) return;
            seen.Add(fromId);

            var node = GetNode(fromId);
            onNode(node);

            foreach (var linkId in node.Links)
                InternalDepthFirstWalk(linkId, onNode, seen);
        }

        public int CountGroupSize(int fromId)
        {
            var total = 0;
            DepthFirstWalk(fromId, node => total++);
            return total;
        }

        public int NumberOfGroups
        {
            get
            {
                var total = 0;
                var seen = new HashSet<int>();
                foreach (var node in Nodes)
                {
                    if (!seen.Contains(node.Id))
                    {
                        total++;
                        InternalDepthFirstWalk(node.Id, n => { }, seen);
                    }
                }

                return total;
            }
        }
    }
}
