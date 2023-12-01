using FluentAssertions;
using Utils;
using Xunit;

namespace Advent2023
{
    public class Day01
    {
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

        int ExtractWrittenNumber(string text, int index)
        {
            var substr = text.Substring(index);
            if (substr.StartsWith("one"))
            {
                return 1;
            }
            else if (substr.StartsWith("two"))
            {
                return 2;
            }
            else if (substr.StartsWith("three"))
            {
                return 3;
            }
            else if (substr.StartsWith("four"))
            {
                return 4;
            }
            else if (substr.StartsWith("five"))
            {
                return 5;
            }
            else if (substr.StartsWith("six"))
            {
                return 6;
            }
            else if (substr.StartsWith("seven"))
            {
                return 7;
            }
            else if (substr.StartsWith("eight"))
            {
                return 8;
            }
            else if (substr.StartsWith("nine"))
            {
                return 9;
            }
            return -1;
        }

        int ExtractNumber2(string text)
        {
            int first = -1;
            for (int i = 0; i <  text.Length; i++)
            {
                if ('0' <= text[i] && text[i] <= '9')
                {
                    first = text[i] - '0';
                    break;
                }
                first = ExtractWrittenNumber(text, i);
                if (first != -1)
                {
                    break;
                }
            }

            int last = -1;
            for (int i = text.Length - 1; i >= 0; i--)
            {
                if ('0' <= text[i] && text[i] <= '9')
                {
                    last = text[i] - '0';
                    break;
                }
                last = ExtractWrittenNumber(text, i);
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
