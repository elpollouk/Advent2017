using Adevent2017.Utils;
using FluentAssertions;
using Xunit;

namespace Adevent2017
{
    public class Problem0801
    {
        int Solve(string input)
        {
            return -1;
        }

        [Theory]
        [InlineData("Data/0701-example.txt", 0)]
        [InlineData("Data/0701.txt", 0)]
        public void Part1(string input, int answer)
        {
            Solve(input).Should().Be(answer);
        }

        [Theory]
        [InlineData("Data/0701-example.txt", 60)]
        [InlineData("Data/0701.txt", 646)]
        public void Part2(string input, int answer)
        {
        }
    }
}
