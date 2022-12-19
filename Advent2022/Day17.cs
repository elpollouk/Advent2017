using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2022
{
    public class Day17
    {
        class Rock
        {
            private bool[][] Bitmap;

            public int Width => Bitmap[0].Length;
            public int Height => Bitmap.Length;

            public Rock(params string[] lines)
            {
                Bitmap = new bool[lines.Length][];
                int width = lines[0].Length;
                for (int y = 0; y < lines.Length; y++)
                {
                    Bitmap[y] = new bool[width];
                    for (int x = 0; x < width; x++)
                    {
                        Bitmap[y][x] = lines[y][x] == '#';
                    }
                }
            }

            public bool CheckCollision(XY pos, HashSet<(int,int)> space)
            {
                if (pos.x < 0) return true;
                if (pos.y < 0) return true;
                if (pos.x + Width > 7) return true;

                for (var y = 0; y < Height; y++)
                {
                    for (var x = 0; x < Width; x++)
                    {
                        if (Bitmap[y][x])
                        {
                            if (space.Contains((pos.x + x, pos.y + y)))
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            public int Write(XY pos, HashSet<(int, int)> space)
            {
                for (var y = 0; y < Height; y++)
                {
                    for (var x = 0; x < Width; x++)
                    {
                        if (Bitmap[y][x])
                        {
                            space.Add((pos.x + x, pos.y + y));
                        }
                    }
                }

                return pos.y + Height;
            }
        }

        Rock[] Rocks =
        {
            new(
                "####"
            ),
            new(
                " # ",
                "###",
                " # "
            ),
            new(
                "###",
                "  #",
                "  #"
            ),
            new(
                "#",
                "#",
                "#",
                "#"
            ),
            new(
                "##",
                "##"
            )
        };

        int Drop(Rock rock, int height, HashSet<(int, int)> space, Func<int> wind)
        {
            XY pos = (2, height);

            while (true)
            {
                var checkPos = pos.Clone().Add(wind(), 0);
                if (!rock.CheckCollision(checkPos, space))
                {
                    pos = checkPos;
                }

                checkPos = pos.Clone().Add(0, -1);
                if (rock.CheckCollision(checkPos, space))
                {
                    return rock.Write(pos, space);
                }
                pos = checkPos;
            }
        }

        Func<Rock> CreateRocks()
        {
            int i = 0;
            return () => Rocks.GetAtMod(i++);
        }

        Func<int> CreateWind(string filename)
        {
            var wind = FileIterator.Lines(filename).First();
            int i = 0;
            return () =>
            {
                var c = wind.GetAtMod(i++);
                return c == '<' ? -1 : 1;
            };
        }

        ulong HashTop32(HashSet<(int,int)> space, int top)
        {
            ulong hash = 0xDEADBEEFDEADBEEF;
            int y = top - 1;
            for (int i = 0; i < 4; i++)
            {
                ulong value = 0;
                for (int j = 0; j < 8; j++)
                {
                    for (int x = 0; x < 7; x++)
                    {
                        value <<= 1;
                        value |= space.Contains((x, y)) ? 1UL : 0UL;
                    }
                    y--;
                }
                hash *= value ^ 0xFFFFFFFFFFFFFFFF;
            }

            return hash;
        }

        [Theory]
        [InlineData("Data/Day17_Test.txt", 3068)]
        [InlineData("Data/Day17.txt", 3106)]
        public void Part1(string filename, int expectedAnswer)
        {
            var wind = CreateWind(filename);
            HashSet<(int, int)> space = new();
            int top = 0;

            for (int i = 0; i < 2022; i++)
            {
                var rock = Rocks.GetAtMod(i);
                var t = Drop(rock, top + 3, space, wind);
                if (top < t) top = t;
            }

            top.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day17_Test.txt", 2022, 3068)]
        [InlineData("Data/Day17_Test.txt", 1000000000000, 1514285714288)]
        [InlineData("Data/Day17.txt", 1000000000000, 1537175792495)]
        public void Part2(string filename, long numDrops, long expectedAnswer)
        {
            var wind = CreateWind(filename);
            var nextRock = CreateRocks();
            HashSet<(int, int)> space = new();
            Dictionary<ulong, (int, int)> seen = new();
            int top = 0;

            int cycleLength = 0;
            int cycleDelta = 0;
            for (int i = 0; i < numDrops; i++)
            {
                var rock = nextRock();
                var t = Drop(rock, top + 3, space, wind);
                if (top < t) top = t;

                var hash = HashTop32(space, top);
                if (seen.TryGetValue(hash, out (int seen, int top) last))
                {
                    cycleLength = i - last.seen;
                    cycleDelta = top - last.top;
                    numDrops -= i;
                    break;
                }
                else
                {
                    seen[hash] = (i, top);
                }
            }

            long numCycles = numDrops / cycleLength;
            long remainderCycles = (numDrops % cycleLength) - 1;
            for (int i = 0; i < remainderCycles; i++)
            {
                var rock = nextRock();
                var t = Drop(rock, top + 3, space, wind);
                if (top < t) top = t;
            }

            long ltop = top + (numCycles * cycleDelta);
            ltop.Should().Be(expectedAnswer);
        }
    }
}
