using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2016
{
    public class Day02
    {
        Dictionary<(int, int), char> keypad1()
        {
            return new Dictionary<(int, int), char>
            {
                [(0, 0)] = '1',
                [(1, 0)] = '2',
                [(2, 0)] = '3',
                [(0, 1)] = '4',
                [(1, 1)] = '5',
                [(2, 1)] = '6',
                [(0, 2)] = '7',
                [(1, 2)] = '8',
                [(2, 2)] = '9'
            };
        }

        Dictionary<(int, int), char> keypad2()
        {
            return new Dictionary<(int, int), char>
            {
                [(2, 0)] = '1',
                [(1, 1)] = '2',
                [(2, 1)] = '3',
                [(3, 1)] = '4',
                [(0, 2)] = '5',
                [(1, 2)] = '6',
                [(2, 2)] = '7',
                [(3, 2)] = '8',
                [(4, 2)] = '9',
                [(1, 3)] = 'A',
                [(2, 3)] = 'B',
                [(3, 3)] = 'C',
                [(2, 4)] = 'D'
            };
        }

        (int, int) Move(char direction, (int x, int y) pos, Dictionary<(int,int),char> keypad)
        {
            (int, int) newPos;
            switch (direction)
            {
                case 'U':
                {
                    newPos = (pos.x, pos.y - 1);
                    break;
                }
                case 'D':
                {
                    newPos = (pos.x, pos.y + 1);
                    break;
                }
                case 'L':
                {
                    newPos = (pos.x - 1, pos.y);
                    break;
                }
                case 'R':
                {
                    newPos = (pos.x + 1, pos.y);
                    break;
                }
                default: throw new Exception();
            }

            return keypad.ContainsKey(newPos) ? newPos : pos;
        }

        string GetCode(IEnumerable<string> lines, Dictionary<(int,int),char> keypad, (int,int) pos)
        {
            var number = "";

            foreach (var line in lines)
            {
                foreach (char direction in line)
                {
                    pos = Move(direction, pos, keypad);
                }
                number += keypad[pos];
            }

            return number;
        }

        [Theory]
        [InlineData("Data/Day02_Test.txt", "1985")]
        [InlineData("Data/Day02.txt", "65556")]
        public void Part1(string filename, string expectedAnswer)
        {
            var lines = FileIterator.Lines(filename);
            GetCode(lines, keypad1(), (1, 1)).Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day02_Test.txt", "5DB3")]
        [InlineData("Data/Day02.txt", "CB779")]
        public void Part2(string filename, string expectedAnswer)
        {
            var lines = FileIterator.Lines(filename);
            GetCode(lines, keypad2(), (0, 2)).Should().Be(expectedAnswer);
        }
    }
}
