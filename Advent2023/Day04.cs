using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2023
{
    public class Day04
    {
        int CountWinningNumbers(string card)
        {
            var match = card.Match(@":([ \d]+)\|([ \d+]+)");
            var winning = match.Groups[1].Value;
            var have = match.Groups[2].Value;

            var winningNumbers = new HashSet<string>();
            var parts = winning.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            foreach (var number in parts)
            {
                winningNumbers.Add(number);
            }

            int count = 0;
            parts = have.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            foreach (var number in parts)
            {
                if (winningNumbers.Contains(number))
                {
                    count++;
                }
            }

            return count;
        }

        long ScoreCard(string card)
        {
            int wins = CountWinningNumbers(card);
            return (long)Math.Pow(2, wins - 1);
        }

        [Theory]
        [InlineData("Data/Day04_Test.txt", 13)]
        [InlineData("Data/Day04.txt", 21821)]
        public void Part1(string filename, long expectedAnswer)
        {
            long total = 0;
            foreach(var line in FileIterator.Lines(filename))
            {
                total += ScoreCard(line);
            }

            total.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day04_Test.txt", 30)]
        [InlineData("Data/Day04.txt", 5539496)]
        public void Part2(string filename, long expectedAnswer)
        {
            var lines = FileIterator.Lines(filename).ToArray();
            var totals = new long[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                var wins = CountWinningNumbers(lines[i]);
                totals[i] += 1;
                for (int j = i+1; j <= i + wins; j++)
                {
                    totals[j] += totals[i];
                }
            }

            totals.Sum().Should().Be(expectedAnswer);
        }
    }
}
