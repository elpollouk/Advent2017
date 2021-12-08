using FluentAssertions;
using System;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2021
{
    public class Day07
    {
        [Theory]
        [InlineData("Data/Day07_Test.txt", 37)]
        [InlineData("Data/Day07.txt", 359648)]
        public void Part1(string filename, long expectedAnswer)
        {
            var numbers = FileIterator.LoadCSV<int>(filename).OrderBy(v => v).ToArray();
            var mid = numbers[numbers.Length / 2];

            long total = 0;
            foreach (var number in numbers)
            {
                total += Math.Abs(mid - number);
            }

            total.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day07_Test.txt", 168)]
        [InlineData("Data/Day07.txt", 100727924)]
        public void Part2(string filename, long expectedAnswer)
        {
            var numbers = FileIterator.LoadCSV<int>(filename);
            var max = numbers.Max() + 1;
            var moveCosts = new int[numbers.Length, max];

            for (var crab = 0; crab < numbers.Length; crab++)
            {
                // Move left
                var costSoFar = 1;
                for (var i = numbers[crab] - 1; i >= 0; i--)
                {
                    moveCosts[crab, i] = costSoFar + moveCosts[crab, i+1];
                    costSoFar++;
                }

                // Move right
                costSoFar = 1;
                for (var i = numbers[crab] + 1; i < max; i++)
                {
                    moveCosts[crab, i] = costSoFar + moveCosts[crab, i-1];
                    costSoFar++;
                }
            }

            long minTotal = int.MaxValue;
            for (var i = 0; i < max; i++)
            {
                long total = 0;

                for (var j = 0; j < numbers.Length; j++)
                {
                    total += moveCosts[j, i];
                }

                if (total < minTotal) minTotal = total;
            }

            minTotal.Should().Be(expectedAnswer);
        }
    }
}
