using FluentAssertions;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2022
{
    public class Day08
    {
        void MarkVisibleX(int[,] grid, bool[,] visible, int step)
        {
            int start = step == 1 ? 0 : grid.GetLength(0) - 1;
            int limit = step == 1 ? grid.GetLength(0) : -1;

            for (int y = 0; y < grid.GetLength(1); y++)
            {
                int maxHeight = int.MinValue;

                int x = start;
                while (x != limit)
                {
                    if (maxHeight < grid[y, x])
                    {
                        maxHeight = grid[y, x];
                        visible[y, x] = true;
                    }
                    x += step;
                }
            }
        }

        void MarkVisibleY(int[,] grid, bool[,] visible, int step)
        {
            int start = step == 1 ? 0 : grid.GetLength(1) - 1;
            int limit = step == 1 ? grid.GetLength(1) : -1;

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                int maxHeight = int.MinValue;

                int y = start;
                while (y != limit)
                {
                    if (maxHeight < grid[y, x])
                    {
                        maxHeight = grid[y, x];
                        visible[y, x] = true;
                    }
                    y += step;
                }
            }
        }

        void ScoreScenicX(int[,] grid, long[,] score, int step)
        {
            int start = step == 1 ? 0 : grid.GetLength(0) - 1;
            int limit = step == 1 ? grid.GetLength(0) : -1;

            for (int y = 0; y < grid.GetLength(1); y++)
            {
                int x = start;
                while (x != limit)
                {
                    int x2 = x + step;
                    int count = 0;
                    while (x2 != limit && grid[x2, y] < grid[x, y])
                    {
                        count++;
                        x2 += step;
                    }
                    if (x2 != limit) count++;
                    score[x, y] *= count;
                    x += step;
                }
            }
        }

        void ScoreScenicY(int[,] grid, long[,] score, int step)
        {
            int start = step == 1 ? 0 : grid.GetLength(1) - 1;
            int limit = step == 1 ? grid.GetLength(1) : -1;

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                int y = start;
                while (y != limit)
                {
                    int y2 = y + step;
                    int count = 0;
                    while (y2 != limit && grid[x, y2] < grid[x, y])
                    {
                        count++;
                        y2 += step;
                    }
                    if (y2 != limit) count++;
                    score[x, y] *= count;
                    y += step;
                }
            }
        }

        long CountVisible(int[,] grid)
        {
            var visible = new bool[grid.GetLength(0), grid.GetLength(1)];
            MarkVisibleX(grid, visible, 1);
            MarkVisibleX(grid, visible, -1);
            MarkVisibleY(grid, visible, 1);
            MarkVisibleY(grid, visible, -1);

            return visible.Items().Where(v => v).Count();
        }

        long[,] ScoreScenic(int[,] grid)
        {
            var score = new long[grid.GetLength(0), grid.GetLength(1)];
            foreach (var (x, y) in score.Rectangle()) {
                score[x, y] = 1;
            }
            ScoreScenicX(grid, score, 1);
            ScoreScenicX(grid, score, -1);
            ScoreScenicY(grid, score, 1);
            ScoreScenicY(grid, score, -1);
            return score;
        }

        [Theory]
        [InlineData("Data/Day08_Test.txt", 21)]
        [InlineData("Data/Day08.txt", 1814)]
        public void Part1(string filename, long expectedAnswer)
        {
            var grid = FileIterator.LoadGrid(filename, ArrayExtensions.CharToInt);
            var count = CountVisible(grid);
            count.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day08_Test.txt", 8)]
        [InlineData("Data/Day08.txt", 330786)]
        public void Part2(string filename, long expectedAnswer)
        {
            var grid = FileIterator.LoadGrid(filename, ArrayExtensions.CharToInt);
            var score = ScoreScenic(grid);
            var highest = score.Items().Max();
            highest.Should().Be(expectedAnswer);
        }
    }
}
