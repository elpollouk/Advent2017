using FluentAssertions;
using System;
using Utils;
using Xunit;

namespace Advent2022
{
    public class Day25
    {
        int ToDecimal(char c) => c switch
        {
            '=' => -2,
            '-' => -1,
            '0' => 0,
            '1' => 1,
            '2' => 2,
            _ => throw new Exception()
        };

        long ToDecimal(string input)
        {
            long value = 0;

            foreach (var c in input)
            {
                value *= 5;
                value += ToDecimal(c);
            }

            return value;
        }

        string ToSnafuChar(long v) => v switch
        {
            -2 => "=",
            -1 => "-",
            0 => "0",
            1 => "1",
            2 => "2",
            _ => throw new Exception()
        };

        string ToSnafu(long value)
        {
            if (value == 0) return "0";

            string r = "";

            while (value != 0)
            {
                long current = value % 5;
                value /= 5;

                if (current > 2)
                {
                    value += 1;
                    current -= 5;
                }

                r = ToSnafuChar(current) + r;
            }

            return r;
        }

        [Theory]
        [InlineData("0", 0)]
        [InlineData("1", 1)]
        [InlineData("2", 2)]
        [InlineData("1=", 3)]
        [InlineData("1-", 4)]
        [InlineData("10", 5)]
        [InlineData("11", 6)]
        [InlineData("12", 7)]
        [InlineData("2=", 8)]
        [InlineData("2-", 9)]
        [InlineData("20", 10)]
        [InlineData("1=0", 15)]
        [InlineData("1-0", 20)]
        [InlineData("1=11-2", 2022)]
        [InlineData("1-0---0", 12345)]
        [InlineData("1121-1110-1=0", 314159265)]
        public void ConversionTest(string snafu, long dec)
        {
            ToDecimal(snafu).Should().Be(dec);
            ToSnafu(dec).Should().Be(snafu);
        }

        [Theory]
        [InlineData("Data/Day25_Test.txt", "2=-1=0")]
        [InlineData("Data/Day25.txt", "2==0=0===02--210---1")]
        public void Part1(string filename, string expectedAnswer)
        {
            long total = 0;
            foreach (var line in FileIterator.Lines(filename))
            {
                total += ToDecimal(line);
            }

            ToSnafu(total).Should().Be(expectedAnswer);
        }
    }
}
