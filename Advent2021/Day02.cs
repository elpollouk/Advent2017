using FluentAssertions;
using System;
using System.Collections.Generic;
using Utils;
using Xunit;

namespace Advent2021
{
    public class Day02
    {
        enum Direction
        {
            Forward,
            Up,
            Down,
        }

        record Move(Direction Direction, int Distance);

        static Move[] Parse(string inputFile)
        {
            List<Move> moves = new();

            FileIterator.ForEachLine<string>(inputFile, line => {
                var parts = line.Split(" ");
                moves.Add(new (parts[0] switch
                {
                    "forward" => Direction.Forward,
                    "up" => Direction.Up,
                    "down" => Direction.Down,
                    _ => throw new InvalidOperationException()
                }, int.Parse(parts[1])));
            });

            return moves.ToArray();
        }

        [Theory]
        [InlineData("Data/Day02_Test.txt", 150)]
        [InlineData("Data/Day02.txt", 1936494)]
        public void Part1(string inputFile, long expectedAnswer)
        {
            var x = 0L;
            var y = 0L;
            var moves = Parse(inputFile);
            foreach (var move in moves)
            {
                switch (move.Direction)
                {
                    case Direction.Forward:
                        x += move.Distance;
                        break;

                    case Direction.Up:
                        y -= move.Distance;
                        break;

                    case Direction.Down:
                        y += move.Distance;
                        break;
                }
            }

            (x * y).Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day02_Test.txt", 900)]
        [InlineData("Data/Day02.txt", 1997106066)]
        public void Part2(string inputFile, long expectedAnswer)
        {
            var x = 0L;
            var y = 0L;
            var aim = 0L;
            var moves = Parse(inputFile);
            foreach (var move in moves)
            {
                switch (move.Direction)
                {
                    case Direction.Forward:
                        x += move.Distance;
                        y += aim * move.Distance;
                        break;

                    case Direction.Up:
                        aim -= move.Distance;
                        break;

                    case Direction.Down:
                        aim += move.Distance;
                        break;
                }
            }

            (x * y).Should().Be(expectedAnswer);
        }
    }
}
