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

        long Predict(string sequence, Func<long[],long> GetRollupValue, Func<long, long, long> UpdateRollupValue)
        {
            Stack<long> rollupValues = new();
            bool allEqual = false;
            var data = sequence.Split(' ').Select(long.Parse).ToArray();

            rollupValues.Push(GetRollupValue(data));
            while (!allEqual)
            {
                (data, allEqual) = Reduce(data);
                var rollupValue = GetRollupValue(data);
                rollupValues.Push(rollupValue);
            }

            var value = rollupValues.Pop();
            while (rollupValues.Count > 0)
            {
                value = UpdateRollupValue(value, rollupValues.Pop());
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
                total += Predict(
                    line,
                    values => values.Last(),
                    (rollup, sequenceValue) => sequenceValue + rollup
                );
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
                total += Predict(
                    line,
                    values => values[0],
                    (rollup, sequenceValue) => sequenceValue - rollup
                );
            }
            total.Should().Be(expectedAnswer);
        }
    }
}
