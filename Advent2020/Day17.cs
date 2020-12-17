using FluentAssertions;
using System.Collections.Generic;
using Utils;
using Xunit;

using Space4 = System.Collections.Generic.HashSet<(int x, int y, int z, int w)>;

namespace Advent2020
{
    public class Day17
    {
        private static int Count4dNeighbours(Space4 space, (int x, int y, int z, int w) coord)
        {
            var count = 0;

            for (int x = coord.x - 1; x <= coord.x + 1; x++)
                for (int y = coord.y - 1; y <= coord.y + 1; y++)
                    for (int z = coord.z - 1; z <= coord.z + 1; z++)
                        for (int w = coord.w - 1; w <= coord.w + 1; w++)
                            if (space.Contains((x, y, z, w))) count++;

            // Correct for counting the center cube
            if (space.Contains(coord)) count--;

            return count;
        }

        private static IEnumerable<(int x, int y, int z, int w)> Explore4d(int minx, int maxx, int miny, int maxy, int minz, int maxz, int minw, int maxw)
        {
            for (var x = minx; x <= maxx; x++)
                for (var y = miny; y <= maxy; y++)
                    for (var z = minz; z <= maxz; z++)
                        for (var w = minw; w <= maxw; w++)
                            yield return (x, y, z, w);
        }

        private static int Simulate4d(string input, bool limitW = false)
        {
            Space4 spaceInput = new();

            var initalSlice = FileIterator.LoadGrid(input, (c, x, y) => c switch
            {
                '#' => true,
                _ => false
            });

            foreach (var (x, y) in initalSlice.Rectangle())
                if (initalSlice[x, y])
                    spaceInput.Add((x, y, 0, 0));

            var minx = 0;
            var maxx = initalSlice.GetLength(0) - 1;
            var miny = 0;
            var maxy = initalSlice.GetLength(1) - 1;
            var minz = 0;
            var maxz = 0;
            var minw = 0;
            var maxw = 0;

            for (var i = 0; i < 6; i++)
            {
                Space4 spaceOutput = new();
                minx--;
                maxx++;
                miny--;
                maxy++;
                minz--;
                maxz++;
                if (!limitW)
                {
                    minw--;
                    maxw++;
                }

                foreach (var coord in Explore4d(minx, maxx, miny, maxy, minz, maxz, minw, maxw))
                {
                    var count = Count4dNeighbours(spaceInput, coord);
                    var active = spaceInput.Contains(coord);
                    if (active && count is 2 or 3)
                    {
                        spaceOutput.Add(coord);
                    }
                    else if (!active && count == 3)
                    {
                        spaceOutput.Add(coord);
                    }
                }

                spaceInput = spaceOutput;
            }

            return spaceInput.Count;
        }

        [Theory]
        [InlineData("Data/Day17_test.txt", 112)]
        [InlineData("Data/Day17.txt", 209)]
        public void Problem1(string input, int expected) => Simulate4d(input, true).Should().Be(expected);


        [Theory]
        [InlineData("Data/Day17_test.txt", 848)]
        [InlineData("Data/Day17.txt", 1492)]
        public void Problem2(string input, int expected) => Simulate4d(input).Should().Be(expected);
    }
}
