using FluentAssertions;
using System;
using Xunit;

namespace Utils.DataStructures
{
    public class BTreeTests
    {
        [Theory]
        [InlineData(1, 2, 3, 4, 5, 6, 7, 8, 9)]
        [InlineData(9, 8, 7, 6, 5, 4, 3, 2, 1)]
        [InlineData(5, 4, 3, 7, 6, 1, 9, 2, 8)]
        [InlineData(3, 9, 5, 8, 1, 2, 6, 7, 4)]
        [InlineData(9)]
        [InlineData(7, 6)]
        [InlineData(3, 1, 2)]
        [InlineData(5, 3, 7, 1)]
        [InlineData(4, 6, 3, 9, 2)]
        [InlineData(9, 6, 8, 3, 1, 5)]
        public void Insert(params int[] keys)
        {
            Func<int, string> item = (key) => " ABCDEFGHI".Substring(key, 1);

            var tree = new BTree<string>();

            foreach (var number in keys)
            {
                tree.Insert(number, item(number));
                tree.VerifyTree();
            }

            foreach (var number in keys)
                tree.Get(number).Should().Be(item(number));

            tree.Get(-1).Should().BeNull();
        }
    }
}
