using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Utils.DataStructures;
using Xunit;

namespace Advent2023
{
    public class Day10
    {
        void Link(Graph<(int x, int y)> graph, (int x, int y) from, (int x, int y) to)
        {
            graph.AddNodeIfNotInGraph(from);
            graph.AddNodeIfNotInGraph(to);
            graph.AddOneWayLink(from, to);
        }

        (Graph<(int x, int y)>, (int x, int y) start) ParseGraph(string filename)
        {
            var graph = new Graph<(int x, int y)>();
            XY startXY = new();

            var grid = FileIterator.LoadGrid(filename);
            foreach (var pos in grid.Rectangle())
            {
                var c = grid[pos.x, pos.y];
                switch (c)
                {
                    case 'S':
                        startXY.x = pos.x;
                        startXY.y = pos.y;
                        break;

                    case '|':
                        Link(graph, pos, (pos.x, pos.y - 1));
                        Link(graph, pos, (pos.x, pos.y + 1));
                        break;

                    case '-':
                        Link(graph, pos, (pos.x - 1, pos.y));
                        Link(graph, pos, (pos.x + 1, pos.y));
                        break;

                    case 'L':
                        Link(graph, pos, (pos.x, pos.y - 1));
                        Link(graph, pos, (pos.x + 1, pos.y));
                        break;

                    case 'J':
                        Link(graph, pos, (pos.x, pos.y - 1));
                        Link(graph, pos, (pos.x - 1, pos.y));
                        break;

                    case '7':
                        Link(graph, pos, (pos.x - 1, pos.y));
                        Link(graph, pos, (pos.x, pos.y + 1));
                        break;

                    case 'F':
                        Link(graph, pos, (pos.x + 1, pos.y));
                        Link(graph, pos, (pos.x, pos.y + 1));
                        break;
                }
            }

            var start = startXY.ToTuple();

            foreach (var node in graph.Items.Where(n => graph.IsLinked(n, start)))
            {
                Link(graph, start, node);
            }

            return (graph, start);
        }

        Dictionary<(int x, int y), int> Score(Graph<(int x, int y)> graph, (int x, int y) start)
        {
            Dictionary<(int x, int y), int> scores = [];
            Queue<(int x, int y)> queue = [];
            queue.Enqueue(start);
            scores[start] = 0;
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                var nextScore = scores[node] + 1;
                foreach (var linked in graph.GetLinked(node))
                {
                    if (scores.TryGetValue(linked, out int score))
                        if (score <= nextScore)
                            continue;

                    scores[linked] = nextScore;
                    queue.Enqueue(linked);
                }
            }

            return scores;
        }

        void Fill(char[,] grid, (int x, int y) start)
        {
            if (grid[start.x, start.y] != '.')
                return;

            Queue<(int x, int y)> queue = [];
            queue.Enqueue(start);
            bool isOuter = true;

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();
                if ((x < 0) || (y < 0) || (x == grid.GetLength(0)) || (y == grid.GetLength(1)))
                {
                    continue;
                }
                if (grid[x, y] != '.')
                {
                    if (grid[x, y] == 'I')
                    {
                        // We found an inner tile so indicate that we're not filling an outer area
                        isOuter = false;
                    }
                    continue;
                }

                queue.Enqueue((x - 1, y));
                queue.Enqueue((x + 1, y));
                queue.Enqueue((x, y - 1));
                queue.Enqueue((x, y + 1));
                grid[x, y] = '?'; // Substitue in a value to replace once we know the replacement type
            }

