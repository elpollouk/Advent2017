using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2023
{
    public class Day03
    {
        bool IsDigit(char c) => '0' <= c && c <= '9';

        bool NextCell(char[,] grid, XY pos)
        {
            if (pos.y >= grid.GetLength(1)) return false;

            pos.x++;
            if (pos.x == grid.GetLength(0))
            {
                pos.x = 0;
                pos.y++;
                if (pos.y == grid.GetLength(1))
                {
                    return false;
                }
            }

            return true;
        }

        bool ScanToNextNumber(char[,] grid, XY start)
        {
            if (start.y >= grid.GetLength(1)) return false;

            while (!IsDigit(grid[start.x, start.y]))
            {
                bool inBounds = NextCell(grid, start);
                if (!inBounds) return false;
            }

            return true;
        }

        string ReadNumberReportAdjecency(char[,] grid, XY start, out bool adjecent)
        {
            adjecent = false;
            string number = "";

            while (IsDigit(grid[start.x, start.y]))
            {
                number += grid[start.x, start.y];

                if (adjecent == false)
                {
                    foreach (var c in grid.GetNeighbours(start.x, start.y))
                    {
                        if (!IsDigit(c) && c != '.')
                        {
                            adjecent = true;
                            break;
                        }
                    }
                }

                bool inBounds = NextCell(grid, start);
                if (!inBounds || start.x == 0) break;
            }

            return number;
        }

        long SumParts(char[,] grid)
        {
            XY pos = new(0, 0);
            long sum = 0;

            while (ScanToNextNumber(grid, pos))
            {
                string number = ReadNumberReportAdjecency(grid, pos, out bool hasAdjancency);
                if (hasAdjancency)
                {
                    sum += int.Parse(number);
                }
            }

            return sum;
        }

        int AddNumberAtPos(char[,] grid, int x, int y, List<int> numbers)
        {
            if (x < 0 || x >= grid.GetLength(0)) return x;
            if (!IsDigit(grid[x, y])) return x;

            while (x >= 0 && IsDigit(grid[x, y]))
            {
                x--;
            }
            x++;

            string result = "";
            while (x < grid.GetLength(0) && IsDigit(grid[x, y]))
            {
                result += grid[x, y];
                x++;
            }
            
            numbers.Add(int.Parse(result));
            return x;
        }

        void AddNumbersOnRow(char[,] grid, int x, int y, List<int> numbers)
        {
            int _x = AddNumberAtPos(grid, x - 1, y, numbers);

            if (_x < x)
            {
                _x = AddNumberAtPos(grid, x, y, numbers);
            }

            if (_x < x + 1)
            {
                AddNumberAtPos(grid, x + 1, y, numbers);
            }
        }

        List<int> GetNumbersAroundPoint(char[,] grid, int x, int y)
        {
            List<int> numbers = new List<int>();

            AddNumbersOnRow(grid, x, y - 1, numbers);
            AddNumbersOnRow(grid, x, y, numbers);
            AddNumbersOnRow(grid, x, y + 1, numbers);

            return numbers;
        }

        [Theory]
        [InlineData("Data/Day03.txt", 8, 8, "995,322")]
        [InlineData("Data/Day03.txt", 55, 1, "501,609")]
        [InlineData("Data/Day03.txt", 136, 16, "189,873")]
        [InlineData("Data/Day03.txt", 78, 18, "445,809")]
        [InlineData("Data/Day03.txt", 123, 19, "641,888")]
        [InlineData("Data/Day03.txt", 135, 137, "525,855")]
        [InlineData("Data/Day03.txt", 45, 132, "274,110")]
        [InlineData("Data/Day03.txt", 60, 113, "148")]
        [InlineData("Data/Day03.txt", 3, 137, null)]
        public void TestGetNumbersAroundPoint(string filename, int x, int y, string expectedNumbers)
        {
            var grid = FileIterator.LoadGrid(filename);
            var parsedNumbers = expectedNumbers != null ? expectedNumbers.Split(',').Select(v => int.Parse(v)).ToArray() : Array.Empty<int>();

            var numbers = GetNumbersAroundPoint(grid, x, y);
            numbers.Count.Should().Be(parsedNumbers.Length);
            for (int i = 0; i <  parsedNumbers.Length; i++)
            {
                numbers[i].Should().Be(parsedNumbers[i]);
            }
        }

        [Theory]
        [InlineData("Data/Day03_Test.txt", 4361)]
        [InlineData("Data/Day03.txt", 522726)]
        public void Part1(string filename, long expectedAnswer)
        {
            var grid = FileIterator.LoadGrid(filename);
            long sum = SumParts(grid);
            sum.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day03_Test.txt", 467835)]
        [InlineData("Data/Day03.txt", 81721933)]
        public void Part2(string filename, long expectedAnswer)
        {
            var grid = FileIterator.LoadGrid(filename);
            long sum = 0;

            foreach ((int x, int y) in grid.Rectangle())
            {
                if (grid[x, y] != '*') continue;

                var numbers = GetNumbersAroundPoint(grid, x, y);
                if (numbers.Count != 2) continue;

                sum += numbers[0] * numbers[1];
            }

            sum.Should().Be(expectedAnswer);
        }
    }
}
