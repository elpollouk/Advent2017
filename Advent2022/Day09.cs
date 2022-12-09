using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2022
{
    public class Day09
    {
        bool IsTouching((int x, int y) head, (int x, int y) tail)
        {
            if (head.x < tail.x-1 || tail.x+1 < head.x) return false;
            if (head.y < tail.y-1 || tail.y+1 < head.y) return false;
            return true;
        }

        (int, int) Move((int x, int y) pos, char dir)
        {
            switch (dir)
            {
                case 'U': return (pos.x, pos.y + 1);
                case 'D': return (pos.x, pos.y - 1);
                case 'L': return (pos.x - 1, pos.y);
                case 'R': return (pos.x + 1, pos.y);
            }
            throw new Exception();
        }

        (int, int) UpdateTail((int x, int y) head, (int x, int y) tail)
        {
            var dx = head.x - tail.x;
            var dy = head.y - tail.y;

            if (dx < -1) dx = -1;
            else if (dx > 1) dx = 1;
            if (dy < -1) dy = -1;
            else if (dy > 1) dy = 1;

            return (tail.x + dx, tail.y + dy);
        }

        void Move(Dictionary<(int, int), int> visited, (int x, int y)[] rope, string op)
        {
            var dir = op[0];
            var dist = int.Parse(op.Substring(2));

            while (dist --> 0)
            {
                rope[0] = Move(rope[0], dir);
                for (int i = 1; i < rope.Length; i++)
                {
                    if (!IsTouching(rope[i-1], rope[i]))
                    {
                        rope[i] = UpdateTail(rope[i-1], rope[i]);
                    }
                }
                visited.Increment(rope.Last());
            }
        }

        [Theory]
        [InlineData("Data/Day09_Test.txt", 2, 13)]
        [InlineData("Data/Day09.txt", 2, 6642)]
        [InlineData("Data/Day09_Test.txt", 10, 1)]
        [InlineData("Data/Day09_Test2.txt", 10, 36)]
        [InlineData("Data/Day09.txt", 10, 2765)]
        public void Solution(string filename, int ropeLength, int expectedAnswer)
        {
            Dictionary<(int, int), int> visited = new();
            var rope = new (int, int)[ropeLength];
            rope.SetAll((0, 0));
            visited.Increment(rope.Last());

            foreach (var line in FileIterator.Lines(filename))
            {
                Move(visited, rope, line);
            }

            var total = visited.Keys.Count;
            total.Should().Be(expectedAnswer);
        }
    }
}
