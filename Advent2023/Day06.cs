using FluentAssertions;
using System;
using System.Collections.Generic;
using Utils;
using Xunit;

namespace Advent2023
{
    public class Day06
    {
        IEnumerable<(long duration, long target)> IterateRaces(string filename)
        {
            var reader = FileIterator.CreateLineReader(filename);
            var durations = reader().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var targets = reader().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < durations.Length; i++)
                yield return (long.Parse(durations[i]), int.Parse(targets[i]));
        }

        (long duration, long target) ParseAsSingleRace(string filename)
        {
            var reader = FileIterator.CreateLineReader(filename);
            var duration = int.Parse(reader().Split(':')[1].Replace(" ", ""));
            var target = long.Parse(reader().Split(':')[1].Replace(" ", ""));

            return(duration, target);
        }

        long CountPossibilities(long duration, long target)
        {
            long count = 0;

            for (int i = 1; i < duration; i++)
            {
                long distance = i * (duration - i);
                if (target < distance)
                {
                    count++;
                }
            }

            return count;
        }

        [Theory]
        [InlineData("Data/Day06_Test.txt", 288)]
        [InlineData("Data/Day06.txt", 588588)]
        public void Part1(string filename, long expectedAnswer)
        {
            long total = 1;

            foreach (var (duration, target) in IterateRaces(filename))
            {
                total *= CountPossibilities(duration, target);
            }

            total.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day06_Test.txt", 71503)]
        [InlineData("Data/Day06.txt", 34655848)]
        public void Part2(string filename, long expectedAnswer)
        {
            var (duration, target) = ParseAsSingleRace(filename);
            long total = CountPossibilities(duration, target);
            total.Should().Be(expectedAnswer);
        }
    }
}
