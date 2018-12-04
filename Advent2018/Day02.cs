using FluentAssertions;
using System;
using System.Collections.Generic;
using Utils;
using Xunit;

namespace Advent2018
{
    static class IntDictionaryExtensions
    {
        public static int Increment<Key>(this Dictionary<Key, int> dict, Key key)
        {
            var value = 0;
            dict.TryGetValue(key, out value);
            value++;
            dict[key] = value;
            return value;
        }
    }

    public class Day02
    {
        void HasTwoOrThree(string input, out bool hasTwo, out bool hasThree)
        {
            hasTwo = false;
            hasThree = false;
            var counts = new Dictionary<char, int>();

            foreach (var c in input)
                counts.Increment(c);

            foreach (var count in counts.Values)
                if (count == 2)
                    hasTwo = true;
                else if (count == 3)
                    hasThree = true;
        }

        [Theory]
        [InlineData("abcdef", false, false)]
        [InlineData("bababc", true, true)]
        [InlineData("abbcde", true, false)]
        [InlineData("abcccd", false, true)]
        [InlineData("aabcdd", true, false)]
        [InlineData("abcdee", true, false)]
        [InlineData("ababab", false, true)]
        public void Problem1_TestHasTwoOrThree(string input, bool expectedTwo, bool expectedThree)
        {
            HasTwoOrThree(input, out bool hasTwo, out bool hasThree);
            hasTwo.Should().Be(expectedTwo);
            hasThree.Should().Be(expectedThree);
        }

        [Theory]
        [InlineData(12, "abcdef", "bababc", "abbcde", "abcccd", "aabcdd", "abcdee", "ababab")]
        public void Problem1_TestChecksum(int expectedSum, params string[] inputs)
        {
            var twos = 0;
            var threes = 0;
            foreach (var input in inputs)
            {
                HasTwoOrThree(input, out bool hasTwo, out bool hasThree);
                if (hasTwo) twos++;
                if (hasThree) threes++;
            }

            (twos * threes).Should().Be(expectedSum);
        }

        [Fact]
        public void Problem1_Solution()
        {
            var inputs = FileIterator.LoadLines<string>("Data/Day02.txt");
            Problem1_TestChecksum(5704, inputs);
        }

        int FindError(string input1, string input2)
        {
            if (input1.Length != input2.Length)
                throw new ArgumentException();

            var r = -1;
            for (var i = 0; i < input1.Length;i++)
                if (input1[i] != input2[i])
                    if (r == -1)
                        r = i;
                    else
                        return -1;

            return r;
        }

        [Theory]
        [InlineData("fgij", "abcde", "fghij", "klmno", "pqrst", "fguij", "axcye", "wvxyz")]
        public void Problem2_Test(string expectedResult, params string[] inputs)
        {
            for (var i = 0; i < inputs.Length - 1; i++)
            {
                for (var j = i + 1; j < inputs.Length; j++)
                {
                    var errorPos = FindError(inputs[i], inputs[j]);
                    if (errorPos != -1)
                    {
                        var result = inputs[i].Remove(errorPos, 1);
                        result.Should().Be(expectedResult);
                        return;
                    }
                }
            }

            throw new Exception("Failed to find matching error.");
        }

        [Fact]
        public void Problem2_Solution()
        {
            var inputs = FileIterator.LoadLines<string>("Data/Day02.txt");
            Problem2_Test("umdryabviapkozistwcnihjqx", inputs);
        }
    }
}
