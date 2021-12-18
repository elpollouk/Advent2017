using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;
using Xunit.Abstractions;

namespace Advent2019
{
    public class Day19
    {
        private readonly ITestOutputHelper output;
        int[] prog;

        public Day19(ITestOutputHelper output)
        {
            this.output = output;
        }

        long StartX(long y) => (y * 2) - ((y - 1) / 3);

        long CheckPoint(long x, long y)
        {
            if (prog == null) prog = FileIterator.LoadCSV<int>("Data/Day19.txt");

            var vm = IntCode.CreateVM((int[])prog.Clone());
            vm.State.InputQueue.Enqueue(x);
            vm.State.InputQueue.Enqueue(y);
            vm.Execute();
            return vm.State.OutputQueue.Dequeue();
        }

        /*[Fact]
        public void StartX_Test()
        {
            for (var y = 1; y < 100; y++)
            {
                var x = StartX(y);
                CheckPoint(x, y).Should().Be(1);
                CheckPoint(x-1, y).Should().Be(0);
            }
        }*/


        [Fact]
        public void Part1()
        {
            var grid = new char[50, 50];

            foreach ((var x, var y) in grid.Rectangle())
                grid[x, y] = CheckPoint(x, y) == 1 ? '#' : '·';

            grid.DebugDump(output.WriteLine, c => c);

            grid.Items()
                .Where(c => c == '#')
                .Count()
                .Should().Be(179);
        }

        [Fact]
        public void Part2()
        {
            /*long x = 100;
            long y = 0;
            while (true)
            {
                if (CheckPoint(x, y) == 1)
                {
                    if (CheckPoint(x - 99, y + 99) == 1) break;
                    x++;
                    y = 0;
                }
                else
                {
                    y++;
                }
            }

            x -= 99;
            (x * 10000 + y).Should().Be(0);*/
        }
    }
}
