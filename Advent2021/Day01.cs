using FluentAssertions;
using Utils;
using Xunit;

namespace Advent2021
{
    public class Day01
    {
        long CountIncreased(int[] readings)
        {
            var prev = readings[0];
            var count = 0L;

            for (var i = 1; i < readings.Length; i++)
            {
                if (prev < readings[i])
                {
                    count++;
                }
                prev = readings[i];
            }

            return count;
        }

        long CountIncreasedWindow(int[] readings)
        {
            var count = 0L;
            var window = readings[0] + readings[1] + readings[2];

            for (var i = 3; i < readings.Length; i++)
            {
                var prevWindow = window;
                window -= readings[i - 3];
                window += readings[i];
                if (prevWindow < window)
                {
                    count++;
                }
            }

            return count;
        }

        [Theory]
        [InlineData("Data/Day01_test.txt", 7)]
        [InlineData("Data/Day01.txt", 1482)]
        public void Part1(string inputFilename, long expectedAnswer)
        {
            var input = FileIterator.LoadLines<int>(inputFilename);
            CountIncreased(input).Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day01_test.txt", 5)]
        [InlineData("Data/Day01.txt", 1518)]
        public void Part2(string inputFilename, long expectedAnswer)
        {
            var input = FileIterator.LoadLines<int>(inputFilename);
            CountIncreasedWindow(input).Should().Be(expectedAnswer);
        }
    }
}
