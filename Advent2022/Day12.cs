using FluentAssertions;
using System.Collections.Generic;
using Utils;
using Utils.Alogrithms;
using Xunit;

namespace Advent2022
{
    public class Day12
    {
        class Adapter : GridAdapterBase<int>
        {
            public Adapter(int[,] grid) : base(grid) {}

            public override IEnumerable<(int x, int y)> GetLinked((int x, int y) node)
            {
                var currentHeight = Grid[node.x, node.y];
                foreach (var pos in Grid.GetAdjacentPos(node.x, node.y))
                {
                    var destHeight = Grid[pos.x, pos.y];
                    if (destHeight <= currentHeight + 1)
                    {
                        yield return pos;
                    }
                }
            }
        }

        (Adapter, (int,int), (int,int)) LoadGrid(string filename)
        {
            (int x, int y) start = (-1, -1);
            (int x, int y) end = (-1, -1);
            var grid = FileIterator.LoadGrid(filename, (c, x, y) =>
            {
                if (c == 'S')
                {
                    start = (x, y);
                    return 0;
                }
                else if (c == 'E')
                {
                    end = (x, y);
                    return 25;
                }
                return c - 'a';
            });

            return (new(grid), start, end);
        }

        int GetPathLength(Adapter adapter, (int,int) start, (int,int) end)
        {
            var path = Astar.FindPath(adapter, start, end);
            if (path == null) return int.MaxValue;
            return path.Count - 1;
        }

        [Theory]
        [InlineData("Data/Day12_Test.txt", 31)]
        [InlineData("Data/Day12.txt", 361)]
        public void Part1(string filename, int expectedAnswer)
        {
            var (adapter, start, end) = LoadGrid(filename);
            GetPathLength(adapter, start, end).Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day12_Test.txt", 29)]
        [InlineData("Data/Day12.txt", 354)]
        public void Part2(string filename, int expectedAnswer)
        {
            var (adapter, _, end) = LoadGrid(filename);
            int shortest = int.MaxValue;

            foreach (var pos in adapter.Grid.Rectangle())
            {
                if (adapter.Grid[pos.x, pos.y] != 0) continue;
                var length = GetPathLength(adapter, pos, end);
                if (length < shortest) shortest = length;
            }

            shortest.Should().Be(expectedAnswer);
        }
    }
}
