using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2022
{
    public class Day06
    {
        int EndOfUnique(string input, int markerLength)
        {
            Dictionary<char, int> window = new();
            for (int i = 0; i < markerLength; i++)
                window.Increment(input[i]);

            if (window.Count == markerLength)
                return markerLength;

            for (int i = markerLength; i < input.Length; i++)
            {
                window.DecrementAndRemoveZero(input[i - markerLength]);
                window.Increment(input[i]);
                if (window.Count == markerLength)
                    return i + 1;
            }

            throw new Exception();
        }

        [Theory]
        [InlineData("mjqjpqmgbljsphdztnvjfqwrcgsmlb", 4, 7)]
        [InlineData("bvwbjplbgvbhsrlpgdmjqwftvncz", 4, 5)]
        [InlineData("nppdvjthqldpwncqszvftbrmjlhg", 4, 6)]
        [InlineData("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", 4, 10)]
        [InlineData("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", 4, 11)]
        [InlineData("mjqjpqmgbljsphdztnvjfqwrcgsmlb", 14, 19)]
        [InlineData("bvwbjplbgvbhsrlpgdmjqwftvncz", 14, 23)]
        [InlineData("nppdvjthqldpwncqszvftbrmjlhg", 14, 23)]
        [InlineData("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", 14, 29)]
        [InlineData("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", 14, 26)]
        public void Examples(string input, int markerLength, int expectedAnswer)
        {
            var result = EndOfUnique(input, markerLength);
            result.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day06.txt", 4, 1282)]
        [InlineData("Data/Day06.txt", 14, 3513)]
        public void Solution(string filename, int markerLength, int expectedAnswer)
        {
            var input = FileIterator.Lines(filename).First();
            var result = EndOfUnique(input, markerLength);
            result.Should().Be(expectedAnswer);
        }
    }
}
