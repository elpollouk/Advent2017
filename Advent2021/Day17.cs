using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Advent2021
{
    public class Day17
    {
        static long SumNatural(long range)
        {
            range *= (range + 1);
            range /= 2;
            return range;
        }

        static bool HitsX(int minX, int maxX, int dX)
        {
            if (SumNatural(dX) <= minX) return false;

            var x = 0;
            while (x <= maxX)
            {
                x += dX;
                dX--;
                if (minX <= x) return true;
            }

            return false;
        }

        static IEnumerable<int> Range(int min, int max)
        {
            do
            {
                yield return min;
                min++;
            }
            while (min <= max);
        }

        static bool FullSimulate(int minX, int maxX, int minY, int maxY, int dX, int dY)
        {
            int x = 0;
            int y = 0;
            bool Hit() => (minX <= x) && (maxY >= y);

            while (x <= maxX && y >= minY)
            {
                if (Hit()) return true;
                y += dY;
                x += dX;
                dY--;
                if (dX != 0) dX--;

            }
            return false;
        }

        [Theory]
        [InlineData(-10, 45)]
        [InlineData(-117, 6786)]
        public void Part1(int minY, long expectedAnswer)
        {
            long y = Math.Abs(minY) - 1;
            long maxHeight = SumNatural(y);
            maxHeight.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData(20, 30, -5, -10, 112)]
        [InlineData(155, 182, -67, -117, 2313)]
        public void Part2(int minX, int maxX, int maxY, int minY, int expectedAnswer)
        {
            int x = 0;
            List<int> initialX = new();
            while (++x <= maxX)
                if (HitsX(minX, maxX, x))
                    initialX.Add(x);

            var minDY = minY;
            var maxDY = Math.Abs(minY) - 1;

            var count = 0;
            foreach (var dY in Range(minDY, maxDY))
                foreach (var dX in initialX)
                    if (FullSimulate(minX, maxX, minY, maxY, dX, dY))
                        count++;

            count.Should().Be(expectedAnswer);
        }
    }
}
