using FluentAssertions;
using System;
using System.Collections.Generic;
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

        static long ParseChunk(string text, ref int index, char opener, bool part1)
        {
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
                        var result = ParseChunk(text, ref index, current, part1);
                        if (result != OK)
                        {
                            if (part1) return result;
                            return result == CORRUPT ? result : (result * 5) + ScoreIncomplete(opener);
                        }

                        break;

                    default:
                        return part1 ? ScoreCorrupt(current) : CORRUPT;
                }

            }

            return part1 ? INCOMPLETE : ScoreIncomplete(opener);
        }

        [Theory]
        [InlineData("Data/Day10_Test.txt", 26397)]
        [InlineData("Data/Day10.txt", 364389)]
        public void Part1(string filename, long expectedAnswer)
        {
            long total = 0;
            FileIterator.ForEachLine<string>(filename, line =>
            {
                int index = 0;
                var result = ParseChunk(line, ref index, line[0], true);
                if (result > 0) total += result;
            });

            total.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day10_Test.txt", 288957)]
        [InlineData("Data/Day10.txt", 2870201088)]
        public void Part2(string filename, long expectedAnswer)
        {
            List<long> scores = new();
            FileIterator.ForEachLine<string>(filename, line =>
            {
                int index = 0;
                var result = ParseChunk(line, ref index, line[0], false);
                if (result != CORRUPT && result != OK) scores.Add(result);
            });

            scores = scores.OrderBy(x => x).ToList();
            scores[scores.Count / 2].Should().Be(expectedAnswer);
        }
    }
}
