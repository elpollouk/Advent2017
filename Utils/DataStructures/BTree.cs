using System;
using System.Collections.Generic;

namespace Utils.DataStructures
{
    class BTree<Item>
    {
        class Node
        {
            public List<int> Keys = new List<int>();
            public List<Item> Items = new List<Item>();

            public Node Parent = null;
            public List<Node> Children = new List<Node>();

            public override string ToString()
            {
                var value = "";
                foreach (var key in Keys)
                    value += $"{key}, ";
                return value;
            }
        }

        private Node _root = null;

        public void Insert(int key, Item item)
        {
            if (_root == null)
            {
                _root = new Node();
                _root.Keys.Add(key);
                _root.Items.Add(item);
                return;
            }

            InsertInternal(_root, key, item);
        }

        private void InsertInternal(Node node, int key, Item item)
        {
            // Need to find the right child to recurse into
            if (node.Children.Count == 0)
            {
                InsertIntoNode(node, key, item, null, null);
            }
            else
            {
                var i = 0;
                while (i < node.Keys.Count)
                {
                    if (key < node.Keys[i])
                    {
                        InsertInternal(node.Children[i], key, item);
                        return;
                    }
                    i++;
                }
                InsertInternal(node.Children[i], key, item);
            }
        }

        private void InsertIntoNode(Node node, int key, Item item, Node leftChild, Node rightChild)
        {
            var insertIndex = 0;
            while (insertIndex < node.Keys.Count)
            {
                if (key < node.Keys[insertIndex])
                    break;
                insertIndex++;
            }

            node.Keys.Insert(insertIndex, key);
            node.Items.Insert(insertIndex, item);

            if (leftChild != null)
            {
                node.Children.Insert(insertIndex, rightChild);
                node.Children.Insert(insertIndex, leftChild);
                rightChild.Parent = node;
                leftChild.Parent = node;
            }

            if (node.Keys.Count == 3)
            {
                // Need to Split and promote key
                var newRightNode = new Node();
                newRightNode.Keys.Add(node.Keys[2]);
                newRightNode.Items.Add(node.Items[2]);
                var midKey = node.Keys[1];
                var midItem = node.Items[1];
                node.Keys.RemoveRange(1, 2);
                node.Items.RemoveRange(1, 2);

                if (node.Children.Count != 0)
                {
                    newRightNode.Children.Add(node.Children[2]);
                    newRightNode.Children.Add(node.Children[3]);
                    newRightNode.Children[0].Parent = newRightNode;
                    newRightNode.Children[1].Parent = newRightNode;
                    node.Children.RemoveRange(2, 2);
                }

                var parent = node.Parent;
                if (parent == null)
                {
                    parent = new Node();
                    _root = parent;
                }
                else
                {
                    parent.Children.Remove(node);
                }

                InsertIntoNode(parent, midKey, midItem, node, newRightNode);
            }
        }

        public Item Get(int key) => GetInternal(_root, key);

        private Item GetInternal(Node node, int key)
        {
            var hasChildren = node.Children.Count != 0;
            var i = 0;
            while (i < node.Keys.Count)
            {
                if (node.Keys[i] == key)
                    return node.Items[i];

                if (hasChildren && key < node.Keys[i])
                    return GetInternal(node.Children[i], key);

                i++;
            }

            if (hasChildren)
                return GetInternal(node.Children[i], key);

            return default(Item);
        }

        public void VerifyTree() => VerifyNode(_root);

        private void VerifyNode(Node node)
        {
            if (node.Parent == null)
            {
                if (node != _root)
                    throw new Exception("Node has no parent but is not the root");
            }
            else
            {
                if (!node.Parent.Children.Contains(node))
                    throw new Exception("Node is not a child of it's parent");
            }

            if (node.Keys.Count > 2)
                throw new Exception("Node has too many keys");

            if (node.Keys.Count == 2)
                if (node.Keys[1] < node.Keys[0])
                    throw new Exception("Keys are in wrong order");

            if (node.Items.Count != node.Keys.Count)
                throw new Exception("Node has the wrong number of items");

            if (node.Children.Count != 0)
            {
                if (node.Children.Count != node.Keys.Count + 1)
                    throw new Exception("Node has wrong number of children");

                foreach (var child in node.Children)
                    VerifyNode(child);
            }
        }
    }
}
