using FluentAssertions;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2020
{
    public class Day01
    {
        private int Calc2(int[] values, int target = 2020)
        {
            for (var i = 0; i < values.Length - 1; i++)
            {
                for (var j = i + 1; j < values.Length; j++)
                {
                    if (values[i] + values[j] != target) continue;
                    return values[i] * values[j];
                }
            }

            return -1;
        }

        private int Calc3(int[] values)
        {
            foreach (var value in values)
            {
                var target = 2020 - value;
                var result = Calc2(values, target);
                if (result == -1) continue;
                return result * value;
            }

            return -1;
        }

        [Theory]
        [InlineData(514579, new int[] { 1721, 979, 366, 299, 675, 1456 })]
        public void Part1_Test(int expected, int[] values)
        {
            Calc2(values).Should().Be(expected);
        }

        [Theory]
        [InlineData(241861950, new int[] { 1721, 979, 366, 299, 675, 1456 })]
        public void Part2_Test(int expected, int[] values)
        {
            Calc3(values).Should().Be(expected);
        }

        [Fact]
        public void Part1_Promblem()
        {
            var values = FileIterator.Lines<int>("Data/Day01.txt").ToArray();
            Calc2(values).Should().Be(889779);
        }

        [Fact]
        public void Part2_Promblem()
        {
            var values = FileIterator.Lines<int>("Data/Day01.txt").ToArray();
            Calc3(values).Should().Be(76110336);
        }
    }
}
