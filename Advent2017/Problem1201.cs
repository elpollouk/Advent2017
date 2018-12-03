using Utils.DataStructures;
using Utils;
using FluentAssertions;
using Xunit;

namespace Advent2017
{
    public class Problem1201
    {
        Graph<int> BuildGraph(string datafile)
        {
            var graph = new Graph<int>();
            FileIterator.ForEachLine<string>(datafile, line =>
            {
                var tokens = line.Replace(",", "").Split(' ');
                var id = int.Parse(tokens[0]);
                graph.AddNodeIfNotInGraph(id);

                for (var i = 2; i < tokens.Length; i++)
                {
                    var linkId = int.Parse(tokens[i]);
                    graph.AddNodeIfNotInGraph(linkId);
                    graph.AddTwoWayLink(id, linkId);
                }
            });

            return graph;
        }

        [Theory]
        [InlineData("Data/1201-example.txt", 6)]
        [InlineData("Data/1201.txt", 380)]
        public void Part1(string datafile, int answer) => BuildGraph(datafile).CountGroupSize(0).Should().Be(answer);

        [Theory]
        [InlineData("Data/1201-example.txt", 2)]
        [InlineData("Data/1201.txt", 181)]
        public void Part2(string datafile, int answer) => BuildGraph(datafile).NumberOfGroups.Should().Be(answer);
    }
}
