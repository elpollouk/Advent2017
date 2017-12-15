using Adevent2017.DataStructures;
using Adevent2017.Utils;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Adevent2017
{

    public class Problem1401
    {
        private const int GridSize = 128;

        string Hash(string input) => Problem1001.Hash(input);

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

        List<int> GetChildren(int x, int y, Graph<int> graph)
        {
            var children = new List<int>();

            if (x > 0 && graph.Contains(GetNodeId(x - 1, y)))
                children.Add(GetNodeId(x - 1, y));

            if (y > 0 && graph.Contains(GetNodeId(x, y - 1)))
                children.Add(GetNodeId(x, y - 1));

            return children;
        }

        Graph<int> BuildGraph(string key)
        {
            var graph = new Graph<int>();
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
                            graph.AddNode(id);

                            var children = GetChildren(x, y, graph);
                            foreach (var linkId in children)
                                graph.AddTwoWayLink(id, linkId);
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
            return graph.Size;
        }

        int CountRegions(string key)
        {
            var graph = BuildGraph(key);
            return graph.NumberOfGroups;
        }

        [Theory]
        [InlineData("flqrgnkx", 8108)]
        [InlineData("jzgqcdpd", 8074)]
        public void Part1(string key, int answer) => CountUsed(key).Should().Be(answer);

        [Theory]
        [InlineData("flqrgnkx", 1242)]
        [InlineData("jzgqcdpd", 1212)]
        public void Part2(string key, int answer) => CountRegions(key).Should().Be(answer);
    }
}
