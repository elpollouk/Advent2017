using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

using Answers = System.Collections.Generic.Dictionary<char, long>;

namespace Advent2020
{
    public class Day06
    {
        const char GROUP_SIZE = '#';

        static List<Answers> LoadAnswers(string input)
        {
            var answers = new List<Answers>();
            var currentGroup = new Answers();
            answers.Add(currentGroup);

            FileIterator.ForEachLine(input, (string line) =>
            {
                if (line == "")
                {
                    currentGroup = new Answers();
                    answers.Add(currentGroup);
                    return;
                }

                currentGroup.Increment(GROUP_SIZE);
                foreach (var c in line)
                    currentGroup.Increment(c);
            });

            return answers;
        }

        [Theory]
        [InlineData("Data/Day06_test.txt", 11)]
        [InlineData("Data/Day06.txt", 7027)]
        public void Problem1(string input, int expected)
        {
            LoadAnswers(input)
                .Select(a => a.Keys.Count - 1) // Exclude the GROUP_SIZE key from the count
                .Sum()
                .Should().Be(expected);
        }

        [Theory]
        [InlineData("Data/Day06_test.txt", 6)]
        [InlineData("Data/Day06.txt", 3579)]
        public void Problem2(string input, int expected)
        {
            LoadAnswers(input)
                // Filter down to keys that have the same count as the group size excluding the GROUP_SIZE key iteslf
                .Select(a => a.Keys
                    .Where(k => a[k] == a[GROUP_SIZE])
                    .Count() - 1
                )
                .Sum()
                .Should().Be(expected);
        }
    }
}
