using FluentAssertions;
using Utils;
using Xunit;

namespace Advent2019
{
    public class Day01
    {
        [Fact]
        void Problem1()
        {
            var total = 0;
            foreach (var value in FileIterator.Lines<int>("Data/Day01.txt"))
                total += (value / 3) - 2;

            total.Should().Be(3239503);
        }

        int CalculateFuel(int mass)
        {
            var req = (mass / 3) - 2;
            if (req <= 0) return 0;
            req += CalculateFuel(req);
            return req;
        }

        [Fact]
        void Problem2()
        {
            var total = 0;
            foreach (var value in FileIterator.Lines<int>("Data/Day01.txt"))
                total += CalculateFuel(value);

            total.Should().Be(4856390);
        }
    }
}
