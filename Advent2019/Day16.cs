using FluentAssertions;
using System;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2019
{
    public class Day16
    {
        static int[] IntsFromString(string input, int repeats)
        {
            var values = new int[input.Length * repeats];

            for (var i = 0; i < values.Length; i++)
                values[i] = input[i % input.Length] - '0';

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
            var total = 0L;

            var sequenceLength = digit + 1;
            var step = 4 * sequenceLength;

            for (var i = digit; i < input.Length; i += step)
            {
                var limit = Math.Min(i + sequenceLength, input.Length);
                for (var j = i; j < limit; j++)
                {
                    total += input[j];
                }
            }

            for (var i = digit + (sequenceLength * 2); i < input.Length; i += step)
            {
                var limit = Math.Min(i + sequenceLength, input.Length);
                for (var j = i; j < limit; j++)
                {
                    total -= input[j];
                }
            }

            return (int)(Math.Abs(total) % 10);
        }

        static string ProcessPart1(string input, int numPhases)
        {
            return StringFromInts(ProcessInput(input, numPhases));
        }

        static int[] ProcessInput(string input, int numPhases)
        {
            var numbers = IntsFromString(input, 1);

            for (var p = 0; p < numPhases; p++)
            {
                var output = new int[numbers.Length];
                for (var i = 0; i < numbers.Length; i++)
                {
                    output[i] = CalculateDigit(numbers, i);
                }
                numbers = output;
            }

            return numbers;
        }

        static void SumDownTo(int[] numbers, int index)
        {
            var runningTotal = numbers[^1];
            for (var i = numbers.Length - 2; i >= index; i--)
            {
                runningTotal += numbers[i];
                numbers[i] = runningTotal % 10;
            }
        }

        [Theory]
        [InlineData("12345678", 4, "01029498")]
        [InlineData("80871224585914546619083218645595", 100, "24176176")]
        [InlineData("19617804207202209144916044189917", 100, "73745418")]
        [InlineData("69317163492948606335995924319873", 100, "52432133")]
        public void Examples(string input, int numPhases, string expectedResult)
        {
            ProcessPart1(input, numPhases).Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("Data/Day16.txt", "94960436")]
        public void Part1(string filename, string expectedAnswer)
        {
            var input = FileIterator.Lines(filename).First();
            ProcessPart1(input, 100).Should().Be(expectedAnswer);
        }

        [Fact]
        public void SumDownToTest()
        {
            const int numPhases = 100;
            var input = FileIterator.Lines("Data/Day16.txt").First();
            var expectedNumbers = ProcessInput(input, numPhases);

            var actualNumbers = IntsFromString(input, 1);

            for (var i = 0; i < numPhases; i++)
                SumDownTo(actualNumbers, actualNumbers.Length / 2);

            for (var i = actualNumbers.Length / 2; i < actualNumbers.Length; i++)
                actualNumbers[i].Should().Be(expectedNumbers[i]);
        }

        [Theory]
        [InlineData("Data/Day16.txt", "57762756")]
        public void Part2(string filename, string expectedAnswer)
        {
            var input = FileIterator.Lines(filename).First();
            var numbers = IntsFromString(input, 10000);
            var offset = int.Parse(input.Substring(0, 7));

            for (var i = 0; i < 100; i++)
                SumDownTo(numbers, offset);

            var answer = "";
            for (var i = offset; i < offset + 8; i++)
                answer += $"{numbers[i]}";

            answer.Should().Be(expectedAnswer);
        }
    }
}
