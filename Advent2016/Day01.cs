using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2016
{
    public class Day01
    {
        (int dX, int dY) Turn(char direction, (int dX, int dY) facing)
        {
            switch (direction)
            {
                case 'L':
                    return facing.RotateLeft();

                case 'R':
                    return facing.RotateRight();

                default:
                    throw new Exception();
            }
        }

        (int x, int y) Move((int x, int y) pos, (int dX, int dY) facing, int distance)
        {
            return (pos.x + (facing.dX * distance), pos.y + (facing.dY * distance));
        }

        long SolvePart1(string[] path)
        {
            (int x, int y) pos = (0, 0);
            (int dX, int dY) facing = (0, 1);

            foreach (var direction in path.Select(v => v.Trim()))
            {
                facing = Turn(direction[0], facing);
                pos = Move(pos, facing, int.Parse(direction.Substring(1)));
            }

            return Math.Abs(pos.x) + Math.Abs(pos.y);
        }

        long SolvePart2(string[] path)
        {
            HashSet<(int, int)> visited = new();
            //List<(int, int)> visited = new();
            (int x, int y) pos = (0, 0);
            (int dX, int dY) facing = (0, 1);

            visited.Add(pos);

            foreach (var direction in path.Select(v => v.Trim()))
            {
                facing = Turn(direction[0], facing);
                int distance = int.Parse(direction.Substring(1));
                for (int i = 0; i < distance; i++)
                {
                    pos = (pos.x + facing.dX, pos.y + facing.dY);
                    if (visited.Contains(pos)) return Math.Abs(pos.x) + Math.Abs(pos.y);
                    visited.Add(pos);
                }
            }

            return Math.Abs(pos.x) + Math.Abs(pos.y);
        }

        [Theory]
        [InlineData("R2, L3", 5)]
        [InlineData("R2, R2, R2", 2)]
        public void Part1_Examples(string path, long expectedAnswer)
        {
            long distance = SolvePart1(path.Split(','));
            distance.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day01_Test.txt", 12)]
        [InlineData("Data/Day01.txt", 246)]
        public void Part1(string filename, long expectedAnswer)
        {
            long distance = SolvePart1(FileIterator.LoadCSV<string>(filename));
            distance.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("R8, R4, R4, R8", 4)]
        public void Part2_Examples(string path, long expectedAnswer)
        {
            long distance = SolvePart2(path.Split(','));
            distance.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day01.txt", 124)]
        public void Part2(string filename, long expectedAnswer)
        {
            long distance = SolvePart2(FileIterator.LoadCSV<string>(filename));
            distance.Should().Be(expectedAnswer);
        }
    }
}
