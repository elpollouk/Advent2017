using FluentAssertions;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Utils;
using Xunit;

namespace Advent2016
{
    public class Day05
    {
        char ToChar(int value)
        {
            switch (value)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    return (char)(value + '0');

                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    return (char)(value - 10 + 'a');

                default:
                    throw new Exception();
            }
        }

        char sixthCharAfterMD5(string initial, long number)
        {
            var input = initial + number;
            var result = MD5.HashData(Encoding.UTF8.GetBytes(input));
            if ((result[0] | result[1] | result[2] >> 4) != 0) return (char)0;
            var c = ToChar(result[2] & 0xF);
            return c;
        }

        (int i, char c) sixthAndSeventhCharAfterMD5(string initial, long number)
        {
            var input = initial + number;
            var result = MD5.HashData(Encoding.UTF8.GetBytes(input));
            if ((result[0] | result[1] | result[2] >> 4) != 0) return (int.MaxValue, (char)0);
            var i = result[2] & 0xF;
            var c = ToChar(result[3] >> 4);
            return (i, c);
        }

        [Theory]
        [InlineData("abc", 3231929, '1')]
        [InlineData("abc", 5017308, '8')]
        [InlineData("abc", 5278568, 'f')]
        public void Md5HashExample(string initial, long number, char expectedAnswer)
        {
            sixthCharAfterMD5(initial, number).Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("abc", "18f47a30")]
        //[InlineData("abbhdwsy", "801b56a7")]
        public void Part1(string initial, string expectedAnswer)
        {
            long i = 0;
            string password = "";

            while (password.Length != 8)
            {
                var result = sixthCharAfterMD5(initial, i++);
                if (result != 0) password += result;
            }

            password.Should().Be(expectedAnswer);
        }

        bool IsAllSet(char[] password)
        {
            foreach (var c in password)
            {
                if (c == 0) return false;
            }
            return true;
        }

        //[Theory]
        //[InlineData("abc", "05ace8e3")]
        //[InlineData("abbhdwsy", "424a0197")]
        public void Part2(string initial, string expectedAnswer)
        {
            long i = 0;
            char[] password = new char[8];

            while (!IsAllSet(password))
            {
                var result = sixthAndSeventhCharAfterMD5(initial, i++);
                if (result.i < password.Length && password[result.i] == 0)
                {
                    password[result.i] = result.c;
                }
            }

            password.Join("").Should().Be(expectedAnswer);
        }
    }
}