            var replacement = isOuter ? 'O' : 'I';
            foreach (var pos in grid.Rectangle())
            {
                if (grid[pos.x, pos.y] == '?')
                {
                    grid[pos.x, pos.y] = replacement;
                }
            }
        }

        void CleanGrid(char[,] grid, Dictionary<(int x, int y), int> scores)
        {
            foreach (var pos in grid.Rectangle())
            {
                if (scores.ContainsKey(pos)) continue;
                grid[pos.x, pos.y] = '.';
            }
        }

        void UpdateStartType(char[,] grid, (int x, int y) start)
        {
            var u = grid[start.x, start.y - 1];
            var d = grid[start.x, start.y + 1];
            var l = grid[start.x - 1, start.y];
            char c;

            if (l == '-' || l == 'L' || l == 'F')
            {
                if (u == '|' || u == '7' || u == 'F') c = 'J';
                else if (d == '|' || d == 'J' || d == 'L') c = '7';
                else c = '-';
            }
            else
            {
                if (u == '|' || u == '7' || u == 'F') c = 'L';
                if (d == '|' || d == 'J' || d == 'L') c = 'F';
                else c = '|';
            }

            grid[start.x, start.y] = c;
        }

        void Tag(char[,] grid, (int x, int y) pos, char value)
        {
            if (pos.x < 0 || pos.y < 0) return;
            if (pos.x == grid.GetLength(0) || pos.y == grid.GetLength(1)) return;
            if (grid[pos.x, pos.y] != '.') return;
            grid[pos.x, pos.y] = value;
        }

        void Trace(char[,] grid, (int x, int y) start)
        {
            Tag(grid, (start.x - 1, start.y), 'O');
            Tag(grid, (start.x, start.y - 1), 'O');
            (int x, int y) pos = (start.x + 1, start.y);
            int dx = 1;
            int dy = 0;
            char tag;
            while (pos != start)
            {
                var c = grid[pos.x, pos.y];
                switch (c)
                {
                    case '-':
                        Tag(grid, (pos.x, pos.y - dx), 'O');
                        Tag(grid, (pos.x, pos.y + dx), 'I');
                        break;

                    case '|':
                        Tag(grid, (pos.x + dy, pos.y), 'O');
                        Tag(grid, (pos.x - dy, pos.y), 'I');
                        break;

                    case '7':
                        tag = dx == 1 ? 'O' : 'I';
                        Tag(grid, (pos.x, pos.y - 1), tag);
                        Tag(grid, (pos.x + 1, pos.y), tag);
                        (dx, dy) = (dy, dx);
                        break;

                    case 'J':
                        tag = dy == 1 ? 'O' : 'I';
                        Tag(grid, (pos.x, pos.y + 1), tag);
                        Tag(grid, (pos.x + 1, pos.y), tag);
                        (dx, dy) = (-dy, -dx);
                        break;

                    case 'L':
                        tag = dy == 1 ? 'I' : 'O';
                        Tag(grid, (pos.x, pos.y + 1), tag);
                        Tag(grid, (pos.x - 1, pos.y), tag);
                        (dx, dy) = (dy, dx);
                        break;

                    case 'F':
                        tag = dx == -1 ? 'I' : 'O';
                        Tag(grid, (pos.x, pos.y - 1), tag);
                        Tag(grid, (pos.x - 1, pos.y), tag);
                        (dx, dy) = (-dy, -dx);
                        break;
                }
                pos = (pos.x + dx, pos.y + dy);
            }
        }

        [Theory]
        [InlineData("Data/Day10_Test.txt", 8)]
        [InlineData("Data/Day10.txt", 6613)]
        public void Part1(string filename, int expectedAnswer)
        {
            var (graph, start) = ParseGraph(filename);
            var scores = Score(graph, start);
            scores.Values.Max().Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day10_Test2.txt", 4)]
        [InlineData("Data/Day10_Test3.txt", 8)]
        [InlineData("Data/Day10_Test4.txt", 10)]
        [InlineData("Data/Day10.txt", 511)]
        public void Part2(string filename, int expectedAnswer)
        {
            // Repeat part one to find all the tiles in the loop
            var (graph, start) = ParseGraph(filename);
            var scores = Score(graph, start);
            var grid = FileIterator.LoadGrid(filename);

            // Scrub out tiles that aren't part of the loop
            CleanGrid(grid, scores);
            // Replace the start tile with its actual reprisenation
            UpdateStartType(grid, start);

            // Find an outer corner to start tracing from
            foreach (var pos in grid.Rectangle())
            {
                if (grid[pos.x, pos.y] == 'F')
                {
                    start = pos;
                    break;
                }
            }

            // Trace clockwise around the loop marking tiles on our relative left as "outer"
            // and tiles on our relative right as "inner"
            Trace(grid, start);

            // Find any untagged cells and use a flood fill to identify them
            foreach (var pos in grid.Rectangle())
            {
                Fill(grid, pos);
            }

            // Count how many inner tiles we have
            int total = grid.Items().Where(v => v == 'I').Count();
            total.Should().Be(expectedAnswer);
        }
    }
}
