using FluentAssertions;
using Xunit;

namespace Utils
{
    public class TrieMatcherTests
    {
        [Fact]
        public void MatchSingleSequence()
        {
            var matcher = new TrieMatcher<int>(-3)
                .AddSequence("abc", 5);

            matcher.Match("abcd").Should().Be(5);
            matcher.Match("abc").Should().Be(5);
            matcher.Match("ab").Should().Be(-3);
            matcher.Match("efgh").Should().Be(-3);
        }

        [Fact]
        public void MatchMultipleSequences()
        {
            var matcher = new TrieMatcher<int>(-7)
                .AddSequence("abc", 11)
                .AddSequence("defg", 13);

            matcher.Match("abcdefg").Should().Be(11);
            matcher.Match("abcdefg", 3).Should().Be(13);
            matcher.Match("hijk").Should().Be(-7);
        }

        [Fact]
        public void EmptyStringMatch()
        {
            var matcher = new TrieMatcher<string>("foo")
                .AddSequence("", "baz");

            matcher.Match("").Should().Be("baz");
            matcher.Match("zzz").Should().Be("baz");
        }

        [Fact]
        public void NestedMatches()
        {
            var matcher = new TrieMatcher<string>("X")
                .AddSequence("abc", "C")
                .AddSequence("a", "A")
                .AddSequence("ab", "B");

            matcher.Match("a").Should().Be("A");
            matcher.Match("ab").Should().Be("B");
            matcher.Match("abc").Should().Be("C");
            matcher.Match("zzz").Should().Be("X");
        }
    }
}
