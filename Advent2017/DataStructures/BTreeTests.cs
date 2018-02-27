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
        public void Insert(params int[] keys)
        {
            var tree = new BTree<string>();

            foreach (var number in keys)
            {
                tree.Insert(number, "A");
                tree.VerifyTree();
            }
        }
    }
}
