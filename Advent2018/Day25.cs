using FluentAssertions;
using System;
using System.Linq;
using Utils;
using Utils.DataStructures;
using Xunit;

namespace Advent2018
{
    public class Day25
    {
        bool IsInRange((int x, int y, int z, int w) a, (int x, int y, int z, int w) b)
        {
            var distance = Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y)  + Math.Abs(a.z - b.z) + Math.Abs(a.w - b.w);
            return distance <= 3;
        }

        Graph<(int x, int y, int z, int w)> LoadGraph(string inputfile)
        {
            var graph = new Graph<(int x, int y, int z, int w)>();

            foreach (var line in FileIterator.Lines(inputfile))
            {
                var split = line.Split(',');
                graph.AddNode((
                    int.Parse(split[0].Trim()),
                    int.Parse(split[1].Trim()),
                    int.Parse(split[2].Trim()),
                    int.Parse(split[3].Trim())
                ));
            }

            var nodes = graph.Items.ToArray();
            for (var i = 0; i < nodes.Length - 1; i++)
            {
                for (var j = i + 1; j < nodes.Length; j++)
                {
                    if (IsInRange(nodes[i], nodes[j]))
                    {
                        graph.AddTwoWayLink(nodes[i], nodes[j]);
                    }
                }
            }

            return graph;
        }

        [Theory]
        [InlineData(2, "Data/Day25-Test1.txt")]
        [InlineData(4, "Data/Day25-Test2.txt")]
        [InlineData(3, "Data/Day25-Test3.txt")]
        [InlineData(8, "Data/Day25-Test4.txt")]
        [InlineData(377, "Data/Day25.txt")] // Solution
        void Problem(int expectedGroups, string inputFile)
        {
            var graph = LoadGraph(inputFile);
            graph.NumberOfGroups.Should().Be(expectedGroups);
        }
    }
}
