using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2023
{
    public class Day09
    {
        (long[], bool) Reduce(long[] data)
        {
            bool allEqual = true;
            var output = new long[data.Length - 1];

            for (var i = 0; i < data.Length - 1; i++)
            {
                output[i] = data[i+1] - data[i];
                if (i != 0 && output[i] != output[i-1])
                {
                    allEqual = false;
                }
            }

            return (output, allEqual);
        }

        long Predict(long[] data)
        {
            Stack<long> rollupValues = new();
            bool allEqual = false;

            rollupValues.Push(data[0]);
            while (!allEqual)
            {
                (data, allEqual) = Reduce(data);
                rollupValues.Push(data[0]);
            }

            var value = rollupValues.Pop();
            while (rollupValues.Count > 0)
            {
                value = rollupValues.Pop() - value;
            }

            return value;
        }

        [Theory]
        [InlineData("Data/Day09_Test1.txt", 114)]
        [InlineData("Data/Day09.txt", 1725987467)]
        public void Part1(string filename, long expectedAnswer)
        {
            long total = 0;
            foreach (var line in FileIterator.Lines(filename))
            {
                var data = line.Split(' ').Select(long.Parse).Reverse().ToArray();
                total += Predict(data);
            }
            total.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day09_Test2.txt", 5)]
        [InlineData("Data/Day09.txt", 971)]
        public void Part2(string filename, long expectedAnswer)
        {
            long total = 0;
            foreach (var line in FileIterator.Lines(filename))
            {
                var data = line.Split(' ').Select(long.Parse).ToArray();
                total += Predict(data);
            }
            total.Should().Be(expectedAnswer);
        }
    }
}
