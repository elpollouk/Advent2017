using Adevent2017.Utils;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Adevent2017
{
    public class Problem1201
    {
        public class Node
        {
            public Node(int id) { Id = id; }
            public int Id;
            public bool Seen = false;
            public List<int> Links = new List<int>();
        }

        public class Graph : Dictionary<int, Node> { }

        public static int WalkGraph(Graph graph, int currentNodeId)
        {
            var currentNode = graph[currentNodeId];
            if (currentNode.Seen) return 0;
            currentNode.Seen = true;

            var count = 1;
            foreach (var linkId in currentNode.Links)
                count += WalkGraph(graph, linkId);

            return count;
        }

        static Graph BuildGraph(string datafile)
        {
            var nodes = new Graph();
            FileIterator.ForEachLine<string>(datafile, line =>
            {
                var tokens = line.Replace(",", "").Split(' ');
                var id = int.Parse(tokens[0]);
                var node = new Node(id);

                for (var i = 2; i < tokens.Length; i++)
                {
                    var linkId = int.Parse(tokens[i]);

                    node.Links.Add(linkId);

                    // Reciprical link
                    Node linkedNode;
                    if (nodes.TryGetValue(linkId, out linkedNode))
                        if (!linkedNode.Links.Contains(id))
                            linkedNode.Links.Add(id);

                    nodes[id] = node;
                }
            });

            return nodes;
        }

        int Solve1(string datafile)
        {
            var nodes = BuildGraph(datafile);
            return WalkGraph(nodes, 0);
        }

        public static int CountGroups(Graph nodes)
        {
            int groupCount = 0;

            foreach (var node in nodes.Values)
            {
                if (!node.Seen)
                {
                    groupCount++;
                    WalkGraph(nodes, node.Id);
                }
            }

            return groupCount;
        }

        [Theory]
        [InlineData("Data/1201-example.txt", 6)]
        [InlineData("Data/1201.txt", 380)]
        public void Part1(string datafile, int answer) => Solve1(datafile).Should().Be(answer);

        [Theory]
        [InlineData("Data/1201-example.txt", 2)]
        [InlineData("Data/1201.txt", 181)]
        public void Part2(string datafile, int answer)
        {
            var nodes = BuildGraph(datafile);
            CountGroups(nodes).Should().Be(answer);
        }
    }
}
