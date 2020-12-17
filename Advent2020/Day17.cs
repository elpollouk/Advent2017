using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Utils;
using Xunit;

using Space3 = System.Collections.Generic.Dictionary<(int x, int y, int z), bool>;
using Space4 = System.Collections.Generic.Dictionary<(int x, int y, int z, int w), bool>;

namespace Advent2020
{
    public class Day17
    {
        private static int Count3dNeighbours(Space3 space, (int x, int y, int z) coord)
        {
            var count = 0;

            for (int x = coord.x - 1; x <= coord.x + 1; x++)
                for (int y = coord.y - 1; y <= coord.y + 1; y++)
                    for (int z = coord.z - 1; z <= coord.z + 1; z++)
                        if (space.GetOrDefault((x, y, z))) count++;

            // Correct for counting the center square
            if (space.GetOrDefault(coord)) count--;

            return count;
        }

        private static int Count4dNeighbours(Space4 space, (int x, int y, int z, int w) coord)
        {
            var count = 0;

            for (int x = coord.x - 1; x <= coord.x + 1; x++)
                for (int y = coord.y - 1; y <= coord.y + 1; y++)
                    for (int z = coord.z - 1; z <= coord.z + 1; z++)
                        for (int w = coord.w - 1; w <= coord.w + 1; w++)
                            if (space.GetOrDefault((x, y, z, w))) count++;

            // Correct for counting the center square
            if (space.GetOrDefault(coord)) count--;

            return count;
        }

        private static IEnumerable<(int x, int y, int z)> Explore3d(int minx, int maxx, int miny, int maxy, int minz, int maxz)
        {
            for (var x = minx - 1; x <= maxx + 1; x++)
                for (var y = miny - 1; y <= maxy + 1; y++)
                    for (var z = minz - 1; z <= maxz + 1; z++)
                        yield return (x, y, z);
        }

        private static IEnumerable<(int x, int y, int z, int w)> Explore4d(int minx, int maxx, int miny, int maxy, int minz, int maxz, int minw, int maxw)
        {
            for (var x = minx - 1; x <= maxx + 1; x++)
                for (var y = miny - 1; y <= maxy + 1; y++)
                    for (var z = minz - 1; z <= maxz + 1; z++)
                        for (var w = minw - 1; w <= maxw + 1; w++)
                            yield return (x, y, z, w);
        }

        static void DumpSlice(Space3 space, int fromx, int tox, int fromy, int toy, int z)
        {
            for (var y = fromy; y <= toy; y++)
            {
                for (var x = fromx; x <= tox; x++)
                {
                    Debug.Write(space.GetOrDefault((x, y, z)) ? '#' : '.');

                }
                Debug.WriteLine("");
            }
        }

        private static int Simulate3d(string input)
        {
            Space3 spaceInput = new();

            var initalSlice = FileIterator.LoadGrid(input, (c, x, y) => c switch
            {
                '#' => true,
                _ => false
            });

            foreach (var (x, y) in initalSlice.Rectangle())
                spaceInput[(x, y, 0)] = initalSlice[x, y];

            var minx = 0;
            var maxx = initalSlice.GetLength(0) - 1;
            var miny = 0;
            var maxy = initalSlice.GetLength(1) - 1;
            var minz = 0;
            var maxz = 0;

            for (var i = 0; i < 6; i++)
            {
                /*Debug.WriteLine("Space dump:");
                for (var z = minz; z <= maxz; z++)
                {
                    DumpSlice(spaceInput, minx, maxx, miny, maxy, z);
                    Debug.WriteLine("");
                }*/
                Space3 spaceOutput = new();

                foreach (var coord in Explore3d(minx, maxx, miny, maxy, minz, maxz))
                {
                    var count = Count3dNeighbours(spaceInput, coord);
                    var active = spaceInput.GetOrDefault(coord);
                    if (active && count != 2 && count != 3)
                    {
                        spaceOutput[coord] = false;
                    }
                    else if (!active && count == 3)
                    {
                        spaceOutput[coord] = true;
                        // Increase the exploration horizon if needed
                        if (coord.x < minx) minx = coord.x;
                        if (coord.x > maxx) maxx = coord.x;
                        if (coord.y < miny) miny = coord.y;
                        if (coord.y > maxy) maxy = coord.y;
                        if (coord.z < minz) minz = coord.z;
                        if (coord.z > maxz) maxz = coord.z;
                    }
                    else if (active)
                    {
                        spaceOutput[coord] = true;
                    }
                }

                spaceInput = spaceOutput;
            }

            return spaceInput.Values.Select(i => i ? 1 : 0).Sum();
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
                spaceInput[(x, y, 0, 0)] = initalSlice[x, y];

            var minx = 0;
            var maxx = initalSlice.GetLength(0) - 1;
            var miny = 0;
            var maxy = initalSlice.GetLength(1) - 1;
            var minz = 0;
            var maxz = 0;
            var minw = limitW ? 1 : 0;
            var maxw = limitW ? -1 : 0;

            for (var i = 0; i < 6; i++)
            {
                Space4 spaceOutput = new();

                foreach (var coord in Explore4d(minx, maxx, miny, maxy, minz, maxz, minw, maxw))
                {
                    var count = Count4dNeighbours(spaceInput, coord);
                    var active = spaceInput.GetOrDefault(coord);
                    if (active && count != 2 && count != 3)
                    {
                        spaceOutput[coord] = false;
                    }
                    else if (!active && count == 3)
                    {
                        spaceOutput[coord] = true;
                        // Increase the exploration horizon if needed
                        if (coord.x < minx) minx = coord.x;
                        if (coord.x > maxx) maxx = coord.x;
                        if (coord.y < miny) miny = coord.y;
                        if (coord.y > maxy) maxy = coord.y;
                        if (coord.z < minz) minz = coord.z;
                        if (coord.z > maxz) maxz = coord.z;
                        if (!limitW)
                        {
                            if (coord.w < minw) minw = coord.w;
                            if (coord.w > maxw) maxw = coord.w;
                        }
                    }
                    else if (active)
                    {
                        spaceOutput[coord] = true;
                    }
                }

                spaceInput = spaceOutput;
            }

            return spaceInput.Values.Select(i => i ? 1 : 0).Sum();
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
