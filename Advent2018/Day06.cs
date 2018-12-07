using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Advent2018
{
    public class Day06
    {
        (int x, int y)[] ParseCoords(string[] coords)
        {
            var result = new(int, int)[coords.Length];
            for (var i = 0; i < coords.Length; i++)
            {
                var split = coords[i].Split(',');
                result[i] = (
                    int.Parse(split[0]),
                    int.Parse(split[1])
                );
            }
            return result;
        }

        (int x, int y) GetAreaExtent((int x, int y)[] coords)
        {
            int maxX = 0;
            int maxY = 0;
            foreach (var coord in coords)
            {
                if (maxX < coord.x)
                    maxX = coord.x;
                if (maxY < coord.y)
                    maxY = coord.y;
            }

            return (maxX + 1, maxY + 1);
        }

        int ManhattenDistance((int x, int y) pos1, (int x, int y) pos2)
        {
            return Math.Abs(pos1.x - pos2.x) + Math.Abs(pos1.y - pos2.y);
        }

        [Theory]
        [InlineData(9, 10, "1, 1", "1, 6", "8, 3", "3, 4", "5, 5", "8, 9")]
        public void Test_GetAreaExtent(int expectedWidth, int expectedHeight, params string[] input)
        {
            var coords = ParseCoords(input);
            var dims = GetAreaExtent(coords);
            dims.x.Should().Be(expectedWidth);
            dims.y.Should().Be(expectedHeight);
        }

        IEnumerable<(int Index, int Distance)> IterateDistances(int[,,] volume, int x, int y)
        {
            for (var i = 0; i < volume.GetLength(2); i++)
                yield return (i, volume[x, y, i]);
        }

        [Theory]
        [InlineData(0, 2, 4, 2, 4)]
        [InlineData(1, 3, 4, 3, 5)]
        [InlineData(2, 3, 7, 2, 8)]
        [InlineData(3, 7, 5, 9, 4)]
        [InlineData(4, 2, 3, 0, 1)]
        public void Test_Manhatten(int expectedDistance, int x1, int y1, int x2, int y2)
        {
            ManhattenDistance((x1, y1), (x2, y2)).Should().Be(expectedDistance);
        }

        [Theory]
        [InlineData(17, 16, 32, "1, 1", "1, 6", "8, 3", "3, 4", "5, 5", "8, 9")]
        public void Problem_Test(int largestOwned, int safeZoneSize, int safeDistance, params string[] input)
        {
            // This is a real brute force approach.
            // It feels that I could solve it with some sort of frontier exploration algorithm instead.
            var coords = ParseCoords(input);
            var dims = GetAreaExtent(coords);
            var volume = new int[dims.x, dims.y, coords.Length];

            // Build a 3D volume with each Z plane being the distance for that co-ordinate
            for (var y = 0; y < dims.y; y++)
                for (var x = 0; x < dims.x; x++)
                    for (var i = 0; i < coords.Length; i++)
                        volume[x, y, i] = ManhattenDistance(coords[i], (x, y));

            // Flatten the volume, recording the co-ordinate that is closest and therfore, owns it
            var owners = new int[dims.x, dims.y];
            // And sum up the total distances for each square
            var totalDistances = new int[dims.x, dims.y];
            for (var y = 0; y < dims.y; y++)
            {
                for (var x = 0; x < dims.x; x++)
                {
                    var scores = IterateDistances(volume, x, y).OrderBy(p => p.Distance).ToArray();
                    if (scores[0].Distance == scores[1].Distance)
                        owners[x, y] = -1;
                    else
                        owners[x, y] = scores[0].Index;

                    totalDistances[x, y] = scores.Select(s => s.Distance).Sum();
                }
            }

            // Mark the edge owners as infinite
            var totalAreas = new int[coords.Length];
            for (var x = 0; x < dims.x; x++)
            {
                if (owners[x, 0] != -1)
                    totalAreas[owners[x, 0]] = -1;
                if (owners[x, dims.y - 1] != -1)
                    totalAreas[owners[x, dims.y - 1]] = -1;
            }
            for (var y = 0; y < dims.y; y++)
            {
                if (owners[0, y] != -1)
                    totalAreas[owners[0, y]] = -1;
                if (owners[dims.x - 1, y] != -1)
                    totalAreas[owners[dims.x - 1, y]] = -1;
            }

            // Sum up the areas of the remaining owners
            foreach (var owner in owners)
                if (owner != -1 && totalAreas[owner] != -1)
                    totalAreas[owner]++;

            totalAreas.Max().Should().Be(largestOwned);

            var safeCount = 0;
            foreach (var distance in totalDistances)
                if (distance < safeDistance)
                    safeCount++;

            safeCount.Should().Be(safeZoneSize);
        }

        [Fact]
        public void Problem_Solution()
        {
            var inputs = Utils.FileIterator.LoadLines<string>("Data/Day06.txt");
            Problem_Test(4475, 35237, 10000, inputs);
        }
    }
}
