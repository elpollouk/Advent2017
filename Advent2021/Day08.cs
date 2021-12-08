using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2021
{
    public class Day08
    {
        record Entry(string[] Init, string[] Display);

        static string StringSort(string input)
        {
            return string.Join("", input.OrderBy(c => c));
        }

        static bool StringContains(string input, string lookFor)
        {
            HashSet<char> inputSet = new(input);
            inputSet.IntersectWith(lookFor);
            return inputSet.Count == lookFor.Length;
        }

        static int StringXor(string a, string b)
        {
            HashSet<char> setA = new(a);
            setA.SymmetricExceptWith(b);
            return setA.Count;
        }

        static Entry[] LoadEntries(string filename)
        {
            List<Entry> entries = new();

            FileIterator.ForEachLine<string>(filename, line =>
            {
                var parts = line.Split(" | ");
                var init = parts[0].Split(" ").Select(s => StringSort(s)).ToArray();
                var display = parts[1].Split(" ").Select(s => StringSort(s)).ToArray();

                entries.Add(new(init, display));
            });

            return entries.ToArray();
        }

        static void RegisterFound(HashSet<string> numbersToDetect, Dictionary<string, int> output, Dictionary<int, string> reverseOutput, string sequence, int number)
        {
            output[sequence] = number;
            reverseOutput[number] = sequence;
            numbersToDetect.Remove(sequence);
        }

        static void DeduceUnique(HashSet<string> numbersToDetect, Dictionary<string, int> output, Dictionary<int, string> reverseOutput, int length, int detectedNumber)
        {
            var s = numbersToDetect.Where(s => s.Length == length).First();
            RegisterFound(numbersToDetect, output, reverseOutput, s, detectedNumber);
        }

        static void Deduce3(HashSet<string> numbersToDetect, Dictionary<string, int> output, Dictionary<int, string> reverseOutput)
        {
            var seven = reverseOutput[7];
            var s = numbersToDetect.Where(s => s.Length == 5 && StringContains(s, seven)).First();
            RegisterFound(numbersToDetect, output, reverseOutput, s, 3);
        }

        static void DeduceXor(HashSet<string> numbersToDetect, Dictionary<string, int> output, Dictionary<int, string> reverseOutput, int length, int knownNumber, int xorResult, int detectedNumber)
        {
            var kn = reverseOutput[knownNumber];
            var s = numbersToDetect.Where(s => s.Length == length && StringXor(s, kn) == xorResult).First();
            RegisterFound(numbersToDetect, output, reverseOutput, s, detectedNumber);
        }


        static Dictionary<string, int> Detect(string[] initData)
        {
            HashSet<string> numbersToDetect = new(initData);
            Dictionary<string, int> detectedNumbers = new();
            Dictionary<int, string> reverseDetectedNumbers = new();

            // Deduce 1, 4, 3, 7
            DeduceUnique(numbersToDetect, detectedNumbers, reverseDetectedNumbers, 2, 1);
            DeduceUnique(numbersToDetect, detectedNumbers, reverseDetectedNumbers, 3, 7);
            DeduceUnique(numbersToDetect, detectedNumbers, reverseDetectedNumbers, 4, 4);
            DeduceUnique(numbersToDetect, detectedNumbers, reverseDetectedNumbers, 7, 8);

            // Deduce 3
            Deduce3(numbersToDetect, detectedNumbers, reverseDetectedNumbers);

            // Deduce 2, 5
            DeduceXor(numbersToDetect, detectedNumbers, reverseDetectedNumbers, 5, 4, 5, 2);
            DeduceXor(numbersToDetect, detectedNumbers, reverseDetectedNumbers, 5, 4, 3, 5);

            // Deduce 9, 0
            DeduceXor(numbersToDetect, detectedNumbers, reverseDetectedNumbers, 6, 3, 1, 9);
            DeduceXor(numbersToDetect, detectedNumbers, reverseDetectedNumbers, 6, 1, 4, 0);

            // Last one is 6
            detectedNumbers[numbersToDetect.First()] = 6;

            return detectedNumbers;
        }

        static int DecodeNumber(Dictionary<string, int> numberKey, string[] digits)
        {
            int value = 0;
            foreach (var digit in digits)
            {
                value *= 10;
                value += numberKey[digit];
            }
            return value;
        }

        [Theory]
        [InlineData("Data/Day08_Test.txt", 26)]
        [InlineData("Data/Day08.txt", 512)]
        public void Part1(string filename, int expectedAnswer)
        {
            var entries = LoadEntries(filename);

            HashSet<int> uniques = new() { 2, 3, 4, 7 };
            var count = entries
                .SelectMany(e => e.Display)
                .Where(s => uniques.Contains(s.Length))
                .Count();
            count.Should().Be(expectedAnswer);
        }


        [Theory]
        [InlineData("Data/Day08_Test.txt", 61229)]
        [InlineData("Data/Day08.txt", 1091165)]
        public void Part2(string filename, long expectedAnswer)
        {
            var entries = LoadEntries(filename);

            long total = entries
                .Select(e => DecodeNumber(Detect(e.Init), e.Display))
                .Sum();

            total.Should().Be(expectedAnswer);
        }
    }
}
