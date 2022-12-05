using FluentAssertions;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2022
{
    public class Day04
    {
        ((int, int), (int, int)) ParseRanges(string text)
        {
            var groups = text.Groups("(\\d+)-(\\d+),(\\d+)-(\\d+)");
            return (
                (int.Parse(groups[0]), int.Parse(groups[1])),
                (int.Parse(groups[2]), int.Parse(groups[3]))
            );
        }

        bool IsFullyContained((int l, int u) a, (int l, int u) b)
        {
            if (a.l <= b.l && b.u <= a.u) return true;
            if (b.l <= a.l && a.u <= b.u) return true;
            return false;
        }

        bool IsOverLapped((int l, int u) a, (int l, int u) b)
        {
            if (a.l <= b.l && b.l <= a.u) return true;
            if (b.l <= a.l && a.l <= b.u) return true;
            return false;
        }

        [Theory]
        [InlineData("Data/Day04_Test.txt", 2)]
        [InlineData("Data/Day04.txt", 462)]
        public void Part1(string filename, int expectedAnswer)
        {
            FileIterator.Lines(filename)
                .Select(ParseRanges)
                .Where(pair => IsFullyContained(pair.Item1, pair.Item2))
                .Count()
                .Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day04_Test.txt", 4)]
        [InlineData("Data/Day04.txt", 835)]
        public void Part2(string filename, int expectedAnswer)
        {
            FileIterator.Lines(filename)
                .Select(ParseRanges)
                .Where(pair => IsOverLapped(pair.Item1, pair.Item2))
                .Count()
                .Should().Be(expectedAnswer);
        }
    }
}
