using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Drawing;
using Utils;
using Xunit;

namespace Advent2018
{
    public class Day17
    {
        enum CellState
        {
            Empty,
            Wall,
            Water,
        }

        class Stroke
        {
            public readonly int StartX;
            public readonly int StartY;
            public readonly int Extent;
            public readonly bool Horizontal;

            public Stroke(string input)
            {
                var match = input.Match(@"(.)=(\d+), .=(\d+)\.\.(\d+)");
                if (match.Groups.Count != 5) Oh.Bugger();

                Horizontal = match.Groups[1].Value == "y";
                if (Horizontal)
                {
                    StartY = int.Parse(match.Groups[2].Value);
                    StartX = int.Parse(match.Groups[3].Value);
                }
                else
                {
                    StartX = int.Parse(match.Groups[2].Value);
                    StartY = int.Parse(match.Groups[3].Value);
                }
                Extent = int.Parse(match.Groups[4].Value);
            }

            public int EndX => Horizontal ? Extent : StartX;
            public int EndY => Horizontal ? StartY : Extent;
        }

        (CellState[,] environment, int offsetX, int minY) LoadEnvironment(string filename)
        {
            var strokes = new List<Stroke>();
            var minX = int.MaxValue;
            var minY = int.MaxValue;
            var maxX = int.MinValue;
            var maxY = int.MinValue;
            FileIterator.ForEachLine<string>(filename, line =>
            {
                var stroke = new Stroke(line);
                strokes.Add(stroke);
                minX = Math.Min(minX, stroke.StartX);
                minY = Math.Min(minY, stroke.StartY);
                maxX = Math.Max(maxX, stroke.EndX);
                maxY = Math.Max(maxY, stroke.EndY);
            });

            var width = maxX - minX;
            var environment = new CellState[width + 3, maxY + 1];
            minX--;

            foreach (var stroke in strokes)
            {
                if (stroke.Horizontal)
                {
                    for (var x = stroke.StartX; x <= stroke.EndX; x++)
                        environment[x - minX, stroke.StartY] = CellState.Wall;
                }
                else
                {
                    for (var y = stroke.StartY; y <= stroke.EndY; y++)
                        environment[stroke.StartX - minX, y] = CellState.Wall;
                }
            }

            return (environment, minX, minY);
        }

        bool IsStable(CellState[,] environment, int x, int y)
        {
            if (environment[x, y] == CellState.Wall)
                return true;

            var xx = x;
            while (environment[xx, y] == CellState.Water)
            {
                xx++;
                if (xx == environment.GetLength(0) || environment[xx, y] == CellState.Empty)
                    return false;
            }

            xx = x;
            while (environment[xx, y] == CellState.Water)
            {
                xx--;
                if (xx == -1 || environment[xx, y] == CellState.Empty)
                    return false;
            }

            return true;
        }

        void Render(CellState[,] environment)
        {
            Renderer.RenderGrid(@"c:\temp\test.png", environment, cell =>
            {
                switch (cell)
                {
                    case CellState.Empty:
                        return Color.Black;

                    case CellState.Water:
                        return Color.Blue;

                    default:
                        return Color.Brown;
                }
            });
        }

        [Theory]
        [InlineData(57, 29, "Data/Day17-Test1.txt")]
        [InlineData(10, 3, "Data/Day17-Test2.txt")]
        [InlineData(20, 11, "Data/Day17-Test3.txt")]
        [InlineData(38364, 30551, "Data/Day17.txt")] // Solution
        void Problem1(int expectedCount, int expectedStable, string inputfile)
        {
            var (environment, offsetX, minY) = LoadEnvironment(inputfile);
            var waterX = 500 - offsetX;
            environment[waterX, 0] = CellState.Water;

            var waterFrontier = new Stack<(int x, int y)>();
            waterFrontier.Push((waterX, 0));

            while (waterFrontier.Count != 0)
            {
                var (x, y) = waterFrontier.Pop();
                if (y == environment.GetLength(1) - 1)
                    continue; // Hit the bottom

                if (environment[x, y + 1] == CellState.Empty)
                {
                    // Expand down
                    waterFrontier.Push((x, y));
                    waterFrontier.Push((x, y + 1));
                    environment[x, y + 1] = CellState.Water;
                }
                else if (IsStable(environment, x, y + 1))
                {
                    if (environment[x - 1, y] == CellState.Empty)
                    {
                        // Expand left along a stable surface
                        waterFrontier.Push((x, y));
                        waterFrontier.Push((x - 1, y));
                        environment[x - 1, y] = CellState.Water;
                    }
                    else if (environment[x + 1, y] == CellState.Empty)
                    {
                        // Expand right along a stable surface
                        waterFrontier.Push((x, y));
                        waterFrontier.Push((x + 1, y));
                        environment[x + 1, y] = CellState.Water;
                    }
                }
            }

            //Render(environment);

            var totalCount = 0;
            var stableCount = 0;
            foreach (var (x, y) in environment.Rectangle())
                if (y >= minY && environment[x, y] == CellState.Water)
                {
                    totalCount++;
                    if (IsStable(environment, x, y))
                        stableCount++;
                }

            totalCount.Should().Be(expectedCount);
            stableCount.Should().Be(expectedStable);
        }
    }
}
