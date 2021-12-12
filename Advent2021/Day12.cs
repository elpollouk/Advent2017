using FluentAssertions;
using System.Collections.Generic;
using Utils;
using Utils.DataStructures;
using Xunit;

namespace Advent2021
{
    public class Day12
    {
        static Graph<string> LoadGraph(string filename)
        {
            Graph<string> graph = new();

            foreach (var line in FileIterator.Lines(filename))
            {
                var parts = line.Split("-");
                graph.AddNodeIfNotInGraph(parts[0]);
                graph.AddNodeIfNotInGraph(parts[1]);

                graph.AddTwoWayLink(parts[0], parts[1]);
            }

            return graph;
        }

        static bool IsSmall(string node) => 'a' <= node[0] && node[0] <= 'z';

        static void Traverse(Graph<string> graph, HashSet<string> visited, string node, ref long totalPaths)
        {
            if (node == "end")
            {
                totalPaths++;
                return;
            }

            if (IsSmall(node) && visited.Contains(node)) return;
            visited.Add(node);

            foreach (var next in graph.GetLinked(node))
                Traverse(graph, visited, next, ref totalPaths);

            visited.Remove(node);
        }

        static void Traverse2(Graph<string> graph, HashSet<string> visited, string visitedTwice, string node, ref long totalPaths)
        {
            if (node == "end")
            {
                totalPaths++;
                return;
            }

            if (IsSmall(node) && visited.Contains(node))
            {
                if (node == "start") return;
                if (visitedTwice != null) return;
                visitedTwice = node;
            }
            else
            {
                visited.Add(node);
            }

            foreach (var next in graph.GetLinked(node))
                Traverse2(graph, visited, visitedTwice, next, ref totalPaths);

            if (node != visitedTwice)
                visited.Remove(node);
        }

        [Theory]
        [InlineData("Data/Day12_Test1.txt", 10)]
        [InlineData("Data/Day12_Test2.txt", 19)]
        [InlineData("Data/Day12_Test3.txt", 226)]
        [InlineData("Data/Day12.txt", 5157)]
        public void Part1(string filename, long expectedAnswer)
        {
            HashSet<string> visited = new();
            long totalPaths = 0;
            Graph<string> graph = LoadGraph(filename);

            Traverse(graph, visited, "start", ref totalPaths);

            totalPaths.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day12_Test1.txt", 36)]
        [InlineData("Data/Day12_Test2.txt", 103)]
        [InlineData("Data/Day12_Test3.txt", 3509)]
        [InlineData("Data/Day12.txt", 144309)]
        public void Part2(string filename, long expectedAnswer)
        {
            HashSet<string> visited = new();
            long totalPaths = 0;
            Graph<string> graph = LoadGraph(filename);

            Traverse2(graph, visited, null, "start", ref totalPaths);

            totalPaths.Should().Be(expectedAnswer);
        }
    }
}
