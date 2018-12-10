using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
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

        Node ParseStructure(Func<int> input)
        {
            var node = new Node();
            var numChildren = input();
            var numMetaData = input();
            for (var i = 0; i < numChildren; i++)
                node.Children.Add(ParseStructure(input));

            for (var i = 0; i < numMetaData; i++)
                node.MetaData.Add(input());

            return node;
        }

        int SumTree_Problem1(Node root)
        {
            var sum = root.MetaData.Sum();
            sum += root.Children.Select(c => SumTree_Problem1(c)).Sum();
            return sum;
        }

        int SumTree_Problem2(Node root)
        {
            if (root.Children.Count() != 0)
            {
                var sum = 0;
                foreach (var index in root.MetaData)
                {
                    var i = index - 1; // Indexes are 1 based not 0 based!
                    if (i < 0 || root.Children.Count() <= i)
                        continue;
                    sum += SumTree_Problem2(root.Children[i]);
                }
                return sum;
            }
            else
            {
                return root.MetaData.Sum();
            }
        }

        [Theory]
        [InlineData(138, 66, "Data/Day08-Test.txt")]
        [InlineData(43825, 19276, "Data/Day08.txt")]
        public void Problem_Test(int answer1, int answer2, string inputFile)
        {
            var values = FileIterator.LoadSSV<int>(inputFile).Reader();
            var root = ParseStructure(values);

            SumTree_Problem1(root).Should().Be(answer1);
            SumTree_Problem2(root).Should().Be(answer2);
        }
    }
}
