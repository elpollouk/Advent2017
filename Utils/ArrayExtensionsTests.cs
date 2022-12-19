using FluentAssertions;
using System.Linq;
using Xunit;

namespace Utils
{
    public class ArrayExtensionsTests
    {
        [Fact]
        public void TestCombinations_1from4()
        {
            char[] source = { 'a', 'b', 'c', 'd' };
            var combinations = source.Combinations(1)
                .Select(v => v.Join(""))
                .ToArray();

            combinations.Length.Should().Be(4);
            combinations[0].Should().Be("a");
            combinations[1].Should().Be("b");
            combinations[2].Should().Be("c");
            combinations[3].Should().Be("d");
        }

        [Fact]
        public void TestCombinations_2from4()
        {
            char[] source = { 'a', 'b', 'c', 'd' };
            var combinations = source.Combinations(2)
                .Select(v => v.Join(""))
                .ToArray();

            combinations.Length.Should().Be(6);
            combinations[0].Should().Be("ab");
            combinations[1].Should().Be("ac");
            combinations[2].Should().Be("ad");
            combinations[3].Should().Be("bc");
            combinations[4].Should().Be("bd");
            combinations[5].Should().Be("cd");
        }

        [Fact]
        public void TestCombinations_3from4()
        {
            char[] source = { 'a', 'b', 'c', 'd' };
            var combinations = source.Combinations(3)
                .Select(v => v.Join(""))
                .ToArray();

            combinations.Length.Should().Be(4);
            combinations[0].Should().Be("abc");
            combinations[1].Should().Be("abd");
            combinations[2].Should().Be("acd");
            combinations[3].Should().Be("bcd");
        }

        [Fact]
        public void TestCombinations_4from4()
        {
            char[] source = { 'a', 'b', 'c', 'd' };
            var combinations = source.Combinations(4)
                .Select(v => v.Join(""))
                .ToArray();

            combinations.Length.Should().Be(1);
            combinations[0].Should().Be("abcd");
        }
    }
}
