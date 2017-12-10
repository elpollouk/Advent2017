using Adevent2017.Utils;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Adevent2017
{
    public class Problem1001
    {
        int[] BuildString(int length)
        {
            var s = new int[length];
            for (var i = 0; i < length; i++)
                s[i] = i;

            return s;
        }

        void Reverse(int[] s, int start, int length)
        {
            var end = (start + length) - 1;

            while (start < end)
            {
                var t = s.GetAtMod(end);
                s.SetAtMod(end, s.GetAtMod(start));
                s.SetAtMod(start, t);
                start++;
                end--;
            }
        }

        int[] Solve(int stringlength, List<int> input, int numRounds)
        {
            var s = BuildString(stringlength);
            var currentPosition = 0;
            var skipSize = 0;

            for (var i = 0; i < numRounds; i++)
            {
                var roundInput = input.Clone();

                while (roundInput.Count != 0)
                {
                    var length = roundInput.Shift();
                    Reverse(s, currentPosition, length);
                    currentPosition += length + skipSize;
                    skipSize++;
                }
            }

            return s;
        }

        [Theory]
        [InlineData("Data/1001-example.txt", 5, 12)]
        [InlineData("Data/1001.txt", 256, 212)]
        public void Part1(string input, int stringLength, int answer)
        {
            var values = FileIterator.LoadCSV<int>(input);
            var sparse = Solve(stringLength, new List<int>(values), 1);
            (sparse[0] * sparse[1]).Should().Be(answer);
        }

        List<int> StringToAscii(string input)
        {
            var ascii = new List<int>();
            for (var i = 0; i < input.Length; i++)
                ascii.Add(input[i]);

            return ascii;
        }

        string Reduce(int[] input)
        {
            if (input.Length != 256) Oh.ForFucksSake();
            var reduced = new byte[16];

            for (var i = 0; i < 16; i++)
            {
                reduced[i] = (byte)input[i * 16];
                for (var j = 1; j < 16; j++)
                {
                    reduced[i] ^= (byte)input[(i * 16) + j];
                }
            }

            return BitConverter.ToString(reduced).Replace("-", "").ToLower();
        }

        [Theory]
        [InlineData("", "a2582a3a0e66e6e86e3812dcb672a272")]
        [InlineData("AoC 2017", "33efeb34ea91902bb2f59c9920caa6cd")]
        [InlineData("1,2,3", "3efbe78a8d82f29979031a4aa0b16a9d")]
        [InlineData("1,2,4", "63960835bcdc130f0b66d7ff4f6a5a8e")]
        [InlineData("212,254,178,237,2,0,1,54,167,92,117,125,255,61,159,164", "96de9657665675b51cd03f0b3528ba26")]
        public void Part2(string input, string answer)
        {
            var values = StringToAscii(input);
            values.Add(17);
            values.Add(31);
            values.Add(73);
            values.Add(47);
            values.Add(23);

            var sparse = Solve(256, values, 64);
            Reduce(sparse).Should().Be(answer);
        }
    }
}
