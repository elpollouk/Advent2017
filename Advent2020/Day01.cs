using FluentAssertions;
using Xunit;

namespace Advent2020
{
    public class Day01
    {
        [Theory]
        [InlineData(1, 0)]
        public void Part1(int a, int b)
        {
            (a - 1).Should().Be(b);
        }
    }
}
