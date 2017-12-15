﻿using Adevent2017.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Adevent2017.DataStructures
{
    public class Graph<T>
    {
        private class GraphNode
        {
            public GraphNode(T item) { Item = item; }

            public GraphNode Parent = null;
            public readonly List<T> Links = new List<T>();
            public readonly T Item;
        }

        readonly Dictionary<T, GraphNode> _nodes = new Dictionary<T, GraphNode>();

        public Graph()
        {

        }

        public IEnumerable<T> Items => _nodes.Keys;
        public int Size => _nodes.Count;
        public bool Contains(T item) => _nodes.ContainsKey(item);

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

        private GraphNode GetNode(T item)
        {
            return _nodes[item];
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

        public bool IsLinked(T fromItem, T toItem)
        {
            var node = GetNode(fromItem);
            return node.Links.Contains(toItem);
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

        public void DepthFirstWalk(T fromItem, Action<T> onItem)
        {
            var seen = new HashSet<T>();
            try
            {
                InternalDepthFirstWalk(fromItem, onItem, seen);
            }
            catch (StopIteration) { }
        }

        private void InternalDepthFirstWalk(T fromItem, Action<T> onItem, HashSet<T> seen)
        {
            if (seen.Contains(fromItem)) return;
            seen.Add(fromItem);

            var node = GetNode(fromItem);
            onItem(node.Item);

            foreach (var linkId in node.Links)
                InternalDepthFirstWalk(linkId, onItem, seen);
        }

        private void BreadthFirstWalk(T fromItem, Action<T> onNode)
        {
            var seen = new HashSet<T>();
            var walkQueue = new Queue<T>();
            walkQueue.Enqueue(fromItem);

            try
            {
                while (walkQueue.Count != 0)
                {
                    var nodeItem = walkQueue.Dequeue();
                    if (seen.Contains(nodeItem)) continue;

                    onNode(nodeItem);
                    var node = GetNode(nodeItem);

                    foreach (var linked in node.Links)
                        walkQueue.Enqueue(linked);
                }
            }
            catch (StopIteration) { }
        }

        public int CountGroupSize(T fromItem)
        {
            var total = 0;
            DepthFirstWalk(fromItem, node => total++);
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
                        InternalDepthFirstWalk(item, n => { }, seen);
                    }
                }

                return total;
            }
        }
    }
}
