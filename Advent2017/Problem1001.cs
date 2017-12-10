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

        int Solve(int stringlength, List<int> input)
        {
            var s = BuildString(stringlength);
            var currentPosition = 0;
            var skipSize = 0;

            while (input.Count != 0)
            {
                var length = input.Shift();
                Reverse(s, currentPosition, length);
                currentPosition += length + skipSize;
                skipSize++;
            }

            return s[0] * s[1];
        }

        [Theory]
        [InlineData("Data/1001-example.txt", 5, 12)]
        [InlineData("Data/1001.txt", 256, 212)]
        public void Part1(string input, int stringLength, int answer)
        {
            var values = FileIterator.LoadCSV<int>(input);
            Solve(stringLength, new List<int>(values)).Should().Be(answer);
        }

        public void Part2(string input, int answer)
        {

        }
    }
}
