using Utils;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Advent2017
{
    using GridWriter = Action<bool[,], int, int>;
    using GridRules = Dictionary<string, Action<bool[,], int, int>>;

    public class Problem2101
    {
        GridWriter CreateWriter(string pattern)
        {
            return (grid, x, y) =>
            {
                var cX = x;
                var cY = y;
                foreach (var c in pattern)
                {
                    switch (c)
                    {
                        case '.':
                            grid[cX++, cY] = false;
                            break;

                        case '#':
                            grid[cX++, cY] = true;
                            break;

                        case '/':
                            cX = x;
                            cY++;
                            break;

                        default:
                            Oh.Bollocks();
                            break;
                    }
                }
            };
        }

        string ReadGrid(bool[,] grid, int size, int x, int y)
        {
            var result = "";
            for (var yy = y; yy < y + size; yy++)
                for (var xx = x; xx < x + size; xx++)
                    result += grid[xx, yy] ? '#' : '.';

            return result;
        }

        bool[,] RotateGrid(bool[,] grid, int size)
        {
            var newGrid = new bool[size, size];
            for (var y = 0; y < size; y++)
                for (var x = 0; x < size; x++)
                    newGrid[size - y - 1, x] = grid[x, y];

            return newGrid;
        }

        bool[,] MirrorGrid(bool[,] grid, int size)
        {
            var newGrid = new bool[size, size];
            for (var y = 0; y < size; y++)
                for (var x = 0; x < size; x++)
                    newGrid[size - x - 1, y] = grid[x, y];

            return newGrid;
        }

        List<string> CreateRotations(string pattern)
        {
            var rotations = new List<string>();

            int gridSize = -1;
            bool[,] grid = null;
            if (pattern.Length == 5)
            {
                gridSize = 2;
                grid = new bool[2, 2];
            }
            else if (pattern.Length == 11)
            {
                gridSize = 3;
                grid = new bool[3, 3];
            }

            var writer = CreateWriter(pattern);
            writer(grid, 0, 0);

            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    var gridPattern = ReadGrid(grid, gridSize, 0, 0);
                    if (!rotations.Contains(gridPattern))
                        rotations.Add(gridPattern);
                    grid = RotateGrid(grid, gridSize);
                }
                grid = MirrorGrid(grid, gridSize);
            }


            return rotations;
        }

        void ParseRule(GridRules rules, string rule)
        {
            var fromTo = rule.Replace(" => ", "|").Split('|');
            var rotations = CreateRotations(fromTo[0]);
            var writer = CreateWriter(fromTo[1]);

            foreach (var rotation in rotations)
            {
                if (rules.ContainsKey(rotation)) Oh.Bugger();
                rules[rotation] = writer;
            }
        }

        GridRules LoadRules(string datafile)
        {
            var rules = new GridRules();
            FileIterator.ForEachLine<string>(datafile, line =>
            {
                ParseRule(rules, line);
            });

            return rules;
        }

        bool[,] CreateInitalGrid()
        {
            var grid = new bool[3, 3];
            var writer = CreateWriter(".#./..#/###");
            writer(grid, 0, 0);
            return grid;
        }

        int Solve1(string datafile, int iterations)
        {
            var rules = LoadRules(datafile);

            var grid = CreateInitalGrid();
            var gridWidth = 3;


            for (var i = 0; i < iterations; i++)
            {
                int subGridSize;
                int numGrids;
                int newGridWidth;
                int gridStep;
                if (gridWidth % 2 == 0)
                {
                    subGridSize = 2;
                    numGrids = gridWidth / 2;
                    newGridWidth = numGrids * 3;
                    gridStep = 3;
                }
                else
                {
                    subGridSize = 3;
                    numGrids = gridWidth / 3;
                    newGridWidth = numGrids * 4;
                    gridStep = 4;
                }

                var newGrid = new bool[newGridWidth, newGridWidth];

                var newX = 0;
                var newY = 0;

                for (var y = 0; y < numGrids; y++)
                {
                    for (var x = 0; x < numGrids; x++)
                    {
                        var pattern = ReadGrid(grid, subGridSize, x * subGridSize, y * subGridSize);
                        var writer = rules[pattern];
                        writer(newGrid, newX, newY);
                        newX += gridStep;
                    }
                    newX = 0;
                    newY += gridStep;
                }

                gridWidth = newGridWidth;
                grid = newGrid;
            }

            var total = 0;
            foreach (var v in grid)
                if (v) total++;

            return total;
        }

        [Theory]
        [InlineData("###/###/###")]
        [InlineData("###/#.#/###")]
        [InlineData(".../.#./...")]
        [InlineData("##/##")]
        [InlineData("../..")]
        void NoRotations(string input)
        {
            var rotations = CreateRotations(input);
            rotations.Count.Should().Be(1);
            rotations[0].Should().Be(input.Replace("/", ""));
        }

        [Fact]
        void Rotation3x3_8()
        {
            var rotations = CreateRotations("###/.../#..");
            rotations.Count.Should().Be(8);
            rotations.Should().Contain("###...#..");
            rotations.Should().Contain("#.#..#..#");
            rotations.Should().Contain("..#...###");
            rotations.Should().Contain("#..#..#.#");
            rotations.Should().Contain("###.....#");
            rotations.Should().Contain("..#..##.#");
            rotations.Should().Contain("#.....###");
            rotations.Should().Contain("#.##..#..");
        }

        [Fact]
        void Rotation3x3_2()
        {
            var rotations = CreateRotations(".../###/...");
            rotations.Count.Should().Be(2);
            rotations.Should().Contain("...###...");
            rotations.Should().Contain(".#..#..#.");
        }

        [Theory]
        [InlineData("Data/2101-example.txt", 2, 12)]
        [InlineData("Data/2101.txt", 5, 117)]
        [InlineData("Data/2101.txt", 18, 2026963)]
        void Part1(string datafile, int iterations, int answer) => Solve1(datafile, iterations).Should().Be(answer);
    }
}
