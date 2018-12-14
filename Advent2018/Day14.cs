using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
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

        bool CompareSequence(IList<int> expected, IList<int> searchedList, int searchedListOffset)
        {
            if (searchedListOffset < 0)
                return false;

            for (var i = 0; i < expected.Count; i++)
            {
                if (searchedList.Count <= searchedListOffset + i)
                    return false;

                if (expected[i] != searchedList[searchedListOffset + i])
                    return false;
            }
            return true;
        }

        int FindOffsetInExpansion(string searchFor, params int[] initial)
        {
            var searchForList = new List<int>();
            foreach (var c in searchFor)
                searchForList.Add(c - '0');

            var list = new List<int>(initial);
            var elves = new int[] { 0, 1 };

            while (true)
            {
                var sum = 0;
                for (var i = 0; i < elves.Length; i++)
                {
                    var score = list[elves[i]];
                    sum += score;
                    elves[i] += (score + 1);
                }
                foreach (var i in SplitNumber(sum))
                {
                    list.Add(i);
                    var offset = list.Count - searchForList.Count;
                    if (CompareSequence(searchForList, list, offset))
                        return offset;
                }

                for (var i = 0; i < elves.Length; i++)
                    elves[i] = elves[i] % list.Count;
            }
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(3, 3)]
        [InlineData(10, 1, 0)]
        [InlineData(18, 1, 8)]
        [InlineData(20, 2, 0)]
        void SplitNumber_Test(int number, params int[] expected) => SplitNumber(number).Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        [Theory]
        [InlineData(9, "5158916779")]
        [InlineData(5, "0124515891")]
        [InlineData(18, "9251071085")]
        [InlineData(2018, "5941429882")]
        [InlineData(909441, "2615161213")] // Part 1 solution
        void Part1(int offset, string expectedLastTen)
        {
            IList<int> scores = Expand(offset + 10, 3, 7);

            var lastTen = "";
            foreach (var i in scores.Skip(offset).Take(10))
                lastTen += $"{i}";

            lastTen.Should().Be(expectedLastTen);
        }

        [Theory]
        [InlineData("51589", 9)]
        [InlineData("01245", 5)]
        [InlineData("92510", 18)]
        [InlineData("59414", 2018)]
        [InlineData("909441", 20403320)] // Part 2 Solution
        void Part2(string searchFor, int expectedOffset) => FindOffsetInExpansion(searchFor, 3, 7).Should().Be(expectedOffset);
    }
}
