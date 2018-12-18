using FluentAssertions;
using System.Collections.Generic;
using Utils;
using Xunit;

namespace Advent2018
{
    public class Day15
    {
        enum CellState
        {
            Clear,
            Wall,
            Elf,
            Golbin
        }

        struct PathStep
        {
            public readonly int DistanceFromStart;
            public readonly (int x, int y) Pos;
            public readonly (int x, int y) From;
    
            public PathStep(int distance, (int x, int y) pos, (int x, int y) from)
            {
                DistanceFromStart = distance;
                Pos = pos;
                From = from;
            }

            public bool IsQuickerThan(PathStep other)
            {
                if (DistanceFromStart < other.DistanceFromStart)
                    return true;
                if (DistanceFromStart > other.DistanceFromStart)
                    return false;

                return false;
            }
        }

        static CellState CharToCellState(char value)
        {
            switch (value)
            {
                case '.':
                    return CellState.Clear;

                case '#':
                    return CellState.Wall;

                case 'E':
                    return CellState.Elf;

                case 'G':
                    return CellState.Golbin;

                default:
                    Oh.Bugger();
                    return CellState.Clear;
            }
        }

        static char CellStateToChar(CellState cellState)
        {
            switch (cellState)
            {
                case CellState.Clear:
                    return '.';

                case CellState.Wall:
                    return '#';

                case CellState.Elf:
                    return 'E';

                case CellState.Golbin:
                    return 'G';

                default:
                    Oh.Bollocks();
                    return 'X';
            }
        }

        List<(int x, int y)> LoadPath(int[] input)
        {
            var path = new List<(int x, int y)>();

            for (var i = 0; i < input.Length; i += 2)
                path.Add((input[i], input[i + 1]));

            return path;
        }

        ICollection<(int x, int y)> PathToNearestTarget(CellState[,] grid, (int x, int y) start, params (int x, int y)[] targets)
        {
            var pathMap = new Dictionary<(int x, int y), PathStep>();
            var frontier = new Queue<PathStep>();
            frontier.Enqueue(new PathStep(0, start, start));

            while (frontier.Count != 0)
            {
                var step = frontier.Dequeue();
                if (!pathMap.TryGetValue((step.Pos.x, step.Pos.y), out PathStep existingStep))
                    existingStep = new PathStep(int.MaxValue, (0, 0), (0, 0));

                if (step.IsQuickerThan(existingStep))
                {
                    pathMap[step.Pos] = step;

                    if (grid[step.Pos.x, step.Pos.y - 1] == CellState.Clear)
                        frontier.Enqueue(new PathStep(step.DistanceFromStart + 1, (step.Pos.x, step.Pos.y - 1), step.Pos));

                    if (grid[step.Pos.x - 1, step.Pos.y] == CellState.Clear)
                        frontier.Enqueue(new PathStep(step.DistanceFromStart + 1, (step.Pos.x - 1, step.Pos.y), step.Pos));

                    if (grid[step.Pos.x + 1, step.Pos.y] == CellState.Clear)
                        frontier.Enqueue(new PathStep(step.DistanceFromStart + 1, (step.Pos.x + 1, step.Pos.y), step.Pos));

                    if (grid[step.Pos.x, step.Pos.y + 1] == CellState.Clear)
                        frontier.Enqueue(new PathStep(step.DistanceFromStart + 1, (step.Pos.x, step.Pos.y + 1), step.Pos));
                }
            }

            var path = new LinkedList<(int x, int y)>();
            if (pathMap.ContainsKey(targets[0]))
            {
                var location = pathMap[targets[0]];
                while (location.Pos.x != start.x || location.Pos.y != start.y)
                {
                    path.AddFirst(location.Pos);
                    location = pathMap[location.From];
                }
            }
            return path;
        }


        [Theory]
        [InlineData(2, 3, 6, 3, 2, 2, 3, 2, 4, 2, 5, 2, 6, 2, 6, 3)]
        void TestNavigation(int startX, int startY, int endX, int endY, params int[] expectedPath)
        {
            var environment = FileIterator.LoadGrid("Data/Day15-NavTest.txt", CharToCellState);
            environment.DebugDump(CellStateToChar);

            var path = LoadPath(expectedPath);
            PathToNearestTarget(environment, (startX, startY), (endX, endY)).Should().BeEquivalentTo(path, options => options.WithStrictOrdering());
        }
    }
}
