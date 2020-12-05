using FluentAssertions;
using System.Linq;
using Utils;
using Xunit;


namespace Advent2020
{
    public class Day05
    {
/*
        int ParseSeatId(string raw)
        {
            // Decode row
            var min = 0;
            var max = 128;
            var mid = 64;

            for (var i = 0; i < 7; i++)
            {
                if (raw[i] == 'F') max = mid;
                else min = mid;
                mid = (min + max) / 2;
            }
            var row = mid;

            // Decode column
            min = 0;
            max = 8;
            mid = 4;

            for (var i = 7; i < 10; i++)
            {
                if (raw[i] == 'L') max = mid;
                else min = mid;
                mid = (min + max) / 2;
            }

            // Encode seat id
            return (row * 8) + mid;
        }
*/
        int ParseSeatId(string raw)
        {
            var result = 0;
            foreach (var c in raw)
            {
                result <<= 1;
                if (c == 'B' || c == 'R') result |= 1;
            }
            return result;
        }

        [Theory]
        [InlineData("FBFBBFFRLR", 357)]
        [InlineData("BFFFBBFRRR", 567)]
        [InlineData("FFFBBBFRRR", 119)]
        [InlineData("BBFFBBFRLL", 820)]
        public void Test_ParseSeatId(string input, int expected) => ParseSeatId(input).Should().Be(expected);

        [Fact]
        public void Problem1()
        {
            FileIterator.Lines("Data/Day05.txt")
                .Select(ParseSeatId)
                .Max()
                .Should().Be(911);
        }

        [Fact]
        public void Problem2()
        {
            var seats = new bool[128 * 8];
            FileIterator.ForEachLine("Data/Day05.txt", (string line) => seats[ParseSeatId(line)] = true);

            var searching = false;
            var seatId = 0;
            foreach (var taken in seats)
            {
                if (searching && !taken) break;
                searching = taken;
                seatId++;
            }

            seatId.Should().Be(629);
        }
    }
}
