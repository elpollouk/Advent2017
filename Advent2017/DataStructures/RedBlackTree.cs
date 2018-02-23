using System;

namespace Adevent2017.DataStructures
{
    enum Colour
    {
        Red,
        Black,
    }

    class RedBlackTree<Key, Value> where Key : IComparable
    { 
        public class Node
        {
            public Node(Key key, Value value)
            {
                this.Key = key;
                this.Value = value;
            }

            public readonly Key Key;
            public readonly Value Value;
            public Colour Colour = Colour.Red;
             
            public Node Parent = null;
            public Node Left = null;
            public Node Right = null;


            public Node Sibling
            {
                get
                {
                    if (Parent == null)
                        return null;
                    if (this == Parent.Left)
                        return Parent.Right;
                    return Parent.Left;
                }
            }

            public Node Uncle
            {
                get
                {
                    if (Parent == null)
                        return null;
                    return Parent.Sibling;
                }
            }

            public Node GrandParent
            {
                get
                {
                    if (Parent == null)
                        return null;
                    return Parent.Parent;
                }
            }

            public void RotateLeft()
            {
                var t = Right;
                Right = t.Left;
                t.Left = this;
                t.Parent = Parent;
                Parent = t;

                if (t.Parent != null)
                {
                    if (t.Parent.Left == this)
                        t.Parent.Left = t;
                    else
                        t.Parent.Right = t;
                }
            }

            public void RotateRight()
            {
                var t = Left;
                Left = t.Right;
                t.Right = this;
                t.Parent = Parent;
                Parent = t;

                if (t.Parent != null)
                {
                    if (t.Parent.Left == this)
                        t.Parent.Left = t;
                    else
                        t.Parent.Right = t;
                }
            }

            public override string ToString()
            {
                if (Left == null)
                    return "Leaf";
                return $"Key: {Key}, Value: {Value}";
            }
        }

        public Node _root = null;
        public readonly Node _leaf = new Node(default(Key), default(Value))
        {
            Colour = Colour.Black
        };

        public Value Search(Key key)
        {
            return SearchInternal(_root, key);
        }

        public Value SearchInternal(Node node, Key key)
        {
            if (node == _leaf)
                return default(Value);

            var compareResult = key.CompareTo(node.Key);
            if (compareResult == 0)
                return node.Value;

            if (compareResult < 0)
                return SearchInternal(node.Left, key);

            return SearchInternal(node.Right, key);
        }

        public void Insert(Key key, Value value)
        {
            var node = new Node(key, value)
            {
                Left = _leaf,
                Right = _leaf
            };

            InsertInternal(_root, node);
            Repair(node);

            _root = node;
            while (_root.Parent != null)
                _root = _root.Parent;
        }

        private void InsertInternal(Node parent, Node node)
        {
            if (parent == null) return; // This is the new root

            var compareResult = node.Key.CompareTo(parent.Key);
            if (compareResult < 0)
            {
                if (parent.Left != _leaf)
                {
                    InsertInternal(parent.Left, node);
                }
                else
                {
                    parent.Left = node;
                    node.Parent = parent;
                }
            }
            else
            {
                if (parent.Right != _leaf)
                {
                    InsertInternal(parent.Right, node);
                }
                else
                {
                    parent.Right = node;
                    node.Parent = parent;
                }
            }
        }

        private void Repair(Node node)
        {
            if (node.Parent == null)
            {
                // Root node should always be black
                node.Colour = Colour.Black;
            }
            else if (node.Parent.Colour == Colour.Black)
            {
                // Do nothing
            }
            else if (node.Uncle.Colour == Colour.Red)
            {
                // Parent and uncle are both red, make the colours align with the current node
                node.Parent.Colour = Colour.Black;
                node.Uncle.Colour = Colour.Black;
                node.GrandParent.Colour = Colour.Red;
                Repair(node.GrandParent);
            }
            else
            {
                // ???? - Shuffle unbalanced nodes into a balanced position
                if (node == node.GrandParent.Left.Right)
                {
                    node.Parent.RotateLeft();
                    node = node.Left;
                }
                else if (node == node.GrandParent.Right.Left)
                {
                    node.Parent.RotateRight();
                    node = node.Right;
                }

                // We need to cache these nodes as the result of rotating them will cause them to change their relationship to the original node
                var p = node.Parent;
                var g = node.GrandParent;

                if (node == p.Left)
                    g.RotateRight();
                else
                    g.RotateLeft();

                p.Colour = Colour.Black;
                g.Colour = Colour.Red;
            }
        }
    }
}
