using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;
using Xunit.Abstractions;

namespace Advent2021
{
    public class Day13
    {
        private readonly ITestOutputHelper output;

        public Day13(ITestOutputHelper output)
        {
            this.output = output;
        }

        static bool[,] LoadPaper(string filename)
        {
            var width = 0;
            var height = 0;
            var reader = FileIterator.CreateLineReader(filename);
            for (var line = reader(); line != ""; line = reader())
            {
                var parts = line.Split(",");
                var x = int.Parse(parts[0]);
                var y = int.Parse(parts[1]);
                if (width < x) width = x;
                if (height < y) height = y;
            }

            var paper = new bool[width + 1, height + 1];
            reader = FileIterator.CreateLineReader(filename);
            for (var line = reader(); line != ""; line = reader())
            {
                var parts = line.Split(",");
                var x = int.Parse(parts[0]);
                var y = int.Parse(parts[1]);
                paper[x, y] = true;
            }

            return paper;
        }

        static List<int> LoadFolds(string filename)
        {
            var reader = FileIterator.CreateLineReader(filename);
            while (reader() != "") { }

            var folds = new List<int>();
            
            for (var line = reader(); line != null; line = reader())
            {
                var parts = line.Groups("fold along (.)=(\\d+)");
                var axis = int.Parse(parts[1]);
                if (parts[0] == "y")
                {
                    axis = -axis;
                }
                folds.Add(axis);
            }
            
            return folds;
        }

        static void Fold(bool[,] paper, int axis)
        {
            if (axis < 0)
            {
                // Fold along Y
                axis = -axis;
                for (var y = axis + 1; y < paper.GetLength(1); y++)
                {
                    var newY = axis - (y - axis);
                    for (var x = 0; x < paper.GetLength(0); x++)
                    {
                        if (paper[x, y])
                        {
                            paper[x, y] = false;
                            paper[x, newY] = true;
                        }
                    }
                }
            }
            else
            {
                // Fold along X
                for (var x = axis + 1; x < paper.GetLength(0); x++)
                {
                    var newX = axis - (x - axis);
                    for (var y = 0; y < paper.GetLength(1); y++)
                    {
                        if (paper[x, y])
                        {
                            paper[x, y] = false;
                            paper[newX, y] = true;
                        }
                    }
                }
            }
        }

        static bool[,] Crop(bool[,] paper)
        {
            int width = 0;
            int height = 0;
            foreach (var (x, y) in paper.Rectangle())
            {
                if (paper[x, y])
                {
                    if (x > width) width = x;
                    if (y > height) height = y;
                }
            }

            var cropped = new bool[width + 1, height + 1];

            foreach (var (x, y) in paper.Rectangle())
            {
                if (paper[x, y])
                {
                    cropped[x, y] = true;
                }
            }

            return cropped;
        }

        [Theory]
        [InlineData("Data/Day13_Test.txt", 17)]
        [InlineData("Data/Day13.txt", 807)]
        public void Part1(string filename, int expectedAnswer)
        {
            var paper = LoadPaper(filename);
            var folds = LoadFolds(filename);

            Fold(paper, folds[0]);

            paper.Items()
                .Select(b => b ? 1 : 0)
                .Sum()
                .Should()
                .Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day13_Test.txt", 16)]
        [InlineData("Data/Day13.txt", 98)]
        [InlineData("Data/Day13_Gee.txt", 102)]
        public void Part2(string filename, int expectedAnswer)
        {
            var paper = LoadPaper(filename);
            var folds = LoadFolds(filename);

            foreach (var fold in folds)
                Fold(paper, fold);

            paper.Items()
                .Select(b => b ? 1 : 0)
                .Sum()
                .Should()
                .Be(expectedAnswer);

            Crop(paper).DebugDump(output.WriteLine, b => b ? '█' : ' ');
        }
    }
}
