using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Utils.Alogrithms;
using Xunit;

namespace Advent2021
{
    public class Day15
    {
        class Grid : Astar.IGraphAdapter<(int x, int y)>
        {
            public readonly int[,] Area;

            public (int x, int y) Exit => (Area.GetLength(0) - 1, Area.GetLength(1) - 1);

            public Grid(int[,] area)
            {
                Area = area;
            }

            public IEnumerable<(int x, int y)> GetLinked((int x, int y) node) => Area.GetAdjecentPos(node.x, node.y);

            public int GetMoveCost((int x, int y) from, (int x, int y) to) => Area[to.x, to.y];

            public int GetScore((int x, int y) from, (int x, int y) to) => (to.x - from.x) + (to.y - from.y);

            public bool NodesEqual((int x, int y) a, (int x, int y) b) => a == b;
        }

        static Grid LoadGrid(string filename)
        {
            var area = FileIterator.LoadGrid(filename, ArrayExtensions.CharToInt);
            return new Grid(area);
        }

        static Grid Expand(Grid grid)
        {
            var oldArea = grid.Area;
            var newArea = new int[oldArea.GetLength(0) * 5, oldArea.GetLength(1) * 5];

            for (var tileY = 0; tileY < 5; tileY++) 
            {
                for (var tileX = 0; tileX < 5; tileX++)
                {
                    var offsetX = tileX * oldArea.GetLength(0);
                    var offsetY = tileY * oldArea.GetLength(1);

                    for (var y = 0; y < oldArea.GetLength(1); y++)
                    {
                        for (var x = 0; x < oldArea.GetLength(0); x++)
                        {
                            var newX = offsetX + x;
                            var newY = offsetY + y;
                            var newValue = (oldArea[x, y] + tileX + tileY) % 9;
                            if (newValue == 0) newValue = 9;
                            newArea[newX, newY] = newValue;
                        }
                    }
                }
            }

            return new Grid(newArea);
        }

        [Theory]
        [InlineData("Data/Day15_Test.txt", 40)]
        [InlineData("Data/Day15.txt", 373)]
        public void Part1(string filename, int expectedAnswer)
        {
            var grid = LoadGrid(filename);
            grid.Area[0, 0] = 0;

            Astar.FindPath(grid, (x:0, y:0), grid.Exit)
                .Select(p => grid.Area[p.x, p.y])
                .Sum()
                .Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day15_Test.txt", 315)]
        [InlineData("Data/Day15.txt", 2868)]
        public void Part2(string filename, int expectedAnswer)
        {
            var grid = LoadGrid(filename);
            grid = Expand(grid);
            grid.Area[0, 0] = 0;

            Astar.FindPath(grid, (x:0, y:0), grid.Exit)
                .Select(p => grid.Area[p.x, p.y])
                .Sum()
                .Should().Be(expectedAnswer);
        }
    }
}
