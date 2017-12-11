using Adevent2017.Utils;
using FluentAssertions;
using System.IO;
using Xunit;

namespace Adevent2017
{
    public class Problem1101
    {
        int Solve(string intput)
        {
            return -1;
        }

        [Theory]
        [InlineData("Data/1101-example.txt", 0)]
        [InlineData("Data/1101.txt", 0)]
        public void Solution(string input, int answer)
        {
            Solve(input).Should().Be(answer);
        }
    }
}
