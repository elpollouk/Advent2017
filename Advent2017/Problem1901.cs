using Adevent2017.Utils;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Adevent2017
{
    using Map = List<char[]>;

    static class Problem19Extensions
    {
        public static char GetCell(this Map map, int x, int y) => map[y][x];
    }

    public class Problem1901
    {
        bool IsLetter(char c) => 'A' <= c && c <= 'Z';

        void ChangeDirection(Map map, int x, int y, ref int dX, ref int dY)
        {
            if (dX == 0)
            {
                var left = map.GetCell(x - 1, y);
                var right = map.GetCell(x + 1, y);
                if (left == '-' || IsLetter(left))
                {
                    dX = -1;
                    dY = 0;
                }
                else if (right == '-' || IsLetter(right))
                {
                    dX = 1;
                    dY = 0;
                }
                else
                {
                    Oh.WhatTheFuck();
                }
            }
            else
            {
                var up = map.GetCell(x, y - 1);
                var down = map.GetCell(x, y + 1);
                if (up == '|' || IsLetter(up))
                {
                    dX = 0;
                    dY = -1;
                }
                else if (down == '|' || IsLetter(down))
                {
                    dX = 0;
                    dY = 1;
                }
                else
                {
                    Oh.Bugger();
                }
            }
        }
        string FollowPath(Map map, int entryPoint)
        {
            var result = "";

            var x = entryPoint;
            var y = 0;
            var dX = 0;
            var dY = 1;

            while (true)
            {
                var c = map.GetCell(x, y);
                switch (c)
                {
                    case '|':
                    case '-':
                        break;

                    case '+':
                        ChangeDirection(map, x, y, ref dX, ref dY);
                        break;

                    case ' ':
                        return result;

                    default:
                        if ('A' <= c && c <= 'Z')
                        {
                            result += c;
                        }
                        else
                        {
                            Oh.Bollocks();
                        }
                        break;
                }

                x += dX;
                y += dY;
            }
        }

        string Solve1(string datafile)
        {
            var map = new Map();
            FileIterator.ForEachLine<string>(datafile, line =>
            {
                map.Add(line.ToCharArray());
            });

            var x = -1;

            // Find entry
            for (var i = 0; i < map[0].Length; i++)
            {
                if (map.GetCell(i, 0) == '|')
                {
                    x = i;
                    break;
                }
            }

            return FollowPath(map, x);
        }

        [Theory]
        [InlineData("Data/1901-example.txt", "ABCDEF")]
        [InlineData("Data/1901.txt", "SXWAIBUZY")]
        void Part1(string datafile, string anwser)
        {
            Solve1(datafile).Should().Be(anwser);
        }
    }
}
