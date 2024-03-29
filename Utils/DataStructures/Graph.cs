﻿using System;
using System.Collections.Generic;

namespace Utils.DataStructures
{
    public class Graph<T>
    {
        public class GraphNode
        {
            public GraphNode(T item) { Item = item; }

            public GraphNode Parent = null;
            public readonly List<T> Links = new List<T>();
            public readonly T Item;
        }

        readonly Dictionary<T, GraphNode> _nodes = new Dictionary<T, GraphNode>();
        readonly EqualityComparer<T> _comparer = EqualityComparer<T>.Default;

        public IEnumerable<T> Items => _nodes.Keys;
        public int Size => _nodes.Count;
        public bool Contains(T item) => _nodes.ContainsKey(item);
        public T Root
        {
            get;
            set;
        }

        public Graph()
        {

        }

        private bool ItemEquals(T a, T b) => _comparer.Equals(a, b);

        private GraphNode GetNode(T item) => _nodes[item];

        public void AddNode(T item)
        {
            if (_nodes.ContainsKey(item)) throw new InvalidOperationException($"Node {item} is already in the graph");
            var node = new GraphNode(item);
            _nodes[item] = node;
        }

        public bool AddNodeIfNotInGraph(T item)
        {
            if (_nodes.ContainsKey(item))
                return false;

            AddNode(item);
            return true;
        }

        public void RemoveNode(T item)
        {
            var node = GetNode(item);

            foreach (var link in node.Links)
            {
                var linkedNode = GetNode(link);
                linkedNode.Links.Remove(item);
                if (linkedNode.Parent == node)
                    linkedNode.Parent = null;
            }

            _nodes.Remove(item);
        }

        public void RemoveOneWayLink(T fromItem, T toItem)
        {
            var from = GetNode(fromItem);
            if (!Contains(toItem)) throw new InvalidOperationException($"Item {toItem} is not in the graph");
            from.Links.Remove(toItem);
        }

        public void RemoveTwoWayLink(T fromItem, T toItem)
        {
            RemoveOneWayLink(fromItem, toItem);
            RemoveOneWayLink(toItem, fromItem);
        }

        public void AddOneWayLink(T fromItem, T toItem)
        {
            var fromNode = GetNode(fromItem);
            if (!Contains(toItem)) throw new InvalidOperationException($"Item {toItem} is not in the graph");
            if (!fromNode.Links.Contains(toItem))
                fromNode.Links.Add(toItem);
        }

        public void AddTwoWayLink(T fromItem, T toItem)
        {
            AddOneWayLink(fromItem, toItem);
            AddOneWayLink(toItem, fromItem);
        }

        public void AddParentChildLink(T parentItem, T childItem)
        {
            AddOneWayLink(parentItem, childItem);
            var parentNode = GetNode(parentItem);
            var childNode = GetNode(childItem);
            childNode.Parent = parentNode;
        }

        public void RemoveParentChildLink(T parentItem, T childItem)
        {
            var childNode = GetNode(childItem);
            if (childNode.Parent == null || !ItemEquals(parentItem, childNode.Parent.Item)) throw new InvalidOperationException($"{parentItem} is not the parent of {childItem}");
            RemoveOneWayLink(parentItem, childItem);
            childNode.Parent = null;
        }

        public bool IsLinked(T fromItem, T toItem)
        {
            var node = GetNode(fromItem);
            return node.Links.Contains(toItem);
        }

        public int GetNumLinks(T fromItem)
        {
            return GetNode(fromItem).Links.Count;
        }

        public T GetLinkedItem(T fromItem, int index)
        {
            var node = GetNode(fromItem);
            return node.Links[index];
        }

        public IEnumerable<T> GetLinked(T fromItem)
        {
            var node = GetNode(fromItem);
            foreach (var linkItem in node.Links)
                yield return linkItem;
        }

        public IEnumerable<T> GetChildren(T parentItem)
        {
            var parentNode = GetNode(parentItem);
            foreach (var childItem in parentNode.Links)
            {
                var childNode = GetNode(childItem);
                if (childNode.Parent == parentNode)
                    yield return childItem;
            }
        }

        public T GetParent(T childItem)
        {
            var parentNode = GetNode(childItem).Parent;
            if (parentNode == null)
                return default(T);
            else
                return parentNode.Item;
        }

        public T GetRoot(T childItem)
        {
            var node = GetNode(childItem);
            while (true)
            {
                if (node.Parent == null)
                    break;

                node = node.Parent;
            }

            return node.Item;
        }

        [Obsolete]
        public void DepthFirstWalk(Action<T> onItem) => DepthFirstWalk(Root, onItem);

        [Obsolete]
        public void DepthFirstWalk(T fromItem, Action<T> onItem)
        {
            var seen = new HashSet<T>();
            try
            {
                foreach (var item in InternalDepthFirstWalk(fromItem, seen))
                    onItem(item);
            }
            catch (StopIteration) { }
        }

        public IEnumerable<T> DepthFirstWalk() => DepthFirstWalk(Root);

        public IEnumerable<T> DepthFirstWalk(T fromItem)
        {
            var seen = new HashSet<T>();
            return InternalDepthFirstWalk(fromItem, seen);
        }

        private IEnumerable<T> InternalDepthFirstWalk(T fromItem, HashSet<T> seen)
        {
            if (seen.Contains(fromItem)) yield break;
            seen.Add(fromItem);

            var node = GetNode(fromItem);
            yield return node.Item;

            foreach (var linkId in node.Links)
                foreach (var item in InternalDepthFirstWalk(linkId, seen))
                    yield return item;
        }

        [Obsolete]
        public void BreadthFirstWalk(Action<T> onItem) => BreadthFirstWalk(Root, onItem);

        [Obsolete]
        public void BreadthFirstWalk(T fromItem, Action<T> onItem)
        {
            try
            {
                foreach (var item in BreadthFirstWalk(fromItem))
                    onItem(item);
            }
            catch (StopIteration) { }
        }

        public IEnumerable<T> BreadthFirstWalk() => BreadthFirstWalk(Root);

        public IEnumerable<T> BreadthFirstWalk(T fromItem)
        {
            var seen = new HashSet<T>();
            var walkQueue = new Queue<T>();
            walkQueue.Enqueue(fromItem);

            while (walkQueue.Count != 0)
            {
                var nodeItem = walkQueue.Dequeue();
                if (seen.Contains(nodeItem)) continue;

                yield return nodeItem;
                var node = GetNode(nodeItem);

                foreach (var linked in node.Links)
                    walkQueue.Enqueue(linked);
            }
        }

        public int CountGroupSize(T fromItem)
        {
            var total = 0;
            foreach (var _ in DepthFirstWalk(fromItem))
                total++;
            return total;
        }

        public int NumberOfGroups
        {
            get
            {
                var total = 0;
                var seen = new HashSet<T>();
                foreach (var item in Items)
                {
                    if (!seen.Contains(item))
                    {
                        total++;
                        foreach (var _ in InternalDepthFirstWalk(item, seen)) { }
                    }
                }

                return total;
            }
        }
    }

    public class IndexedGraph<IndexKeyType, NodeType> : Graph<NodeType>
    {
        private readonly Dictionary<IndexKeyType, NodeType> _Index = new Dictionary<IndexKeyType, NodeType>();

        public NodeType this[IndexKeyType key]
        {
            set
            {
                _Index[key] = value;
                AddNode(value);
            }
            get => _Index[key];
        }

        public void AddParentChildLink(IndexKeyType parent, IndexKeyType child) => AddParentChildLink(_Index[parent], _Index[child]);
    }
}
