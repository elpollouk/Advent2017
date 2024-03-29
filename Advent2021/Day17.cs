﻿using FluentAssertions;
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

        static IEnumerable<int> RangeX(int minX, int maxX)
        {
            for (int x = 1; x <= maxX; x++)
            {
                if (SumNatural(x) < minX) continue;
                yield return x;
            }
        }

        static IEnumerable<int> RangeY(int minY, int maxY)
        {
            maxY = (-minY) - 1;

            do
            {
                yield return minY;
                minY++;
            }
            while (minY <= maxY);
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
            long y = (-minY) - 1;
            long maxHeight = SumNatural(y);
            maxHeight.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData(20, 30, -5, -10, 112)]
        [InlineData(155, 182, -67, -117, 2313)]
        public void Part2(int minX, int maxX, int maxY, int minY, int expectedAnswer)
        {
            var count = 0;
            foreach (var dX in RangeX(minX, maxX))
                foreach (var dY in RangeY(minY, maxY))
                    if (FullSimulate(minX, maxX, minY, maxY, dX, dY))
                        count++;
            
            count.Should().Be(expectedAnswer);
        }
    }
}
