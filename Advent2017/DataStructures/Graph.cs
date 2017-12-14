using Adevent2017.Utils;
using System;
using System.Collections.Generic;

namespace Adevent2017.DataStructures
{
    public class Graph<T>
    {
        readonly Dictionary<int, GraphNode<T>> _nodes = new Dictionary<int, GraphNode<T>>();

        public Graph()
        {

        }

        public IEnumerable<GraphNode<T>> Nodes => _nodes.Values;
        public int Size => _nodes.Count;
        public bool Contains(int id) => _nodes.ContainsKey(id);
        public bool TryGetNode(int id, out GraphNode<T> node) =>_nodes.TryGetValue(id, out node);

        public GraphNode<T> CreateNode(int id)
        {
            if (_nodes.ContainsKey(id)) throw new InvalidOperationException($"Node {id} is already in the graph");
            var node = new GraphNode<T>(id);
            _nodes[id] = node;
            return node;
        }

        public GraphNode<T> GetNode(int id)
        {
            return _nodes[id];
        }

        public GraphNode<T> GetOrCreateNode(int id)
        {
            GraphNode<T> node;
            if (_nodes.TryGetValue(id, out node))
                return node;

            return CreateNode(id);
        }

        public GraphNode<T> RemoveNode(int id)
        {
            var node = GetNode(id);

            foreach (var link in node.Links)
            {
                var linkedNode = GetNode(link);
                linkedNode.Links.Remove(id);
                if (linkedNode.Parent == node)
                    linkedNode.Parent = null;
            }

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

        public void AddParentChildLink(int parentId, int childId)
        {
            AddOneWayLink(parentId, childId);
            var parentNode = GetNode(parentId);
            var childNode = GetNode(childId);
            childNode.Parent = parentNode;
        }

        public bool IsLinked(int fromId, int toId)
        {
            var node = GetNode(fromId);
            return node.Links.Contains(toId);
        }

        public IEnumerable<GraphNode<T>> GetLinked(int fromId)
        {
            var linked = new List<GraphNode<T>>();
            var node = GetNode(fromId);
            foreach (var linkId in node.Links)
                linked.Add(GetNode(linkId));
            
            return linked;
        }

        public IEnumerable<GraphNode<T>> GetChildren(int parentId)
        {
            var children = new List<GraphNode<T>>();
            var parent = GetNode(parentId);
            foreach (var childId in parent.Links)
            {
                var child = GetNode(childId);
                if (child.Parent == parent)
                    children.Add(child);
            }
            return children;
        }

        public GraphNode<T> GetParent(int childId)
        {
            return GetNode(childId).Parent;
        }

        public void DepthFirstWalk(int fromId, Action<GraphNode<T>> onNode)
        {
            var seen = new HashSet<int>();
            try
            {
                InternalDepthFirstWalk(fromId, onNode, seen);
            }
            catch (StopIteration) { }
        }

        private void InternalDepthFirstWalk(int fromId, Action<GraphNode<T>> onNode, HashSet<int> seen)
        {
            if (seen.Contains(fromId)) return;
            seen.Add(fromId);

            var node = GetNode(fromId);
            onNode(node);

            foreach (var linkId in node.Links)
                InternalDepthFirstWalk(linkId, onNode, seen);
        }

        private void BreadthFirstWalk(int fromId, Action<GraphNode<T>> onNode)
        {
            var seen = new HashSet<int>();
            var walkQueue = new Queue<int>();
            walkQueue.Enqueue(fromId);

            try
            {
                while (walkQueue.Count != 0)
                {
                    var nodeId = walkQueue.Dequeue();
                    if (seen.Contains(nodeId)) continue;

                    var node = GetNode(nodeId);
                    onNode(node);

                    foreach (var linkId in node.Links)
                        walkQueue.Enqueue(linkId);
                }
            }
            catch (StopIteration) { }
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

    public class Graph : Graph<object> { }
}
