using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2016
{
    public class Day07
    {
        enum ParserState
        {
            NORMAL,
            INNER
        }
        bool IsValidABBA(string text, int i)
        {
            if (text[i] == text[i+1]) return false;
            return text[i] == text[i+3] && text[i+1] == text[i+2];
        }

        bool IsValidABA(string text, int i)
        {
            if (text[i] == text[i + 1]) return false;
            return text[i] == text[i + 2];
        }

        bool IsValidTLS(string text)
        {
            ParserState state = ParserState.NORMAL;
            bool found = false;

            for (int i = 0; i < text.Length - 3; i++)
            {
                if (text[i] == '[')
                {
                    state = ParserState.INNER;
                }
                else if (text[i] == ']')
                {
                    state = ParserState.NORMAL;
                }
                else if (IsValidABBA(text, i))
                {
                    if (state == ParserState.INNER) return false;
                    found = true;
                }
            }
            return found;
        }

        bool IsValidSSL(string text)
        {
            ParserState state = ParserState.NORMAL;
            HashSet<string> outer = new();
            HashSet<string> inner = new();

            for (int i = 0; i < text.Length - 2; i++)
            {
                if (text[i] == '[')
                {
                    state = ParserState.INNER;
                }
                else if (text[i] == ']')
                {
                    state = ParserState.NORMAL;
                }
                else if (IsValidABA(text, i))
                {
                    if (state == ParserState.NORMAL)
                    {
                        outer.Add(text.Substring(i, 3));
                    }
                    else
                    {
                        inner.Add($"{text[i + 1]}{text[i]}{text[i + 1]}");
                    }
                }
            }

            return outer.Intersect(inner).Any();
        }

        [Theory]
        [InlineData("Data/Day07_Test.txt", 2)]
        [InlineData("Data/Day07.txt", 118)]
        public void Part1(string filename, long expectedAnswer)
        {
            long count = 0;

            foreach (var line in FileIterator.Lines(filename))
            {
                if (IsValidTLS(line))
                {
                    count++;
                }
            }

            count.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day07_Test.txt", 3)]
        [InlineData("Data/Day07.txt", 260)]
        public void Part2(string filename, long expectedAnswer)
        {
            long count = 0;

            foreach (var line in FileIterator.Lines(filename))
            {
                if (IsValidSSL(line))
                {
                    count++;
                }
            }

            count.Should().Be(expectedAnswer);
        }
    }
}
