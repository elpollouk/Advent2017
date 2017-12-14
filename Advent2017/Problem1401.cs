using Adevent2017.Utils;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Adevent2017
{
    public class Problem1401
    {
        [Theory]
        [InlineData("Data/1401-example.txt", 24)]
        [InlineData("Data/1401.txt", 2160)]
        public void Part1(string datafile, int answer)
        {
            var lines = FileIterator.LoadLines<string>(datafile);
            lines.Length.Should().Be(answer);
        }

        [Theory]
        [InlineData("Data/1401-example.txt", 10)]
        [InlineData("Data/1401.txt", 3907470)]
        public void Part2(string datafile, int answer)
        {
            var lines = FileIterator.LoadLines<string>(datafile);
            lines.Length.Should().Be(answer);
        }
    }
}
