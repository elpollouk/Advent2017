using Adevent2017.Utils;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Adevent2017
{
    using Node = Problem1201.Node;

    public class Problem1401
    {
        private const int GridSize = 128;

        string Hash(String input) => Problem1001.Hash(input);

        int CountGroups(Dictionary<int, Node> graph) => Problem1201.CountGroups(graph);

        int GetNodeId(int x, int y) => (y * GridSize) + x;

        int CharToInt(char c)
        {
            switch (c)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return c - '0';

                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                    return 10 + (c - 'a');
            }

            Oh.ShttingHell();
            return -1;
        }

        List<int> GetChildren(int x, int y, Dictionary<int, Node> graph)
        {
            var children = new List<int>();

            if (x > 0 && graph.ContainsKey(GetNodeId(x - 1, y)))
                children.Add(GetNodeId(x - 1, y));

            if (y > 0 && graph.ContainsKey(GetNodeId(x, y - 1)))
                children.Add(GetNodeId(x, y - 1));

            return children;
        }

        Dictionary<int, Node> BuildGraph(string key)
        {
            var graph = new Dictionary<int, Node>();
            for (var y = 0; y < GridSize; y++)
            {
                var rowKey = $"{key}-{y}";
                var hash = Hash(rowKey);
                if (hash.Length != 32) Oh.ShttingHell();

                var x = 0;
                foreach (var ic in hash)
                {
                    var c = CharToInt(ic);
                    for (var bit = 0; bit < 4; bit++)
                    {
                        if ((c & 8) == 8)
                        {
                            var id = GetNodeId(x, y);
                            var node = new Node(id);
                            graph[id] = node;

                            var children = GetChildren(x, y, graph);
                            foreach (var linkId in children)
                            {
                                node.Links.Add(linkId);

                                // Reciprical link
                                Node linkedNode;
                                if (graph.TryGetValue(linkId, out linkedNode))
                                    if (!linkedNode.Links.Contains(id))
                                        linkedNode.Links.Add(id);
                            }
                        }
                        c <<= 1;
                        x++;
                    }
                }
            }

            return graph;
        }

        int CountUsed(string key)
        {
            var graph = BuildGraph(key);
            return graph.Count;
        }

        int CountRegions(string key)
        {
            var graph = BuildGraph(key);
            return CountGroups(graph);
        }

        [Theory]
        [InlineData("flqrgnkx", 8108)]
        [InlineData("jzgqcdpd", 8074)]
        public void Part1(string datafile, int answer) => CountUsed(datafile).Should().Be(answer);

        [Theory]
        [InlineData("flqrgnkx", 1242)]
        [InlineData("jzgqcdpd", 1212)]
        public void Part2(string datafile, int answer) => CountRegions(datafile).Should().Be(answer);
    }
}
