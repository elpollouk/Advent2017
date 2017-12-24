using Adevent2017.Utils;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Adevent2017
{
    public class Problem2201
    {
        enum NodeState
        {
            Clean = 0,
            Weakened = 1,
            Infected = 2,
            Flagged = 3,
        }

        ulong EncodeCoords(int x, int y)
        {
            ulong ux = (ulong)x;
            ulong uy = (ulong)y;
            return (ux << 32) | (uy & 0xFFFFFFFF);
        }

        Dictionary<ulong, NodeState> LoadGrid(string datafile, out int startX, out int startY)
        {
            var grid = new Dictionary<ulong, NodeState>();
            var x = 0;
            var y = 0;

            FileIterator.ForEachLine<string>(datafile, line =>
            {
                x = 0;
                foreach (var c in line)
                {
                    if (c == '#')
                    {
                        grid[EncodeCoords(x, y)] = NodeState.Infected;
                    }
                    x++;
                }
                y++;
            });

            startX = x / 2;
            startY = y / 2;

            return grid;
        }

        void TurnLeft(ref int dx, ref int dy)
        {
            var t = dx;
            dx = dy;
            dy = -t;
        }

        void TurnRight(ref int dx, ref int dy)
        {
            var t = dx;
            dx = -dy;
            dy = t;
        }

        void Reverse(ref int dx, ref int dy)
        {
            dx = -dx;
            dy = -dy;
        }

        int Solve1(string datafile, int cycles)
        {
            int x;
            int y;
            var dy = -1;
            var dx = 0;
            var infections = 0;
            var grid = LoadGrid(datafile, out x, out y);

            for (var i = 0; i < cycles; i++)
            {
                var ec = EncodeCoords(x, y);
                var state = grid.GetOrDefault(ec, NodeState.Clean);
                if (state == NodeState.Infected)
                {
                    TurnRight(ref dx, ref dy);
                    grid.Remove(ec);
                }
                else
                {
                    TurnLeft(ref dx, ref dy);
                    grid[ec] = NodeState.Infected;
                    infections++;
                }
                x += dx;
                y += dy;
            }

            return infections;
        }

        int Solve2(string datafile, int cycles)
        {
            int x;
            int y;
            var dy = -1;
            var dx = 0;
            var infections = 0;
            var grid = LoadGrid(datafile, out x, out y);

            for (var i = 0; i < cycles; i++)
            {
                var ec = EncodeCoords(x, y);
                var state = grid.GetOrDefault(ec, NodeState.Clean);
                switch (state)
                {
                    case NodeState.Clean:
                        TurnLeft(ref dx, ref dy);
                        grid[ec] = NodeState.Weakened;
                        break;

                    case NodeState.Weakened:
                        grid[ec] = NodeState.Infected;
                        infections++;
                        break;

                    case NodeState.Infected:
                        TurnRight(ref dx, ref dy);
                        grid[ec] = NodeState.Flagged;
                        break;

                    case NodeState.Flagged:
                        Reverse(ref dx, ref dy);
                        grid.Remove(ec);
                        break;
                }
                x += dx;
                y += dy;
            }

            return infections;
        }

        [Theory]
        [InlineData("Data/2201-example.txt", 7, 5)]
        [InlineData("Data/2201-example.txt", 70, 41)]
        [InlineData("Data/2201-example.txt", 10000, 5587)]
        [InlineData("Data/2201.txt", 10000, 5182)]
        void Part1(string datafile, int cycles, int answer) => Solve1(datafile, cycles).Should().Be(answer);

        [Theory]
        [InlineData("Data/2201-example.txt", 100, 26)]
        [InlineData("Data/2201-example.txt", 10000000, 2511944)]
        [InlineData("Data/2201.txt", 10000000, 2512008)]
        void Part2(string datafile, int cycles, int answer) => Solve2(datafile, cycles).Should().Be(answer);
    }
}
