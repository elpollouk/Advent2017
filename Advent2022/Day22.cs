using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2022
{
    public class Day22
    {
        enum Cell
        {
            OPEN,
            WALL,
            VOID
        }

        enum Op
        {
            LEFT,
            RIGHT,
            MOVE
        }

        struct Instruction
        {
            public Op Op;
            public int Operand;
        }

        Cell[,] ParseMap(Func<string> reader)
        {
            List<Cell[]> buffer = new();
            int width = 0;

            var line = reader();
            while (line.Length != 0)
            {
                var row = new Cell[line.Length];
                if (width < row.Length) width = row.Length;

                for (int x = 0; x < row.Length; x++)
                {
                    row[x] = line[x] switch
                    {
                        ' ' => Cell.VOID,
                        '.' => Cell.OPEN,
                        '#' => Cell.WALL,
                        _ => throw new Exception()
                    };
                }

                buffer.Add(row);
                line = reader();
            }

            var grid = new Cell[width, buffer.Count];
            for (var y = 0; y < buffer.Count; y++)
            {
                var row = buffer[y];
                for (int x = 0; x < width; x++)
                {
                    grid[x, y] = x < row.Length ? row[x] : Cell.VOID;
                }
            }
            return grid;
        }

        Instruction[] ParseInstructions(string line)
        {
            List<Instruction> instructions = new();
            int accumulator = 0;
            foreach (var c in line)
            {
                if ('0' <= c && c <= '9')
                {
                    accumulator *= 10;
                    accumulator += c - '0';
                }
                else if (c == 'L' || c == 'R')
                {
                    instructions.Add(new()
                    {
                        Op = Op.MOVE,
                        Operand = accumulator
                    });
                    accumulator = 0;
                    instructions.Add(new()
                    {
                        Op = c == 'L' ? Op.LEFT : Op.RIGHT
                    });
                }
                else
                {
                    throw new Exception();
                }
            }

            if (accumulator != 0)
            {
                instructions.Add(new()
                {
                    Op = Op.MOVE,
                    Operand = accumulator
                });
            }
            return instructions.ToArray();
        }

        void Exec(Cell[,] grid, XY pos, XY facing, Instruction instruction)
        {
            var Wrap = (XY pos) =>
            {
                if (pos.x == grid.GetLength(0)) pos.x = 0;
                else if (pos.x == -1) pos.x = grid.GetLength(0) - 1;
                else if (pos.y == grid.GetLength(1)) pos.y = 0;
                else if (pos.y == -1) pos.y = grid.GetLength(1) - 1;
            };

            switch (instruction.Op)
            {
                case Op.LEFT:
                    facing.RotateLeft();
                    break;

                case Op.RIGHT:
                    facing.RotateRight();
                    break;

                default:
                    for (int i = 0; i < instruction.Operand; i++)
                    {
                        var peek = pos.Clone().Add(facing);
                        Wrap(peek);
                        var c = grid[peek.x, peek.y];
                        while (c == Cell.VOID)
                        {
                            peek.Add(facing);
                            Wrap(peek);
                            c = grid[peek.x, peek.y];
                        }

                        if (c == Cell.WALL) break;
                        pos.Set(peek);
                    }
                    break;
            }
        }

        int FacingValue(XY facing)
        {
            if (facing.x == 1) return 0;
            if (facing.y == 1) return 1;
            if (facing.x == -1) return 2;
            return 3;
        }

        [Theory]
        [InlineData("Data/Day22_Test.txt", 6032)]
        [InlineData("Data/Day22.txt", 95358)]
        public void Part1(string filename, long expectedAnswer)
        {
            var reader = FileIterator.CreateLineReader(filename);
            var grid = ParseMap(reader);
            var instructions = ParseInstructions(reader());

            int x = 0;
            while (grid[x, 0] != Cell.OPEN)
            {
                x++;
            }
            XY pos = (x, 0);
            XY facing = (1, 0);

            foreach (var instruction in instructions)
            {
                Exec(grid, pos, facing, instruction);
            }

            long result = (pos.y + 1) * 1000;
            result += (pos.x + 1) * 4;
            result += FacingValue(facing);

            result.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day22_Test.txt", 0)]
        [InlineData("Data/Day22.txt", 0)]
        public void Part2(string filename, long expectedAnswer)
        {

        }
    }
}
