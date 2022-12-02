using FluentAssertions;
using System.Linq;
using Utils;
using Xunit;
using Xunit.Abstractions;

namespace Advent2019
{
    public class Day17
    {
        private readonly ITestOutputHelper output;

        public Day17(ITestOutputHelper output)
        {
            this.output = output;
        }

        static char[,] Grid(string[] lines)
        {
            var grid = new char[lines[0].Length, lines.Length];

            for (int y = 0; y < lines.Length; y++)
            {
                var line = lines[y];
                for (int x = 0; x < line.Length; x++)
                {
                    grid[x, y] = line[x];
                }
            }

            return grid;
        }

        static int Alignment(char[,] grid)
        {
            var sum = 0;
            foreach ((var x, var y) in grid.Rectangle())
            {
                if (grid[x, y] != '#') continue;

                var count = grid.GetAdjacent(x, y)
                    .Where(c => c == '#')
                    .Count();

                if (count == 4)
                    sum += (x * y);
            }

            return sum;
        }

        [Fact]
        public void Part1_Example()
        {
            var lines = FileIterator.LoadLines<string>("Data/Day17_Test.txt");
            var grid = Grid(lines);
            Alignment(grid).Should().Be(76);
        }

        [Fact]
        public void Part1()
        {
            var vm = IntCode.CreateVM("Data/Day17.txt");
            vm.Execute();
            var lines = vm.ReadLines().ToArray();

            foreach (var line in lines)
                output.WriteLine(line);

            var grid = Grid(lines);
            Alignment(grid).Should().Be(3920);
        }

        [Fact]
        public void Part2()
        {
            var vm = IntCode.CreateVM("Data/Day17.txt", new() { [0] = 2 });

            vm.WriteLines(
                "A,A,B,C,B,C,B,C,C,A",
                "R,8,L,4,R,4,R,10,R,8",
                "L,12,L,12,R,8,R,8",
                "R,10,R,4,R,4",
                "n"
            );

            vm.Execute();

            foreach (var line in vm.ReadLines())
                output.WriteLine(line);

            var count = vm.Read();
            count.Should().Be(673996);
        }
    }
}
