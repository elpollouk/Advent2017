using FluentAssertions;
using Xunit;

namespace Advent2019
{
    public class Day04
    {
        private static bool IsValidPassword1(int password)
        {
            var s = $"{password}";
            if (s.Length != 6) return false;

            var lastChar = (char)0;
            bool hasRepeat = false;
            foreach (var c in s)
            {
                if (c < lastChar) return false;
                if (c == lastChar) hasRepeat = true;
                lastChar = c;
            }

            return hasRepeat;
        }

        private static bool IsValidPassword2(int password)
        {
            var s = $"{password}";
            if (s.Length != 6) return false;

            var lastChar = (char)0;
            int repeatCount = 0;
            bool hasRepeat = false;
            foreach (var c in s)
            {
                if (c < lastChar) return false;
                if (c == lastChar)
                {
                    repeatCount++;
                }
                else
                {
                    if (repeatCount == 1) hasRepeat = true;
                    repeatCount = 0;
                }
                lastChar = c;
            }

            if (repeatCount == 1) hasRepeat = true;

            return hasRepeat;
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(99999, false)]
        [InlineData(111111, true)]
        [InlineData(223450, false)]
        [InlineData(123789, false)]
        [InlineData(122345, true)]
        [InlineData(135679, false)]
        [InlineData(1000000, false)]
        public void TestIsValidPassword1(int password, bool expected)
        {
            IsValidPassword1(password).Should().Be(expected);
        }

        [Theory]
        [InlineData(112233, true)]
        [InlineData(123444, false)]
        [InlineData(111122, true)]
        public void TestIsValidPassword2(int password, bool expected)
        {
            IsValidPassword2(password).Should().Be(expected);
        }

        [Theory]
        [InlineData(137683, 596253, 1864)]
        public void Problem1(int from, int to, int answer)
        {
            var count = 0;
            for (var i = from; i <= to; i++)
                if (IsValidPassword1(i))
                    count++;

            count.Should().Be(answer);
        }

        [Theory]
        [InlineData(137683, 596253, 1258)]
        public void Problem2(int from, int to, int answer)
        {
            var count = 0;
            for (var i = from; i <= to; i++)
                if (IsValidPassword2(i))
                    count++;

            count.Should().Be(answer);
        }
    }
}
