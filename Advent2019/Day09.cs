using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2019
{
    public class Day09
    {
        [Theory]
        [InlineData("Data/Day09-example.txt")]
        public void Example1(string input)
        {
            var prog = FileIterator.LoadCSV<int>(input);
            var vm = IntCode.CreateVM((int[])prog.Clone());
            var output = new List<long>();
            vm.State.Output = v => output.Add(v);
            vm.Execute();

            for (int i = 0; i < prog.Length; i++)
                output[i].Should().Be(prog[i]);
        }

        [Theory]
        [InlineData("Data/Day09-example2.txt")]
        public void Example2(string input)
        {
            var prog = FileIterator.LoadCSV<int>(input);
            var vm = IntCode.CreateVM((int[])prog.Clone());
            long output = 0;
            vm.State.Output = v => output = v;
            vm.Execute();

            $"{output}".Length.Should().Be(16);
        }

        [Theory]
        [InlineData("Data/Day09-example3.txt")]
        public void Example3(string input)
        {
            var prog = FileIterator.LoadCSV<long>(input);
            var vm = IntCode.CreateVM(prog);
            long output = 0;
            vm.State.Output = v => output = v;
            vm.Execute();

            output.Should().Be(1125899906842624);
        }

        [Fact]
        void TestGP()
        {
            var prog = new int[] { 109, 19, 204, -34, 99 };
            var vm = IntCode.CreateVM(prog);
            vm.State.Mem[1985] = 12345;
            vm.State.GP = 2000;

            long output = 0;
            vm.State.Output = o => output = o;
            vm.Execute();
            vm.State.GP.Should().Be(2019);
            output.Should().Be(12345);
        }

        [Theory]
        [InlineData("Data/Day09.txt", 1, 3598076521)]
        [InlineData("Data/Day09.txt", 2, 90722)]
        public void Part1(string input, int mode, long answer)
        {
            var prog = FileIterator.LoadCSV<long>(input);
            var vm = IntCode.CreateVM(prog);
            vm.State.InputQueue.Enqueue(mode);
            
            long output = long.MinValue;
            vm.State.Output = v => output = v;
            vm.Execute();

            output.Should().Be(answer);
        }
    }
}
