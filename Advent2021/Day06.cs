using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2021
{
    public class Day06
    {
        readonly Dictionary<(int, int), long> cache = new();
        long CalcFish(int spawnTimer, int days)
        {
            if (cache.TryGetValue((spawnTimer, days), out var count)) return count;
            var timer = days;

            count = 1;
            if (spawnTimer < timer)
            {
                timer -= (spawnTimer + 1);
                count += CalcFish(8, timer);

                while (7 <= timer)
                {
                    timer -= 7;
                    count += CalcFish(8, timer);
                }
            }

            cache[(spawnTimer, days)] = count;
            return count;
        }

        [Theory]
        [InlineData(3, 1, 1)]
        [InlineData(3, 3, 1)]
        [InlineData(3, 4, 2)]
        [InlineData(3, 5, 2)]
        [InlineData(3, 6, 2)]
        [InlineData(3, 10, 2)]
        [InlineData(3, 11, 3)]
        [InlineData(3, 12, 3)]
        [InlineData(3, 13, 4)]
        [InlineData(3, 17, 4)]
        [InlineData(3, 18, 5)]
        [InlineData(3, 19, 5)]
        [InlineData(3, 20, 7)]
        [InlineData(1, 1, 1)]
        [InlineData(1, 2, 2)]
        [InlineData(1, 8, 2)]
        [InlineData(1, 9, 3)]
        [InlineData(1, 10, 3)]
        [InlineData(1, 11, 4)]
        public void CalcFishTest(int spawnTimer, int days, long expectedAnswer) => CalcFish(spawnTimer, days).Should().Be(expectedAnswer);

        [Theory]
        [InlineData("Data/Day06_Test.txt", 18, 26)]
        [InlineData("Data/Day06_Test.txt", 80, 5934)]
        [InlineData("Data/Day06_Test.txt", 256, 26984457539)]
        [InlineData("Data/Day06.txt", 80, 374994)]
        [InlineData("Data/Day06.txt", 256, 1686252324092)]
        public void RecursiveSolution(string filename, int days, long expectedAnswer)
        {
            var numbers = FileIterator.LoadCSV<int>(filename);
            long total = 0;
            foreach (var number in numbers)
            {
                total += CalcFish(number, days);
            }

            total.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day06_Test.txt", 18, 26)]
        [InlineData("Data/Day06_Test.txt", 80, 5934)]
        [InlineData("Data/Day06_Test.txt", 256, 26984457539)]
        [InlineData("Data/Day06.txt", 80, 374994)]
        [InlineData("Data/Day06.txt", 256, 1686252324092)]
        public void MatrixSolution(string filename, int days, long expectedAnswer)
        {
            var fish = new long[9];
            var numbers = FileIterator.LoadCSV<int>(filename);
            foreach (var number in numbers)
                fish[number]++;

            while (days-- > 0)
            {
                var newFish = fish[0];
                for (int i = 1; i < fish.Length; i++)
                    fish[i - 1] = fish[i];
                fish[6] += newFish;
                fish[8] = newFish;
            }

            fish.Sum().Should().Be(expectedAnswer);
        }
    }
}
