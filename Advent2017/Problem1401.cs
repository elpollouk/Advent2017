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

        List<int> GetChildren(int x, int y, bool[,] grid)
        {
            var children = new List<int>();

            if (x > 0 && grid[x - 1, y])
                children.Add(GetNodeId(x - 1, y));

            if (x < (GridSize-1) && grid[x + 1, y])
                children.Add(GetNodeId(x + 1, y));

            if (y > 0 && grid[x, y - 1])
                children.Add(GetNodeId(x, y - 1));

            if (y < (GridSize-1) && grid[x, y + 1])
                children.Add(GetNodeId(x, y + 1));

            return children;
        }

        bool[,] BuildGrid(string key)
        {
            var grid = new bool[GridSize, GridSize];
            for (var row = 0; row < GridSize; row++)
            {
                var rowKey = $"{key}-{row}";
                var hash = Hash(rowKey);
                if (hash.Length != 32) Oh.ShttingHell();

                var cell = 0;
                foreach (var ic in hash)
                {
                    var c = CharToInt(ic);
                    for (var bit = 0; bit < 4; bit++)
                    {
                        var value = (c & 8) == 8;
                        grid[row, cell] = value;
                        c <<= 1;
                        cell++;
                    }
                }
            }

            return grid;
        }

        Dictionary<int, Node> BuildGraph(bool[,] grid)
        {
            var nodes = new Dictionary<int, Node>();
            for (var x = 0; x < GridSize; x++)
            {
                for (var y = 0; y < GridSize; y++)
                {
                    if (!grid[x, y]) continue;

                    var id = GetNodeId(x, y);
                    var children = GetChildren(x, y, grid);

                    Node node = new Node(id);
                    nodes[id] = node;

                    foreach (var linkId in children)
                    {
                        node.Links.Add(linkId);

                        // Reciprical link
                        Node linkedNode;
                        if (nodes.TryGetValue(linkId, out linkedNode))
                            if (!linkedNode.Links.Contains(id))
                                linkedNode.Links.Add(id);
                    }
                }
            }

            return nodes;
        }

        int CountUsed(string key)
        {
            var grid = BuildGrid(key);
            var graph = BuildGraph(grid);
            return graph.Count;
        }

        int CountRegions(string key)
        {
            var grid = BuildGrid(key);
            var graph = BuildGraph(grid);
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
