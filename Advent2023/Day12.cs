using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Utils;
using Xunit;

namespace Advent2023
{
    public class Day12
    {
        class CheckGroup
        {
            readonly int[] counts;

            public CheckGroup(string groups)
            {
                counts = groups.Split(',').Select(int.Parse).ToArray();
            }

            public bool IsValid(int group, int checkSize)
            {
                if (counts.Length <= group) return false;
                return checkSize == counts[group];
            }

            public bool HasCompleted(int group)
            {
                return group == counts.Length;
            }
        }

        class CheckContext
        {
            readonly CheckGroup checks;
            readonly string original;
            readonly char[] currentState;

            public CheckContext(string input, string groups)
            { 
                checks = new CheckGroup(groups);
                original = input;
                currentState = [.. original];
            }

            public long CountValid(int checkIndex, int currentGroupSize, int group)
            {
                long count = 0;
                while (checkIndex < currentState.Length)
                {
                    var c = currentState[checkIndex];
                    switch (c)
                    {
                        case '#':
                            currentGroupSize++;
                            break;

                        case '?':
                            currentState[checkIndex] = '.';
                            count += CountValid(checkIndex, currentGroupSize, group);
                            currentState[checkIndex] = '#';
                            count += CountValid(checkIndex, currentGroupSize, group);
                            currentState[checkIndex] = '?';
                            return count;

                        case '.':
                            if (currentGroupSize != 0)
                            {
                                if (!checks.IsValid(group, currentGroupSize))
                                {
                                    return count;
                                }
                                group++;
                                currentGroupSize = 0;
                            }
                            break;
                    }
                    checkIndex++;
                }

                if (currentGroupSize != 0)
                {
                    if (!checks.IsValid(group, currentGroupSize))
                    {
                        return count;
                    }
                    group++;
                }

                if (checks.HasCompleted(group))
                {
                    count++;
                }
                return count;
            }
        }

        long CountValid(string input, string groups)
        {
            CheckContext context = new(input, groups);
            return context.CountValid(0, 0, 0);
        }

        (string, string) Expand(string pattern, string groups)
        {
            var newPattern = pattern;
            var newGroups = groups;

            for (int i = 0; i < 4; i++)
            {
                newPattern += '?' + pattern;
                newGroups += ',' + groups;
            }

            return(newPattern, newGroups);
        }

        [Theory]
        [InlineData("#.#.###", "1,1,3", 1)]
        [InlineData(".#.###.#.######", "1,3,1,6", 1)]
        [InlineData("####.#...#...", "4,1,1", 1)]
        [InlineData(".###.##....#", "3,2,1", 1)]
        [InlineData(".#.....", "1", 1)]
        [InlineData(".......", "1", 0)]
        [InlineData(".##....", "1", 0)]
        [InlineData(".#...#.", "1", 0)]
        [InlineData(".#....#", "1", 0)]
        [InlineData("#..#...", "1", 0)]
        [InlineData(".###.##....", "3,2,1", 0)]
        [InlineData("?", "1", 1)]
        [InlineData("??", "1", 2)]
        [InlineData("??", "2", 1)]
        [InlineData(".?.?.?.", "1", 3)]
        [InlineData(".???.", "2", 2)]
        [InlineData("?###????????", "3,2,1", 10)]
        public void CountValidTest(string input, string groups, long expectedCount)
        {
            CountValid(input, groups).Should().Be(expectedCount);
        }

        [Theory]
        [InlineData("???.###", "1,1,3", 1)]
        [InlineData("?###????????", "3,2,1", 506250)]
        public void ExpandTest(string input, string groups, long expectedCount)
        {
            (input, groups) = Expand(input, groups);
            CountValid(input, groups).Should().Be(expectedCount);
        }

        [Theory]
        [InlineData("Data/Day12_Test.txt", 21)]
        [InlineData("Data/Day12.txt", 7195)]
        public void Part1(string filename, long expectedAnswer)
        {
            long total = 0;
            foreach (var line in FileIterator.Lines(filename))
            {
                var parts = line.Split(' ');
                total += CountValid(parts[0], parts[1]);
            }
            total.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day12_Test.txt", 525152)]
        //[InlineData("Data/Day12.txt", 0)]
        public void Part2(string filename, long expectedAnswer)
        {
            long total = 0;
            foreach (var line in FileIterator.Lines(filename))
            {
                var parts = line.Split(' ');
                var (input, groups) = Expand(parts[0], parts[1]);
                total += CountValid(input, groups);
            }
            total.Should().Be(expectedAnswer);
        }
    }
}
