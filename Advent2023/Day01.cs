using FluentAssertions;
using Utils;
using Xunit;

namespace Advent2023
{
    public class Day01
    {
        private readonly TrieMatcher<int>.MatchFunc numberMatcher = new TrieMatcher<int>(-1)
            .AddSequence("0",     0)
            .AddSequence("1",     1)
            .AddSequence("2",     2)
            .AddSequence("3",     3)
            .AddSequence("4",     4)
            .AddSequence("5",     5)
            .AddSequence("6",     6)
            .AddSequence("7",     7)
            .AddSequence("8",     8)
            .AddSequence("9",     9)
            .AddSequence("one",   1)
            .AddSequence("two",   2)
            .AddSequence("three", 3)
            .AddSequence("four",  4)
            .AddSequence("five",  5)
            .AddSequence("six",   6)
            .AddSequence("seven", 7)
            .AddSequence("eight", 8)
            .AddSequence("nine",  9)
            .Build();

        int ExtractNumber1(string text)
        {
            int first = -1;
            int last = -1;
            foreach (var c in text)
            {
                if ('0' <= c && c <= '9')
                {
                    last = c - '0';
                    if (first < 0)
                    {
                        first = last;
                    }
                }
            }

            return (first * 10) + last;
        }

        int ExtractNumber2(string text)
        {
            int first = -1;
            for (int i = 0; i <  text.Length; i++)
            {
                first = numberMatcher(text, i);
                if (first != -1)
                {
                    break;
                }
            }

            int last = -1;
            for (int i = text.Length - 1; i >= 0; i--)
            {
                last = numberMatcher(text, i);
                if (last != -1)
                {
                    break;
                }
            }

            return (first * 10) + last;
        }

        [Theory]
        [InlineData("Data/Day01_Test1.txt", 142)]
        [InlineData("Data/Day01.txt", 54697)]
        public void Part1(string filename, long expectedAnswer)
        {
            long total = 0;
            foreach (var line in FileIterator.Lines(filename))
            {
                total += ExtractNumber1(line);
            }
            total.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day01_Test2.txt", 281)]
        [InlineData("Data/Day01.txt", 54885)]
        public void Part2(string filename, long expectedAnswer)
        {
            long total = 0;
            foreach (var line in FileIterator.Lines(filename))
            {
                total += ExtractNumber2(line);
            }
            total.Should().Be(expectedAnswer);
        }
    }
}
