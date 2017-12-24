using Adevent2017.Utils;
using FluentAssertions;
using System;
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

        int Solve1(string datafile, int cycles)
        {
            var grid = new Dictionary<ulong, NodeState>();
            var dy = -1;
            var dx = 0;
            var x = 0;
            var y = 0;
            var infections = 0;

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

            x /= 2;
            y /= 2;

            for (var i = 0; i < cycles; i++)
            {
                var ec = EncodeCoords(x, y);
                var state = grid.GetOrDefault(ec, NodeState.Clean);
                if (state == NodeState.Infected)
                {
                    var t = dx;
                    dx = -dy;
                    dy = t;
                    grid.Remove(ec);
                }
                else
                {
                    var t = dx;
                    dx = dy;
                    dy = -t;
                    grid[ec] = NodeState.Infected;
                    infections++;
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
    }
}
