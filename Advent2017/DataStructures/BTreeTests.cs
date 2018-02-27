using FluentAssertions;
using Xunit;

namespace Adevent2017.DataStructures
{
    public class BTreeTests
    {
        [Theory]
        [InlineData(1, 2, 3, 4, 5, 6, 7, 8, 9)]
        [InlineData(9, 8, 7, 6, 5, 4, 3, 2, 1)]
        [InlineData(5, 4, 3, 7, 6, 1, 9, 2, 8)]
        [InlineData(3, 9, 5, 8, 1, 2, 6, 7, 4)]
        [InlineData(2, 1, 3)]
        [InlineData(5, 3, 7, 1)]
        [InlineData(9)]
        [InlineData(7, 6)]
        public void Insert(params int[] keys)
        {
            var items = " ABCDEFGHI";

            var tree = new BTree<string>();

            foreach (var number in keys)
            {
                tree.Insert(number, items.Substring(number, 1));
                tree.VerifyTree();
            }

            foreach (var number in keys)
                tree.Get(number).Should().Be(items.Substring(number, 1));
        }
    }
}
