using FluentAssertions;
using System.Collections.Generic;
using Utils;
using Xunit;

namespace Advent2020
{
    public class Day09
    {
        long FirstInvalid(string input, int windowSize)
        {
            var window = new List<long>();

            foreach (var value in FileIterator.Lines<long>(input))
            {
                if (window.Count == windowSize)
                {
                    var isValid = Day01.Calc2(window, value);
                    if (isValid < 0) return value;
                    window.Shift();
                }
                window.Add(value);
            }

            throw new Expletive("Fuck");
        }

        List<long> ContiguousSet(string input, long target)
        {
            var window = new List<long>();
            var windowSum = 0L;

            foreach (var value in FileIterator.Lines<long>(input))
            {
                if (value == target) continue; // I'm not sure if we're meant to search after the target number or not
                window.Add(value);
                windowSum += value;

                while (windowSum > target)
                    windowSum -= window.Shift();

                if (windowSum == target) return window;
            }

            throw new Expletive("Bollocks");
        }

        [Theory]
        [InlineData("Data/Day09_test.txt", 5, 127)]
        [InlineData("Data/Day09.txt", 25, 133015568)]
        public void Problem1(string input, int windowSize, long expected)
        {
            FirstInvalid(input, windowSize).Should().Be(expected);
        }

        [Theory]
        [InlineData("Data/Day09_test.txt", 127, 62)]
        [InlineData("Data/Day09.txt", 133015568, 16107959)]
        public void Problem2(string input, long target, long expected)
        {
            var set = ContiguousSet(input, target);
            var (min, max) = set.MinAndMax();
            (min + max).Should().Be(expected);
        }
    }
}
