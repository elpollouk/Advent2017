using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Xunit;

namespace Advent2018
{
    public class Day14
    {
        List<int> SplitNumber(int number)
        {
            var list = new List<int>();
            while (number != 0)
            {
                list.Insert(0, number % 10);
                number /= 10;
            }
            if (list.Count == 0)
                list.Add(0);
            return list;
        }

        List<int> Expand(int requiredNumber, params int[] initial)
        {
            var list = new List<int>(initial);
            var elves = new int[] { 0, 1 };

            while (list.Count < requiredNumber)
            {
                var sum = 0;
                for (var i = 0; i < elves.Length; i++)
                {
                    var score = list[elves[i]];
                    sum += score;
                    elves[i] += (score + 1);
                }
                foreach (var i in SplitNumber(sum))
                    list.Add(i);

                for (var i = 0; i < elves.Length; i++)
                    elves[i] = elves[i] % list.Count;
            }

            return list;
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(3, 3)]
        [InlineData(10, 1, 0)]
        [InlineData(18, 1, 8)]
        [InlineData(20, 2, 0)]
        void SplitNumber_Test(int number, params int[] expected) => SplitNumber(number).Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        [Theory]
        [InlineData(9, 10, "5158916779", 3, 7)]
        [InlineData(5, 10, "0124515891", 3, 7)]
        [InlineData(18, 10, "9251071085", 3, 7)]
        [InlineData(2018, 10, "5941429882", 3, 7)]
        [InlineData(909441, 10, "2615161213", 3, 7)] // Part 1 solution
        void Expand_Test(int offset, int count, string expectedLastTen, params int[] initialScores)
        {
            IList<int> scores = Expand(offset + count, initialScores);

            var lastTen = "";
            foreach (var i in scores.Skip(offset).Take(count))
                lastTen += $"{i}";

            lastTen.Should().Be(expectedLastTen);
        }
    }
}
