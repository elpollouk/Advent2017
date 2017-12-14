using Adevent2017.DataStructures;
using Adevent2017.Utils;
using FluentAssertions;
using Xunit;

namespace Adevent2017
{
    public class Problem1201
    {
        static Graph BuildGraph(string datafile)
        {
            var graph = new Graph();
            FileIterator.ForEachLine<string>(datafile, line =>
            {
                var tokens = line.Replace(",", "").Split(' ');
                var id = int.Parse(tokens[0]);
                graph.GetOrCreateNode(id);

                for (var i = 2; i < tokens.Length; i++)
                {
                    var linkId = int.Parse(tokens[i]);
                    graph.GetOrCreateNode(linkId);
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
