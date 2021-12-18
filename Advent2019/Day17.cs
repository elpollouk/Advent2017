using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        static IEnumerable<string> ReadLines(IntCode.VM vm)
        {
            StringBuilder sb = new();
            var q = vm.State.OutputQueue;
            while (q.Count != 0)
            {
                if (q.Peek() >= 128) yield break;

                var c = (char)q.Dequeue();
                if (c == '\n')
                {
                    var line = sb.ToString();
                    yield return line;
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }

            if (sb.Length != 0) yield return sb.ToString();
        }

        static void EnqueueInput(IntCode.VM vm, params string[] input)
        {
            var q = vm.State.InputQueue;
            foreach (var line in input)
            {
                if (line.Length > 20) throw new InvalidOperationException($"Line too long: {line}");
                foreach (var c in line)
                    q.Enqueue(c);

                q.Enqueue('\n');
            }
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

                var count = grid.GetAdjecent(x, y)
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
            var prog = FileIterator.LoadCSV<int>("Data/Day17.txt");
            var vm = IntCode.CreateVM(prog);
            vm.Execute();
            var lines = ReadLines(vm).ToArray();

            foreach (var line in lines)
                output.WriteLine(line);

            var grid = Grid(lines);
            Alignment(grid).Should().Be(3920);
        }

        [Fact]
        public void Part2()
        {
            var prog = FileIterator.LoadCSV<int>("Data/Day17.txt");
            prog[0] = 2;
            var vm = IntCode.CreateVM(prog);

            EnqueueInput(vm,
                "A,A,B,C,B,C,B,C,C,A",
                "R,8,L,4,R,4,R,10,R,8",
                "L,12,L,12,R,8,R,8",
                "R,10,R,4,R,4",
                "n"
            );

            vm.Execute();

            foreach (var line in ReadLines(vm))
                output.WriteLine(line);

            var count = vm.State.OutputQueue.Dequeue();
            count.Should().Be(673996);
        }
    }
}
