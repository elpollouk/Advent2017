using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2023
{
    public class Day11
    {
        [Theory]
        [InlineData("Data/Day11_Test.txt", 2, 374)]
        [InlineData("Data/Day11_Test.txt", 10, 1030)]
        [InlineData("Data/Day11_Test.txt", 100, 8410)]
        [InlineData("Data/Day11.txt", 2, 9686930)]
        [InlineData("Data/Day11.txt", 1000000, 630728425490)]
        public void Solve(string filename, int expansionFactor, long expectedAnswer)
        {
            var grid = FileIterator.LoadGrid(filename);
            var maxX = grid.GetLength(0) - 1;
            var maxY = grid.GetLength(1) - 1;

            Dictionary<int, List<XY>> galByRow = [];
            Dictionary<int, List<XY>> galByCol = [];

            foreach (var ((x, y), c) in grid.Iterate())
            {
                if (c == '.') continue;

                var gal = new XY(x, y);
                galByRow.GetOrCreate(y, () => []).Add(gal);
                galByCol.GetOrCreate(x, () => []).Add(gal);
            }

            // Expand horizontally
            int expansion = 0;
            for (int i = 0; i <= maxX; i++)
            {
                var list = galByCol.GetOrDefault(i);
                if (list == null)
                {
                    expansion += (expansionFactor - 1);
                }
                else foreach (var gal in list)
                {
                    gal.x += expansion;
                }
            }

            // Expand vertically
            expansion = 0;
            for (int i = 0; i <= maxY; i++)
            {
                var list = galByRow.GetOrDefault(i);
                if (list == null)
                {
                    expansion += (expansionFactor - 1);
                }
                else foreach (var gal in list)
                {
                    gal.y += expansion;
                }
            }

            // Flatten the collections and get the Manhattan distances
            long total = 0;
            var galaxies = galByRow.Values.SelectMany(l => l).ToArray();
            for (int i = 0; i < galaxies.Length - 1; i++)
            {
                var gal1 = galaxies[i];
                for (int j = i + 1; j < galaxies.Length; j++)
                {
                    var gal2 = galaxies[j];
                    total += gal1.ManhattanDistanceTo(gal2);
                }
            }

            total.Should().Be(expectedAnswer);
        }
    }
}
