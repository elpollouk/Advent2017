using FluentAssertions;
using System.Collections.Generic;
using Utils;
using Xunit;

namespace Advent2018
{
    public class Day01
    {
        [Theory]
        [InlineData(3, +1, -2, +3, +1)]
        [InlineData(3, +1, +1, +1)]
        [InlineData(0, +1, +1, -2)]
        [InlineData(-6, -1, -2, -3)]
        public void Problem1_Test(int expectedFreq, params int[] deltas)
        {
            var freq = 0;
            foreach (var delta in deltas)
                freq += delta;

            freq.Should().Be(expectedFreq);
        }

        [Fact]
        public void Problem1_Solution()
        {
            var deltas = FileIterator.LoadLines<int>("Day01.txt");
            Problem1_Test(540, deltas);
        }

        [Theory]
        [InlineData(2, +1, -2, +3, +1)]
        [InlineData(0, +1, -1)]
        [InlineData(10, +3, +3, +4, -2, -4)]
        [InlineData(5, -6, +3, +8, +5, -6)]
        [InlineData(14, +7, +7, -2, -7, -4)]
        public void Problem2_Test(int expectedFreq, params int[] deltas)
        {
            var seen = new HashSet<int>();
            var deltaIndex = 0;
            var freq = 0;
            while (!seen.Contains(freq))
            {
                seen.Add(freq);
                freq += deltas.GetAtMod(deltaIndex++);
            }

            freq.Should().Be(expectedFreq);
        }

        [Fact]
        public void Problem2_Solution()
        {
            var deltas = FileIterator.LoadLines<int>("Day01.txt");
            Problem2_Test(73056, deltas);
        }
    }
}
