using FluentAssertions;
using System;
using System.Collections.Generic;
using Utils;
using Xunit;

using WrapMap = System.Collections.Generic.Dictionary<((int x, int y) pos, (int x, int y) facing), ((int x, int y) pos, (int x, int y) facing)>;

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

        void Step(Cell[,] grid, Action<XY, XY> wrap, XY pos, XY facing, Instruction instruction)
        {
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
                        wrap(peek, facing);
                        var c = grid[peek.x, peek.y];
                        while (c == Cell.VOID)
                        {
                            peek.Add(facing);
                            wrap(peek, facing);
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

        long Exec(Cell[,] grid, Action<XY, XY> wrap, Instruction[] instructions)
        {
            int x = 0;
            while (grid[x, 0] != Cell.OPEN) x++;
            XY pos = (x, 0);
            XY facing = (1, 0);

            foreach (var instruction in instructions)
                Step(grid, wrap, pos, facing, instruction);

            long result = (pos.y + 1) * 1000;
            result += (pos.x + 1) * 4;
            result += FacingValue(facing);

            return result;
        }

        Action<XY, XY> WrapPart1(Cell[,] grid)
        {
            return (pos, facing) =>
            {
                if (pos.x == grid.GetLength(0)) pos.x = 0;
                else if (pos.x == -1) pos.x = grid.GetLength(0) - 1;
                else if (pos.y == grid.GetLength(1)) pos.y = 0;
                else if (pos.y == -1) pos.y = grid.GetLength(1) - 1;
            };
        }

        [Theory]
        [InlineData("Data/Day22_Test.txt", 6032)]
        [InlineData("Data/Day22.txt", 95358)]
        public void Part1(string filename, long expectedAnswer)
        {
            var reader = FileIterator.CreateLineReader(filename);
            var grid = ParseMap(reader);
            var instructions = ParseInstructions(reader());
            var wrap = WrapPart1(grid);

            var result = Exec(grid, wrap, instructions);

            result.Should().Be(expectedAnswer);
        }

        void WrapSeam(Cell[,] grid, WrapMap wrappings, int length, Func<int, (int x, int y)> fnFromPos, Func<int, (int x, int y)> fnToPos, (int x, int y) fromFacing, (int x, int y) toFacing)
        {
            for (int i = 0; i < length; i++)
            {
                var fromPos = fnFromPos(i);
                var toPos = fnToPos(i);

                var cell = grid[toPos.x, toPos.y];
                var facing = cell == Cell.OPEN ? (-toFacing.x, -toFacing.y) : fromFacing;
                wrappings[((fromPos.x + fromFacing.x, fromPos.y + fromFacing.y), fromFacing)] = (toPos, facing);

                cell = grid[fromPos.x, fromPos.y];
                facing = cell == Cell.OPEN ? (-fromFacing.x, -fromFacing.y) : toFacing;
                wrappings[((toPos.x + toFacing.x, toPos.y + toFacing.y), toFacing)] = (fromPos, facing);
            }
        }

        WrapMap BuildTestWrappingMap(Cell[,] grid)
        {
            WrapMap wrappings = new();

            WrapSeam(grid, wrappings, 4, i => (8 + i, 0), i => (3 - i, 4), (0, -1), (0, -1));      // 1-12
            WrapSeam(grid, wrappings, 4, i => (11, i), i => (15, 11 - i), (1, 0), (1, 0));         // 2-5
            WrapSeam(grid, wrappings, 4, i => (11, 4 + i), i => (15 - i, 8), (1, 0), (0, -1));     // 3-4
            WrapSeam(grid, wrappings, 4, i => (12 + i, 11), i => (0, 7 - i), (0, 1), (-1, 0));     // 6-11
            WrapSeam(grid, wrappings, 4, i => (8 + i, 11), i => (3 - i, 7), (0, 1), (0, 1));       // 7-10
            WrapSeam(grid, wrappings, 4, i => (8, 8 + i), i => (7 - i, 7), (-1, 0), (0, 1));       // 8-9
            WrapSeam(grid, wrappings, 4, i => (4 + i, 4), i => (8, i), (0, -1), (-1, 0));          // 13-14

            return wrappings;
        }

        WrapMap BuildSolutionWrappingMap(Cell[,] grid)
        {
            WrapMap wrappings = new();

            WrapSeam(grid, wrappings, 50, i => (50 + i, 0), i => (0, 150 + i), (0, -1), (-1, 0));      // 1-10
            WrapSeam(grid, wrappings, 50, i => (100 + i, 0), i => (i, 199), (0, -1), (0, 1));          // 2-9
            WrapSeam(grid, wrappings, 50, i => (149, i), i => (99, 149 - i), (1, 0), (1, 0));          // 3-6
            WrapSeam(grid, wrappings, 50, i => (100 + i, 49), i => (99, 50 + i), (0, 1), (1, 0));      // 4-5
            WrapSeam(grid, wrappings, 50, i => (50 + i, 149), i => (49, 150 + i), (0, 1), (1, 0));     // 7-8
            WrapSeam(grid, wrappings, 50, i => (0, 100 + i), i => (50, 49 - i), (-1, 0), (-1, 0));     // 11-14
            WrapSeam(grid, wrappings, 50, i => (i, 100), i => (50, 50 + i), (0, -1), (-1, 0));         // 12-13

            return wrappings;
        }

        Action<XY, XY> WrapPart2(WrapMap wrappings)
        {
            return (pos, facing) =>
            {
                if (wrappings.TryGetValue((pos.ToTuple(), facing.ToTuple()), out var dest))
                {
                    pos.Set(dest.pos);
                    facing.Set(dest.facing);
                }
            };
        }

        [Theory]
        [InlineData("Data/Day22_Test.txt", 5031)]
        [InlineData("Data/Day22.txt", 144361)]
        public void Part2(string filename, long expectedAnswer)
        {
            var reader = FileIterator.CreateLineReader(filename);
            var grid = ParseMap(reader);
            var instructions = ParseInstructions(reader());
            WrapMap wrapping = filename.Contains("Test") ? BuildTestWrappingMap(grid) : BuildSolutionWrappingMap(grid);
            var wrap = WrapPart2(wrapping);

            var result = Exec(grid, wrap, instructions);

            result.Should().Be(expectedAnswer);
        }
    }
}
