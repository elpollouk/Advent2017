using FluentAssertions;
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

        long CheckPoint(long x, long y)
        {
            if (prog == null) prog = FileIterator.LoadCSV<int>("Data/Day19.txt");

            var vm = IntCode.CreateVM((int[])prog.Clone());
            vm.Write(x);
            vm.Write(y);
            vm.Execute();
            return vm.Read();
        }

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
            long x = 100;
            long y = 0;
            while (true)
            {
                if (CheckPoint(x, y) == 1)
                {
                    if (CheckPoint(x - 99, y + 99) == 1) break;
                    x++;
                }
                else
                {
                    y++;
                }
            }

            x -= 99;
            (x * 10000 + y).Should().Be(9760485);
        }
    }
}
