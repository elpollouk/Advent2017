﻿using FluentAssertions;
using System;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2021
{
    public class Day10
    {
        private const long OK = 0;
        private const long INCOMPLETE = -1;
        private const long CORRUPT = -2;
        
        static long ScoreCorrupt(char closer)
        {
            return closer switch {
                ')' => 3,
                ']' => 57,
                '}' => 1197,
                '>' => 25137,
                _ => throw new InvalidOperationException()
            };
        }

        static long ScoreIncomplete(char opener)
        {
            return opener switch
            {
                '(' => 1,
                '[' => 2,
                '{' => 3,
                '<' => 4,
                _ => throw new InvalidOperationException()
            };
        }

        static char GetCloser(char opener)
        {
            return opener switch
            {
                '(' => ')',
                '{' => '}',
                '[' => ']',
                '<' => '>',
                _ => throw new InvalidOperationException()
            };
        }

        static long ParsePart1(string text, ref int index)
        {
            var opener = text[index];
            var closer = GetCloser(opener);
            while (++index < text.Length)
            {
                char current = text[index];
                if (current == closer)
                {
                    return OK;
                }

                switch (current)
                {
                    case '(':
                    case '[':
                    case '{':
                    case '<':
                        var result = ParsePart1(text, ref index);
                        if (result != OK) return result;
                        break;

                    default:
                        return ScoreCorrupt(current);
                }

            }

            return INCOMPLETE;
        }

        static long ParsePart1(string text)
        {
            int index = 0;
            do
            {
                var result = ParsePart1(text, ref index);
                if (result != OK) return result;
            }
            while (++index < text.Length);
            return OK;
        }

        static long ParsePart2(string text, ref int index)
        {
            var opener = text[index];
            var closer = GetCloser(opener);
            while (++index < text.Length)
            {
                char current = text[index];
                if (current == closer)
                {
                    return OK;
                }

                switch (current)
                {
                    case '(':
                    case '[':
                    case '{':
                    case '<':
                        var result = ParsePart2(text, ref index);
                        if (result != OK) return result == CORRUPT ? result : (result * 5) + ScoreIncomplete(opener);
                        break;

                    default:
                        return CORRUPT;
                }

            }

            return ScoreIncomplete(opener);
        }

        static long ParsePart2(string text)
        {
            int index = 0;
            do
            {
                var result = ParsePart2(text, ref index);
                if (result != OK) return result;
            }
            while (++index < text.Length);
            return OK;
        }

        [Theory]
        [InlineData("[({(<(())[]>[[{[]{<()<>>", 288957)]
        [InlineData("[(()[<>])]({[<{<<[]>>(", 5566)]
        [InlineData("(((({<>}<{<{<>}{[]{[]{}", 1480781)]
        [InlineData("{<[[]]>}<{[{[{[]{()[[[]", 995444)]
        [InlineData("<{([{{}}[<[[[<>{}]]]>[]]", 294)]
        public void Part2Examples(string example, long expectedAnswer)
        {
            ParsePart2(example).Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day10_Test.txt", 26397)]
        [InlineData("Data/Day10.txt", 364389)]
        public void Part1(string filename, long expectedAnswer)
        {
            FileIterator.Lines<string>(filename)
                .Select(line => ParsePart1(line))
                .Where(v => v > 0)
                .Sum()
                .Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day10_Test.txt", 288957)]
        [InlineData("Data/Day10.txt", 2870201088)]
        public void Part2(string filename, long expectedAnswer)
        {
            var scores = FileIterator.Lines<string>(filename)
                .Select(line => ParsePart2(line))
                .Where(v => v > 0)
                .OrderBy(v => v)
                .ToList();

            scores[scores.Count / 2].Should().Be(expectedAnswer);
        }
    }
}
