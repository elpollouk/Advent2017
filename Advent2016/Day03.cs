using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2016
{
    public class Day03
    {
        int[] ParseTriangle(string text)
        {
            return text.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.Parse(s))
                .Order()
                .ToArray();

        }

        int[] ParseTriangle2(string text)
        {
            return text.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.Parse(s))
                .ToArray();

        }

        int[] Sort(params int[] values)
        {
            return values.Order().ToArray();
        }

        int[][] Transpose(List<int[]> buffer)
        {
            int[][] a = new int[3][];
            a[0] = Sort(buffer[0][0], buffer[1][0], buffer[2][0]);
            a[1] = Sort(buffer[0][1], buffer[1][1], buffer[2][1]);
            a[2] = Sort(buffer[0][2], buffer[1][2], buffer[2][2]);
            return a;
        }

        bool IsTriangle(int[] t) => t[2] < (t[0] + t[1]);

        [Theory]
        [InlineData(" 10  25  5", false)]
        public void IsTriangleTest(string text, bool expectedAnswer) => IsTriangle(ParseTriangle(text)).Should().Be(expectedAnswer);

        [Theory]
        [InlineData("Data/Day03.txt", 1032)]
        public void Part1(string filename, long expectedAnswer)
        {
            long count = 0;

            foreach (var line in FileIterator.Lines(filename))
            {
                var triangle = ParseTriangle(line);
                if (IsTriangle(triangle)) count++;
            }

            count.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day03.txt", 1838)]
        public void Part2(string filename, long expectedAnswer)
        {
            long count = 0;
            List<int[]> buffer = new();

            foreach (var line in FileIterator.Lines(filename))
            {
                var triangle = ParseTriangle2(line);
                buffer.Add(triangle);
                if (buffer.Count == 3)
                {
                    foreach (var t in Transpose(buffer))
                        if (IsTriangle(t))
                            count++;
                    buffer.Clear();
                }
            }

            count.Should().Be(expectedAnswer);
        }
    }
}
