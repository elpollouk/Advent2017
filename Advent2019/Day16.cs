using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2019
{
    public class Day16
    {
        static readonly int[] BASE_SEQUENCE = new int[] { 0, 1, 0, -1 };

        static Func<long> CreateSequence(long iteration)
        {
            iteration++;
            int baseIndex = 0;
            int iterationCount = 0;

            return () =>
            {
                var result = BASE_SEQUENCE[baseIndex % BASE_SEQUENCE.Length];
                if (++iterationCount == iteration)
                {
                    iterationCount = 0;
                    baseIndex++;
                }
                return result;
            };
        }

        static int[] IntsFromString(string input)
        {
            var values = new int[input.Length];

            for (var i = 0; i < input.Length; i++)
                values[i] = input[i] - '0';

            return values;
        }

        static string StringFromInts(int[] input)
        {
            var s = "";

            for (var i = 0; i < 8; i++)
                s += $"{input[i]}";

            return s;
        } 

        static int CalculateDigit(int[] input, long digit)
        {
            var sequence = CreateSequence(digit);
            sequence();

            var total = 0L;
            foreach (var v in input)
            {
                total += v * sequence();
            }

            return (int)(Math.Abs(total) % 10);
        }

        static string ProcessInput(string input, int numPhases)
        {
            var numbers = IntsFromString(input);

            for (var p = 0; p < numPhases; p++)
            {
                var output = new int[numbers.Length];
                for (var i = 0; i < numbers.Length; i++)
                {
                    output[i] = CalculateDigit(numbers, i);
                }
                numbers = output;
            }

            return StringFromInts(numbers);
        }

        [Theory]
        [InlineData("12345678", 4, "01029498")]
        [InlineData("80871224585914546619083218645595", 100, "24176176")]
        [InlineData("19617804207202209144916044189917", 100, "73745418")]
        [InlineData("69317163492948606335995924319873", 100, "52432133")]
        public void Examples(string input, int numPhases, string expectedResult)
        {
            ProcessInput(input, numPhases).Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("Data/Day16.txt", "94960436")]
        public void Part1(string filename, string expectedAnswer)
        {
            var input = FileIterator.Lines(filename).First();
            ProcessInput(input, 100).Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day16_Test.txt", 0)]
        [InlineData("Data/Day16.txt", 0)]
        public void Part2(string filename, long expectedAnswer)
        {

        }
    }
}
