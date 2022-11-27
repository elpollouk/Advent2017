using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2016
{
    public class Day06
    {
        [Theory]
        [InlineData("Data/Day06_Test.txt", "easter")]
        [InlineData("Data/Day06.txt", "wkbvmikb")]
        public void Part1(string filename, string expectedAnswer)
        {
            Dictionary<char, int>[] counts = new Dictionary<char, int>[expectedAnswer.Length];
            for (int i = 0; i < counts.Length; i++)
                counts[i] = new();

            foreach (var line in FileIterator.Lines(filename))
            {
                for (int i = 0; i < line.Length; i++)
                {
                    counts[i].Increment(line[i]);
                }
            }

            string answer = "";
            foreach (var dic in counts)
            {
                answer += dic.Max();
            }

            answer.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day06_Test.txt", "advent")]
        [InlineData("Data/Day06.txt", "evakwaga")]
        public void Part2(string filename, string expectedAnswer)
        {
            Dictionary<char, int>[] counts = new Dictionary<char, int>[expectedAnswer.Length];
            for (int i = 0; i < counts.Length; i++)
                counts[i] = new();

            foreach (var line in FileIterator.Lines(filename))
            {
                for (int i = 0; i < line.Length; i++)
                {
                    counts[i].Increment(line[i]);
                }
            }

            string answer = "";
            foreach (var dic in counts)
            {
                answer += dic.Min();
            }

            answer.Should().Be(expectedAnswer);
        }
    }
}
