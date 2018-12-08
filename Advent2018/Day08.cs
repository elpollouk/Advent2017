using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Advent2018
{
    public class Day08
    {
        class Node
        {
            public readonly List<Node> Children = new List<Node>();
            public readonly List<int> MetaData = new List<int>();
        };

        Node ParseStructure(int[] input, ref int offset)
        {
            var node = new Node();
            var numChildren = input[offset++];
            var numMetaData = input[offset++];
            for (var i = 0; i < numChildren; i++)
                node.Children.Add(ParseStructure(input, ref offset));

            for (var i = 0; i < numMetaData; i++)
                node.MetaData.Add(input[offset++]);

            return node;
        }

        int SumTree(Node root)
        {
            var sum = root.MetaData.Sum();
            sum += root.Children.Select(c => SumTree(c)).Sum();
            return sum;
        }

        [Theory]
        [InlineData(138, "Data/Day08-Test.txt")]
        [InlineData(43825, "Data/Day08.txt")]
        public void Problem1_Test(int answer, string inputFile)
        {
            var values = Utils.FileIterator.LoadSSV<int>(inputFile);
            int offset = 0;
            var root = ParseStructure(values, ref offset);
            offset.Should().Be(values.Length);

            SumTree(root).Should().Be(answer);
        }
    }
}
