using FluentAssertions;
using System;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2020
{
    public class Day02
    {
        class PasswordEntry
        {
            public readonly char RequiredLetter;
            public readonly int Min;
            public readonly int Max;
            public readonly string Password;

            public PasswordEntry(string raw)
            {
                var groups = raw.Groups("^(\\d+)-(\\d+) (.): (.+)$");
                RequiredLetter = groups[2][0];
                Min = int.Parse(groups[0]);
                Max = int.Parse(groups[1]);
                Password = groups[3];
            }

            public static bool IsValidPart1(PasswordEntry entry)
            {
                var count = 0;
                foreach (var letter in entry.Password)
                    if (entry.RequiredLetter == letter)
                        count++;

                return entry.Min <= count && count <= entry.Max;
            }

            public static bool IsValidPart2(PasswordEntry entry)
            {
                var letter1 = entry.Password[entry.Min - 1];
                var letter2 = entry.Password[entry.Max - 1];

                if (letter1 == entry.RequiredLetter && letter2 != entry.RequiredLetter) return true;
                if (letter1 != entry.RequiredLetter && letter2 == entry.RequiredLetter) return true;
                return false;
            }
        }

        int CountValid(string input, Func<PasswordEntry, bool> validator)
        {
            return FileIterator.Lines(input)
                .Select((e) => new PasswordEntry(e))
                .Where(validator)
                .Count();
        }

        [Theory]
        [InlineData("Data/Day02_test.txt", 2)]
        [InlineData("Data/Day02.txt", 655)]
        public void Problem1(string input, int expected)
        {
            CountValid(input, PasswordEntry.IsValidPart1).Should().Be(expected);
        }

        [Theory]
        [InlineData("Data/Day02_test.txt", 1)]
        [InlineData("Data/Day02.txt", 673)]
        public void Problem2(string input, int expected)
        {
            CountValid(input, PasswordEntry.IsValidPart2).Should().Be(expected);
        }
    }
}
