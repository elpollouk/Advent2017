using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2020
{
    public class Day10
    {
        int CalcJolts(string input)
        {
            var prevJolt = 0;
            var jump1 = 0;
            var jump3 = 1; // Include the final 3 jolt jump up front
            foreach (var jolt in FileIterator.Lines<int>(input).OrderBy((v) => v))
            {
                switch (jolt - prevJolt)
                {
                    case 1:
                        jump1++;
                        break;

                    case 2:
                        break;

                    case 3:
                        jump3++;
                        break;

                    default:
                        Oh.Bugger();
                        break;
                }
                prevJolt = jolt;
            }

            return jump1 * jump3;
        }

        long CountRoutes(string input)
        {
            // Graph nodes are just the total number of valid routes to reach the node
            var graph = new Dictionary<int, long>();
            var adapters = FileIterator.Lines<int>(input);

            // Populate the graph with uncalculated values, we'll populate it in the next step
            foreach (var adapter in adapters)
                graph[adapter] = 0;

            // All graphs start at 0 which has an implicit single route in
            graph[0] = 1;

            // Loop through the adapters in order, cascading up the total number of routes to reach each node in the graph.
            // A node's total route value is the sum of the route values of all the nodes that link to the current node.
            // We traverse the graph in adapter value order and back tracking to find the incoming links.
            foreach (var adapter in adapters.OrderBy((v) => v))
            {
                var total = 0L;
                for (var i = 1; i < 4; i++)
                    if (graph.TryGetValue(adapter - i, out long incomingTotal))
                        total += incomingTotal;

                graph[adapter] = total;
            }

            // Find the maximum total route value in the graph, this should be the last node, but the nodes aren't in order
            return graph.Values.Max((n) => n);
        }

        [Theory]
        [InlineData("Data/Day10_test1.txt", 35)]
        [InlineData("Data/Day10_test2.txt", 220)]
        [InlineData("Data/Day10.txt", 2046)]
        public void Problem1(string input, int expected)
        {
            CalcJolts(input).Should().Be(expected);
        }

        [Theory]
        [InlineData("Data/Day10_test1.txt", 8)]
        [InlineData("Data/Day10_test2.txt", 19208)]
        [InlineData("Data/Day10.txt", 1157018619904)]
        public void Problem2(string input, long expected)
        {
            CountRoutes(input).Should().Be(expected);
        }
    }
}
