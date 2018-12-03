using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Advent2017
{
    public class Problem1701
    {
        List<int> Solve1(int stepSize, int numCycles)
        {
            int currentPosition = 0;
            var buffer = new List<int>();
            buffer.Add(0);

            for (var i = 1; i < numCycles + 1; i++)
            {
                currentPosition = ((currentPosition + stepSize) % buffer.Count) + 1;
                buffer.Insert(currentPosition, i);
            }

            return buffer;
        }

        int Solve2(int stepSize, int numCycles)
        {
            var valueAt1 = -1;
            int bufferSize = 1;
            int currentPosition = 0;

            for (var i = 1; i < numCycles + 1; i++)
            {
                currentPosition = ((currentPosition + stepSize) % bufferSize) + 1;
                if (currentPosition == 1)
                    valueAt1 = i;
                bufferSize++;
            }

            return valueAt1;
        }

        [Fact]
        public void Part1Example()
        {
            var buffer = Solve1(3, 9);
            buffer.Count.Should().Be(10);
            buffer[0].Should().Be(0);
            buffer[1].Should().Be(9);
            buffer[2].Should().Be(5);
            buffer[3].Should().Be(7);
            buffer[4].Should().Be(2);
            buffer[5].Should().Be(4);
            buffer[6].Should().Be(3);
            buffer[7].Should().Be(8);
            buffer[8].Should().Be(6);
            buffer[9].Should().Be(1);
        }

        [Fact]
        public void Part1Solution()
        {
            var buffer = Solve1(380, 2017);
            buffer.Count.Should().Be(2018);
            var i = buffer.IndexOf(2017);
            buffer[i + 1].Should().Be(204);
        }

        [Theory]
        [InlineData(3, 9, 9)]
        [InlineData(380, 50000000, 28954211)]
        public void Part2Solution(int stepSize, int numCycles, int answer) => Solve2(stepSize, numCycles).Should().Be(answer);
    }
}
