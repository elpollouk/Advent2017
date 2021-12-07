using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2021
{
    public class Day03
    {
        static (int[] counts, int threshold) ParseCountsThreshold(string inputFilename)
        {
            int[] counts = null;
            var threshold = 0;

            FileIterator.ForEachLine<string>(inputFilename, line => {
                if (counts == null)
                {
                    counts = new int[line.Length];
                }

                for (var i = 0; i < line.Length; i++)
                {
                    if (line[i] == '1')
                    {
                        counts[i]++;
                    }
                }
                threshold++;
            });

            threshold /= 2;

            return (counts, threshold);
        }

        static (int, int) ParseGammaEpsilon(string inputFilename)
        {
            (var counts, var threshold) = ParseCountsThreshold(inputFilename);

            int gamma = 0;
            int epsilon = 0;
            foreach (var count in counts)
            {
                gamma <<= 1;
                epsilon <<= 1;

                if (count < threshold)
                {
                    epsilon |= 1;
                }
                else
                {
                    gamma |= 1;
                }
            }
            return (gamma, epsilon);
        }

        static bool GetCriteria(IEnumerable<string> numbers, int position)
        {
            int count = numbers.Select(v => v[position]).Where(v => v == '1').Count();
            return count >= (numbers.Count() - count);
        }

        static IEnumerable<string> Filter(IEnumerable<string> numbers, bool criteria, int position)
        {
            var actualCriteria = GetCriteria(numbers, position);
            return numbers.Where(v => v[position] == '1' == (actualCriteria == criteria));
        }

        static int Filter(IEnumerable<string> numbers, bool criteria)
        {
            int position = 0;
            while (numbers.Count() != 1)
            {
                numbers = Filter(numbers, criteria, position);
                position++;
            }

            var valueString = numbers.First();
            int result = 0;
            foreach (var v in valueString)
            {
                result <<= 1;
                result |= (v == '1') ? 1 : 0;
            }

            return result;
        }

        static (int, int) ParseOxygenCO2(string inputFilename)
        {
            var oxygenSet = FileIterator.LoadLines<string>(inputFilename);
            var co2Set = FileIterator.LoadLines<string>(inputFilename);

            int oxygen = Filter(oxygenSet, true);
            int co2 = Filter(co2Set, false);

            return (oxygen, co2);
        }

        [Theory]
        [InlineData("Data/Day03_Test.txt", 198)]
        [InlineData("Data/Day03.txt", 3901196)]
        public void Part1(string inputFile, int expectedAnswer)
        {
            (var gamma, var epsilon) = ParseGammaEpsilon(inputFile);
            (gamma * epsilon).Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day03_Test.txt", 230)]
        [InlineData("Data/Day03.txt", 4412188)]
        public void Part2(string inputFile, int expectedAnswer)
        {
            (var oxygen, var co2) = ParseOxygenCO2(inputFile);
            (oxygen * co2).Should().Be(expectedAnswer);
        }
    }
}
