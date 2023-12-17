using FluentAssertions;
using System;
using System.Collections.Generic;
using Utils;
using Utils.Alogrithms;
using Xunit;

namespace Advent2023
{
    public class Day17
    {
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
            Any
        }

        record State(int x, int y, Direction dir, int count);

        static int ResolveCount1(Direction dir, State node)
        {
            if (node.dir != dir)
            {
                return 1;
            }
            
            if (node.count < 3)
            {
                return node.count + 1;
            }

            return -1;
        }

        static int ResolveCount2(Direction dir, State node)
        {
            // Special case handling for the start state
            if (node.dir == Direction.Any)
            {
                return 1;
            }

            if (node.dir != dir)
            {
                if (node.count < 4) return -1;
                return 1;
            }
            
            if (node.count < 10)
            {
                return node.count + 1;
            }

            return -1;
        }

        class GraphAdapter(int[,] grid, Func<Direction, State, int> resolveCount) : Astar.IGraphAdapter<State>
        {
            State ResolveDirection(Direction dir, State node)
            {
                int dX;
                int dY;

                switch (dir)
                {
                    case Direction.Up:
                        if (node.dir == Direction.Down) return null;
                        dX = 0;
                        dY = -1;
                        break;

                    case Direction.Down:
                        if (node.dir == Direction.Up) return null;
                        dX = 0;
                        dY = 1;
                        break;

                    case Direction.Left:
                        if (node.dir == Direction.Right) return null;
                        dX = -1;
                        dY = 0;
                        break;

                    case Direction.Right:
                        if (node.dir == Direction.Left) return null;
                        dX = 1;
                        dY = 0;
                        break;

                    default:
                        throw new Exception();
                };

                int x = node.x + dX;
                int y = node.y + dY;

                if (!grid.IsInBounds(x, y)) return null;

                int count = resolveCount(dir, node);
                if (count == -1) return null;

                return new(x, y, dir, count);
            }

            public IEnumerable<State> GetLinked(State node)
            {
                var newState = ResolveDirection(Direction.Up, node);
                if (newState != null) yield return newState;

                newState = ResolveDirection(Direction.Down, node);
                if (newState != null) yield return newState;

                newState = ResolveDirection(Direction.Left, node);
                if (newState != null) yield return newState;

                newState = ResolveDirection(Direction.Right, node);
                if (newState != null) yield return newState;
            }

            public int GetMoveCost(State from, State to)
            {
                return grid[to.x, to.y];
            }

            public int GetScore(State from, State to)
            {
                return Math.Abs(from.x - to.x) + Math.Abs(from.y - to.y);
            }
        }

        long Solve(string filename, Func<Direction, State, int> resolveCount)
        {
            var grid = FileIterator.LoadGrid(filename, (c, x, y) => c - '0');
            var path = Astar.FindPath(
                new GraphAdapter(grid, resolveCount),
                new State(0, 0, Direction.Any, 0),
                new State(grid.GetLength(0) - 1, grid.GetLength(1) - 1, Direction.Any, 0)
            );

            long total = 0;
            foreach (var step in path)
            {
                total += grid[step.x, step.y];
            }
            // Remove the starting cell's heat value as we only incur it if we explicitly move into it
            total -= grid[0, 0];

            return total;
        }

        [Theory]
        [InlineData("Data/Day17_Test.txt", 102)]
        [InlineData("Data/Day17.txt", 814)]
        public void Part1(string filename, long expectedAnswer) => Solve(filename, ResolveCount1).Should().Be(expectedAnswer);

        [Theory]
        [InlineData("Data/Day17_Test.txt", 94)]
        [InlineData("Data/Day17.txt", 974)]
        public void Part2(string filename, long expectedAnswer) => Solve(filename, ResolveCount2).Should().Be(expectedAnswer);
    }
}
