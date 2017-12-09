using Adevent2017.Utils;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Adevent2017
{
    public class Problem0901
    {
        int Solve(string[] input)
        {
            return -1;
        }

        [Theory]
        [InlineData("Data/0901-example.txt", 1)]
        [InlineData("Data/0901.txt", 3089)]
        public void Part1(string input, int answer)
        {
            var lines = FileIterator.LoadLines<string>(input);
            Solve(lines).Should().Be(answer);
        }

        [Theory]
        [InlineData("Data/0901-example.txt", 10)]
        [InlineData("Data/0901.txt", 5391)]
        public void Part2(string input, int answer)
        {
            var lines = FileIterator.LoadLines<string>(input);
            Solve(lines).Should().Be(answer);
        }
    }
}
