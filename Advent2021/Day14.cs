using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2021
{
    public class Day14
    {
        static (string template, Dictionary<(char, char), char> rules) LoadData(string filename)
        {
            Dictionary<(char, char), char> rules = new();

            var reader = FileIterator.CreateLineReader(filename);
            var template = reader();
            reader();

            foreach (var mapping in FileIterator.Lines(reader))
            {
                var groups = mapping.Groups("(.)(.) -> (.)");
                rules.Add((groups[0][0], groups[1][0]), groups[2][0]);
            }

            return (template, rules);
        }

        static void Add(long[] acc, long[] d)
        {
            for (int i = 0; i < acc.Length; i++)
                acc[i] += d[i];
        }

        static readonly Dictionary<(char, char, int), long[]> cache = new();

        static void Expand(Dictionary<(char, char), char> rules, long[] counts, char c1, char c2, int iteration)
        {
            if (iteration == 0) return;
            if (cache.TryGetValue((c1, c2, iteration), out long[] delta))
            {
                Add(counts, delta);
                return;
            }

            var cM = rules[(c1, c2)];

            delta = new long[26];
            delta[cM - 'A']++;
            Expand(rules, delta, c1, cM, iteration - 1);
            Expand(rules, delta, cM, c2, iteration - 1);
            Add(counts, delta);

            cache[(c1, c2, iteration)] = delta;
        }

        [Theory]
        [InlineData("Data/Day14_Test.txt", 2, 5)]
        [InlineData("Data/Day14_Test.txt", 10, 1588)]
        [InlineData("Data/Day14_Test.txt", 40, 2188189693529)]
        [InlineData("Data/Day14.txt", 10, 3095)]
        [InlineData("Data/Day14.txt", 40, 3152788426516)]
        public void Part1(string filename, int iterations, long expectedAnswer)
        {
            cache.Clear();
            (var p, var rules) = LoadData(filename);
            var counts = new long[26];
            counts[p[0] - 'A'] = 1;
            for (int i = 1; i < p.Length; i++)
            {
                counts[p[i] - 'A']++;
                Expand(rules, counts, p[i - 1], p[i], iterations);
            }

            (var min, var max) = counts.Where(v => v != 0).MinAndMax();
            (max - min).Should().Be(expectedAnswer);
        }
        
    }
}
