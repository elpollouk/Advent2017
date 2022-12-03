using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2022
{
    public class Day03
    {
        int GetPriority(char c)
        {
            if ('a' <= c && c <= 'z')
            {
                return c - 'a' + 1;
            }
            if ('A' <= c && c <= 'Z')
            {
                return c - 'A' + 27;
            }
            throw new Exception();
        }

        char GetSharedItem(string text)
        {
            HashSet<char> compartment1 = new();
            int limit = text.Length / 2;
            for (int i = 0; i < limit; i++)
            {
                compartment1.Add(text[i]);
            }

            for (int i = limit; i < text.Length; i++)
            {
                if (compartment1.Contains(text[i]))
                    return text[i];
            }

            throw new Exception();
        }

        char GetCommonItem(IList<string> group)
        {
            List<HashSet<char>> sets = new();

            foreach (string s in group)
            {
                HashSet<char> set = new();
                foreach (char c in s)
                {
                    set.Add(c);
                }
                sets.Add(set);
            }

            var common = sets[0].Intersect(sets[1]).Intersect(sets[2]);
            return common.First();
        }

        [Theory]
        [InlineData("Data/Day03_Test.txt", 157)]
        [InlineData("Data/Day03.txt", 8153)]
        public void Part1(string filename, long expectedAnswer)
        {
            long sum = 0;

            foreach (var line in FileIterator.Lines(filename))
            {
                var item = GetSharedItem(line);
                sum += GetPriority(item);
            }

            sum.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day03_Test.txt", 70)]
        [InlineData("Data/Day03.txt", 2342)]
        public void Part2(string filename, long expectedAnswer)
        {
            long sum = 0;
            List<string> group = new();

            foreach (var line in FileIterator.Lines(filename))
            {
                group.Add(line);
                if (group.Count == 3)
                {
                    var item = GetCommonItem(group);
                    sum += GetPriority(item);
                    group.Clear();
                }
            }

            sum.Should().Be(expectedAnswer);
        }
    }
}
