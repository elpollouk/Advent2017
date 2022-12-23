using FluentAssertions;
using System;
using Utils;
using Xunit;

using Grid = System.Collections.Generic.Dictionary<(int x, int y), Advent2022.Day23.Elf>;

namespace Advent2022
{
    public class Day23
    {
        public class Elf
        {
            public XY currentPos;
            public XY prevPos;
        }

        bool IsIsolated(Grid grid, XY pos)
        {
            if (grid.ContainsKey((pos.x - 1, pos.y - 1))) return false;
            if (grid.ContainsKey((pos.x, pos.y - 1))) return false;
            if (grid.ContainsKey((pos.x + 1, pos.y - 1))) return false;

            if (grid.ContainsKey((pos.x - 1, pos.y))) return false;
            if (grid.ContainsKey((pos.x + 1, pos.y))) return false;
            
            if (grid.ContainsKey((pos.x - 1, pos.y + 1))) return false;
            if (grid.ContainsKey((pos.x, pos.y + 1))) return false;
            if (grid.ContainsKey((pos.x + 1, pos.y + 1))) return false;

            return true;
        }

        static bool TryMoveNorth(Grid grid, XY pos)
        {
            if (grid.ContainsKey((pos.x - 1, pos.y - 1))) return false;
            if (grid.ContainsKey((pos.x, pos.y - 1))) return false;
            if (grid.ContainsKey((pos.x + 1, pos.y - 1))) return false;
            pos.Add(0, -1);
            return true;
        }

        static bool TryMoveSouth(Grid grid, XY pos)
        {
            if (grid.ContainsKey((pos.x - 1, pos.y + 1))) return false;
            if (grid.ContainsKey((pos.x, pos.y + 1))) return false;
            if (grid.ContainsKey((pos.x + 1, pos.y + 1))) return false;
            pos.Add(0, 1);
            return true;
        }

        static bool TryMoveWest(Grid grid, XY pos)
        {
            if (grid.ContainsKey((pos.x - 1, pos.y - 1))) return false;
            if (grid.ContainsKey((pos.x - 1, pos.y))) return false;
            if (grid.ContainsKey((pos.x - 1, pos.y + 1))) return false;
            pos.Add(-1, 0);
            return true;
        }

        static bool TryMoveEast(Grid grid, XY pos)
        {
            if (grid.ContainsKey((pos.x + 1, pos.y - 1))) return false;
            if (grid.ContainsKey((pos.x + 1, pos.y))) return false;
            if (grid.ContainsKey((pos.x + 1, pos.y + 1))) return false;
            pos.Add(1, 0);
            return true;
        }

        void Rollback(Grid grid, Elf elf)
        {
            if (elf.prevPos == null) return;
            grid.Remove(elf.currentPos.ToTuple());
            elf.currentPos = elf.prevPos;
            elf.prevPos = null;
            if (grid.TryGetValue(elf.currentPos.ToTuple(), out var next))
            {
                Rollback(grid, next);
            }
            grid[elf.currentPos.ToTuple()] = elf;
        }

        void Rotate(Func<Grid, XY, bool>[] ops)
        {
            var t = ops[0];
            for (var i = 0; i < 3; i++)
            {
                ops[i] = ops[i + 1];
            }
            ops[3] = t;
        }

        (Grid, bool) Step(Grid grid, Func<Grid, XY,bool>[] ops)
        {
            Grid next = new();
            foreach (var elf in grid.Values)
            {
                elf.prevPos = null;
            }

            foreach (var elf in grid.Values)
            {
                if (!IsIsolated(grid, elf.currentPos))
                {
                    var nextPos = elf.currentPos.Clone();
                    foreach (var op in ops)
                    {
                        if (op(grid, nextPos))
                        {
                            elf.prevPos = elf.currentPos;
                            elf.currentPos = nextPos;
                            break;
                        }
                    }
                }

                if (next.TryGetValue(elf.currentPos.ToTuple(), out var existingElf))
                {
                    Rollback(next, existingElf);
                    elf.currentPos = elf.prevPos;
                    elf.prevPos = null;
                }
                next[elf.currentPos.ToTuple()] = elf;
            }

            bool moved = false;
            foreach (var elf in grid.Values)
            {
                if (elf.prevPos != null) moved = true;
            }

            return (next, moved);
        }

        ((int x, int y) from, (int x, int y) to) GetExtents(Grid grid)
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;

            foreach (var pos in grid.Keys)
            {
                if (pos.x < minX) minX = pos.x;
                if (pos.x > maxX) maxX = pos.x;
                if (pos.y < minY) minY = pos.y;
                if (pos.y > maxY) maxY = pos.y;
            }

            return ((minX, minY), (maxX + 1, maxY + 1));
        }

        Grid LoadGrid(string filename)
        {
            Grid grid = new();

            var y = 0;
            foreach (var line in FileIterator.Lines(filename))
            {
                for (int x = 0; x < line.Length; x++)
                {
                    if (line[x] != '#') continue;

                    Elf elf = new Elf
                    {
                        currentPos = (x, y)
                    };
                    grid[elf.currentPos.ToTuple()] = elf;
                }
                y++;
            }

            return grid;
        }

        [Theory]
        [InlineData("Data/Day23_Test.txt", 110)]
        [InlineData("Data/Day23.txt", 4254)]
        public void Part1(string filename, long expectedAnswer)
        {
            var grid = LoadGrid(filename);
            var ops = new[]
            {
                TryMoveNorth,
                TryMoveSouth,
                TryMoveWest,
                TryMoveEast
            };

            for (int i = 0; i < 10; i++)
            {
                (grid, var _) = Step(grid, ops);
                Rotate(ops);
            }

            var extents = GetExtents(grid);
            var width = extents.to.x - extents.from.x;
            var height = extents.to.y - extents.from.y;

            long count = (width * height) - grid.Count;
            count.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day23_Test.txt", 20)]
        [InlineData("Data/Day23.txt", 992)]
        public void Part2(string filename, long expectedAnswer)
        {
            var grid = LoadGrid(filename);
            var ops = new[]
            {
                TryMoveNorth,
                TryMoveSouth,
                TryMoveWest,
                TryMoveEast
            };

            long count = 0;
            bool moved = true;
            while (moved)
            {
                (grid, moved) = Step(grid, ops);
                Rotate(ops);
                count++;
            }

            count.Should().Be(expectedAnswer);
        }
    }
}
