using FluentAssertions;
using Xunit;

namespace Adevent2017.DataStructures
{
    public class RedBlackTreeTests
    {
        static readonly RedBlackTree<int, string>.Node Leaf = null;

        void VerifyNode(RedBlackTree<int, string>.Node node, string value, string parentValue, string leftValue, string rightValue)
        {
            node.Value.Should().Be(value);

            if (parentValue != null)
                node.Parent.Value.Should().Be(parentValue);
            else
                node.Parent.Should().BeNull();

            if (leftValue != null)
                node.Left.Value.Should().Be(leftValue);
            else
                node.Left.Should().Be(Leaf);

            if (rightValue != null)
                node.Right.Value.Should().Be(rightValue);
            else
                node.Right.Should().Be(Leaf);
        }

        [Fact]
        public void Insert()
        {
            var tree = new RedBlackTree<int, string>();
            tree.Insert(5, "A");
            VerifyNode(tree._root, "A", null, null, null);

            tree.Insert(4, "B");
            VerifyNode(tree._root, "A", null, "B", null);
            VerifyNode(tree._root.Left, "B", "A", null, null);

            tree.Insert(3, "C");
            VerifyNode(tree._root, "B", null, "C", "A");
            VerifyNode(tree._root.Left, "C", "B", null, null);
            VerifyNode(tree._root.Right, "A", "B", null, null);

            tree.Insert(2, "D");
            VerifyNode(tree._root, "B", null, "C", "A");
            VerifyNode(tree._root.Left, "C", "B", "D", null);
            VerifyNode(tree._root.Right, "A", "B", null, null);
            VerifyNode(tree._root.Left.Left, "D", "C", null, null);

            tree.Insert(1, "E");
            VerifyNode(tree._root, "B", null, "D", "A");
            VerifyNode(tree._root.Left, "D", "B", "E", "C");
            VerifyNode(tree._root.Right, "A", "B", null, null);
            VerifyNode(tree._root.Left.Left, "E", "D", null, null);
            VerifyNode(tree._root.Left.Right, "C", "D", null, null);

            tree.Insert(6, "F");
            VerifyNode(tree._root, "B", null, "D", "A");
            VerifyNode(tree._root.Left, "D", "B", "E", "C");
            VerifyNode(tree._root.Right, "A", "B", null, "F");
            VerifyNode(tree._root.Left.Left, "E", "D", null, null);
            VerifyNode(tree._root.Left.Right, "C", "D", null, null);
            VerifyNode(tree._root.Right.Right, "F", "A", null, null);

            tree.Insert(7, "G");
            VerifyNode(tree._root, "B", null, "D", "F");
            VerifyNode(tree._root.Left, "D", "B", "E", "C");
            VerifyNode(tree._root.Right, "F", "B", "A", "G");
            VerifyNode(tree._root.Left.Left, "E", "D", null, null);
            VerifyNode(tree._root.Left.Right, "C", "D", null, null);
            VerifyNode(tree._root.Right.Left, "A", "F", null, null);
            VerifyNode(tree._root.Right.Right, "G", "F", null, null);
        }


        [Fact]
        public void Search()
        {
            var tree = new RedBlackTree<int, string>();
            tree.Insert(5, "A");
            tree.Insert(4, "B");
            tree.Insert(3, "C");
            tree.Insert(2, "D");
            tree.Insert(1, "E");
            tree.Insert(7, "F");
            tree.Insert(6, "G");

            tree.Search(1).Should().Be("E");
            tree.Search(2).Should().Be("D");
            tree.Search(3).Should().Be("C");
            tree.Search(4).Should().Be("B");
            tree.Search(5).Should().Be("A");
            tree.Search(6).Should().Be("G");
            tree.Search(7).Should().Be("F");
            tree.Search(8).Should().BeNull();
        }
    }
}
