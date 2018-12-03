using Utils;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Advent2017
{
    public class Problem1001
    {
        static int[] BuildString(int length)
        {
            var s = new int[length];
            for (var i = 0; i < length; i++)
                s[i] = i;

            return s;
        }

        static void Reverse(int[] s, int start, int length)
        {
            var end = (start + length) - 1;

            while (start < end)
            {
                var t = s.GetAtMod(end);
                s.SetAtMod(end--, s.GetAtMod(start));
                s.SetAtMod(start++, t);
            }
        }

        static int[] Solve(int stringlength, int numRounds, List<int> input)
        {
            var s = BuildString(stringlength);
            var currentPosition = 0;
            var skipSize = 0;

            for (var i = 0; i < numRounds; i++)
            {
                foreach (var length in input)
                {
                    Reverse(s, currentPosition, length);
                    currentPosition += length + skipSize;
                    skipSize++;
                }
            }

            return s;
        }

        [Theory]
        [InlineData("3,4,1,5", 5, 12)]
        [InlineData("212,254,178,237,2,0,1,54,167,92,117,125,255,61,159,164", 256, 212)]
        public void Part1(string input, int stringLength, int answer)
        {
            var values = input.SplitAndConvert<int>();
            var sparse = Solve(stringLength, 1, new List<int>(values));
            (sparse[0] * sparse[1]).Should().Be(answer);
        }

        static List<int> StringToAscii(string input)
        {
            var ascii = new List<int>();
            for (var i = 0; i < input.Length; i++)
                ascii.Add(input[i]);

            return ascii;
        }

        static string Reduce(int[] input)
        {
            if (input.Length != 256) Oh.ForFucksSake();
            var reduced = new byte[16];

            for (var i = 0; i < 16; i++)
                for (var j = 0; j < 16; j++)
                    reduced[i] ^= (byte)input[(i * 16) + j];

            return BitConverter.ToString(reduced).Replace("-", "").ToLower();
        }

        [Theory]
        [InlineData("", "a2582a3a0e66e6e86e3812dcb672a272")]
        [InlineData("AoC 2017", "33efeb34ea91902bb2f59c9920caa6cd")]
        [InlineData("1,2,3", "3efbe78a8d82f29979031a4aa0b16a9d")]
        [InlineData("1,2,4", "63960835bcdc130f0b66d7ff4f6a5a8e")]
        [InlineData("212,254,178,237,2,0,1,54,167,92,117,125,255,61,159,164", "96de9657665675b51cd03f0b3528ba26")]
        public void Part2(string input, string answer) => Hash(input).Should().Be(answer);

        public static string Hash(string input)
        {
            var values = StringToAscii(input);
            values.Add(17);
            values.Add(31);
            values.Add(73);
            values.Add(47);
            values.Add(23);

            var sparse = Solve(256, 64, values);
            return Reduce(sparse);
        }
    }
}
