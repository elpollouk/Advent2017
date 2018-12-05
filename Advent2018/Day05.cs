using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using Utils;
using Xunit;

namespace Advent2018
{
    public class Day05
    {
        int UnitToValue(char unit)
        {
            if (('A' <= unit) && (unit <= 'Z'))
                return (unit - 'A') + 1; // We want the item to have a non-zero value
            return -((unit - 'a') + 1);
        }

        int Collapse(string input)
        {
            var list = new List<int>();
            foreach (var unit in input)
                list.Add(UnitToValue(unit));

            var i = 0;
            while (i < list.Count - 1)
            {
                if (list[i] + list[i+1] == 0)
                {
                    list.RemoveRange(i, 2);
                    if (i != 0) i--;
                }
                else
                {
                    i++;
                }
            }

            return list.Count;
        }

        [Theory]
        [InlineData("aA", 0)]
        [InlineData("Aa", 0)]
        [InlineData("abBA", 0)]
        [InlineData("aabAAB", 6)]
        [InlineData("dabAcCaCBAcCcaDA", 10)]
        public void Problem1_Test(string input, int expectedLength) => Collapse(input).Should().Be(expectedLength);

        [Fact]
        public void Problem1_Solution()
        {
            var input = File.ReadAllText("Data/Day05.txt");
            Collapse(input).Should().Be(9370);
        }
    }
}
