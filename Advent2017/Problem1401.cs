using Adevent2017.Utils;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Adevent2017
{
    public class Problem1401
    {
        string Hash(String input) => Problem1001.Hash(input);

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

        bool[,] BuildGrid(string key)
        {
            var grid = new bool[128, 128];
            for (var row = 0; row < 128; row++)
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
                        if (cell == 128) Oh.WhatTheFuck();
                        var value = (c & 8) == 8;
                        grid[row, cell] = value;
                        c <<= 1;
                        cell++;
                    }
                }
            }

            return grid;
        }

        class Node
        {
            public Node(int id) { Id = id; }
            public int Id;
            public bool Seen = false;
            public List<int> Links = new List<int>();
        }

        int GetNodeId(int x, int y) => (y * 128) + x;

        List<int> GetChildren(int x, int y, bool[,] grid)
        {
            var children = new List<int>();

            if (x > 0 && grid[x - 1, y])
                children.Add(GetNodeId(x - 1, y));

            if (x < 127 && grid[x + 1, y])
                children.Add(GetNodeId(x + 1, y));

            if (y > 0 && grid[x, y - 1])
                children.Add(GetNodeId(x, y - 1));

            if (y < 127 && grid[x, y + 1])
                children.Add(GetNodeId(x, y + 1));

            return children;
        }

        int WalkGraph(Dictionary<int, Node> graph, int currentNodeId)
        {
            var currentNode = graph[currentNodeId];
            if (currentNode.Seen) return 0;
            currentNode.Seen = true;

            var count = 1;
            foreach (var linkId in currentNode.Links)
                count += WalkGraph(graph, linkId);

            return count;
        }

        Dictionary<int, Node> BuildGraph(bool[,] grid)
        {
            var nodes = new Dictionary<int, Node>();
            for (var x = 0; x < 128; x++)
            {
                for (var y = 0; y < 128; y++)
                {
                    if (!grid[x, y]) continue;

                    var id = GetNodeId(x, y);
                    var children = GetChildren(x, y, grid);

                    Node node;
                    if (!nodes.TryGetValue(id, out node))
                    {
                        node = new Node(id);
                        nodes[id] = node;
                    }

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
            var count = 0;
            for (var y = 0; y < 128; y++)
            {
                for (var x = 0; x < 128; x++)
                {
                    count += grid[x, y] ? 1 : 0;
                }
            }
            return count;
        }

        int CountRegions(string key)
        {
            var grid = BuildGrid(key);
            var nodes = BuildGraph(grid);
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
        [InlineData("flqrgnkx", 8108)]
        [InlineData("jzgqcdpd", 8074)]
        public void Part1(string datafile, int answer) => CountUsed(datafile).Should().Be(answer);

        [Theory]
        [InlineData("flqrgnkx", 1242)]
        [InlineData("jzgqcdpd", 1212)]
        public void Part2(string datafile, int answer) => CountRegions(datafile).Should().Be(answer);
    }
}
