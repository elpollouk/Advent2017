using FluentAssertions;
using System.Collections.Generic;
using Utils;
using Xunit;

using Graph = System.Collections.Generic.Dictionary<
    string, (                                       // Graph node key (bag colour)
        System.Collections.Generic.List<string>,    // List of parent bag colours
        System.Collections.Generic.List<(           // List of child bags
            int,                                    // Number of child bag of specified colour
            string                                  // Specified colour of child bag
        )>
    )>;

namespace Advent2020
{
    public class Day07
    {
        static Graph LoadBags(string input)
        {
            var graph = new Graph();

            foreach (var line in FileIterator.Lines(input))
            {
                var groups = line.Groups("^(.+) bags contain (.+)\\.$");
                var bagType = groups[0];
                var bagNode = graph.GetOrCreate(bagType, () => (new List<string>(), new List<(int, string)>()));

                if (groups[1] != "no other bags")
                {
                    var children = groups[1].Split(',');
                    foreach (var child in children)
                    {
                        groups = child.Groups("(\\d+) (.+) bag");
                        bagNode.Item2.Add((int.Parse(groups[0]), groups[1]));

                        var childNode = graph.GetOrCreate(groups[1], () => (new List<string>(), new List<(int, string)>()));
                        childNode.Item1.Add(bagType);
                    }
                }
            }

            return graph;
        }

        void CollectUp(Graph graph, string bagType, HashSet<string> visited)
        {
            var node = graph[bagType];
            foreach (var parent in node.Item1)
            {
                visited.Add(parent);
                CollectUp(graph, parent, visited);
            }
        }

        int CountDown(Graph graph, string bagType)
        {
            // Count the child bags exclusive of the container bag itself. That way, we only get a count of the bags
            // held within it. Callers must manually add 1 for the container bag itself if they care.
            var count = 0;

            var node = graph[bagType];
            foreach (var child in node.Item2)
                count += child.Item1 + (child.Item1 * CountDown(graph, child.Item2));

            return count;
        }

        [Theory]
        [InlineData("Data/Day07_test.txt", 4)]
        [InlineData("Data/Day07.txt", 300)]
        public void Problem1(string input, int expected)
        {
            var outterBags = new HashSet<string>();
            var graph = LoadBags(input);
            CollectUp(graph, "shiny gold", outterBags);

            outterBags.Count.Should().Be(expected);
        }

        [Theory]
        [InlineData("Data/Day07_test.txt", 32)]
        [InlineData("Data/Day07_test2.txt", 126)]
        [InlineData("Data/Day07.txt", 8030)]
        public void Problem2(string input, int expected)
        {
            var graph = LoadBags(input);
            CountDown(graph, "shiny gold").Should().Be(expected);
        }
    }
}
