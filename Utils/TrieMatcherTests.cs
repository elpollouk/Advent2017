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
                .AddSequence("abc", 5)
                .Build();

            matcher("abcd").Should().Be(5);
            matcher("abc").Should().Be(5);
            matcher("ab").Should().Be(-3);
            matcher("efgh").Should().Be(-3);
        }

        [Fact]
        public void MatchMultipleSequences()
        {
            var matcher = new TrieMatcher<int>(-7)
                .AddSequence("abc", 11)
                .AddSequence("defg", 13)
                .Build();

            matcher("abcdefg").Should().Be(11);
            matcher("abcdefg", 3).Should().Be(13);
            matcher("hijk").Should().Be(-7);
        }

        [Fact]
        public void MatchSequenceLongerThanInputSequence()
        {
            var matcher = new TrieMatcher<int>(-7)
                .AddSequence("abcdef", 11)
                .Build();

            matcher("abcd").Should().Be(-7);
            matcher("zyx").Should().Be(-7);
            matcher("").Should().Be(-7);
        }

        [Fact]
        public void EmptyStringsAlwaysMatch()
        {
            var matcher = new TrieMatcher<string>("foo")
                .AddSequence("", "baz")
                .Build();

            matcher("").Should().Be("baz");
            matcher("zzz").Should().Be("baz");
        }

        [Fact]
        public void NestedMatchesAreGreedy()
        {
            var matcher = new TrieMatcher<string>("X")
                .AddSequence("abc", "C")
                .AddSequence("a", "A")
                .AddSequence("ab", "B")
                .Build();

            matcher("a").Should().Be("A");
            matcher("ab").Should().Be("B");
            matcher("abc").Should().Be("C");
            matcher("zzz").Should().Be("X");
        }

        [Fact]
        public void PartialMatchNotFound()
        {
            var matcher = new TrieMatcher<bool>(false)
                .AddSequence("abc", true)
                .Build();

            matcher("abd").Should().BeFalse();
        }
    }
}
