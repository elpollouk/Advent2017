using FluentAssertions;
using Xunit;

namespace Utils
{
    public class BitFlagTests
    {
        [Theory]
        [InlineData(0, 0, 1)]
        [InlineData(0, 1, 2)]
        [InlineData(0, 63, long.MinValue)]
        [InlineData(5, 1, 7)]
        [InlineData(7, 1, 7)]
        public void TestSetFlagLong(long value, int flag, long expectedResult)
        {
            value.SetFlag(flag);
            value.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(7, 1, true, 7)]
        [InlineData(7, 1, false, 5)]
        [InlineData(5, 1, true, 7)]
        [InlineData(5, 1, false, 5)]
        public void TestSetFlagWithStateLong(long value, int flag, bool state, long expectedResult)
        {
            value.SetFlag(flag, state);
            value.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(1, 0, 0)]
        [InlineData(2, 1, 0)]
        [InlineData(long.MinValue, 63, 0)]
        [InlineData(7, 1, 5)]
        [InlineData(5, 1, 5)]
        public void TestClearFlagLong(long value, int flag, long expectedResult)
        {
            value.ClearFlag(flag);
            value.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(1, 0, true)]
        [InlineData(2, 1, true)]
        [InlineData(long.MinValue, 63, true)]
        [InlineData(7, 1, true)]
        [InlineData(5, 1, false)]
        public void TestCheckFlagLong(long value, int flag, bool expectedResult)
        {
            value.CheckFlag(flag).Should().Be(expectedResult);
        }
    }
}
