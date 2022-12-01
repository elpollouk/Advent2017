using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2022
{
    public class Day01
    {
        [Theory]
        [InlineData("Data/Day01_Test.txt", 24000, 45000)]
        [InlineData("Data/Day01.txt", 71124, 204639)]
        public void Solve(string filename, long topOne, long topThree)
        {
            List<long> values = new();
            long calories = 0;

            foreach (var line in FileIterator.Lines(filename))
            { 
                if (line == "")
                {
                    values.Add(calories);
                    calories = 0;
                    continue;
                }

                calories += long.Parse(line);
            }
            values.Add(calories);

            var ordered = values.OrderByDescending(x => x);
            ordered.First().Should().Be(topOne);
            ordered.Take(3).Sum().Should().Be(topThree);
        }
    }
}
