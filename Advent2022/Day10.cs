using FluentAssertions;
using System;
using Utils;
using Xunit;
using Xunit.Abstractions;

namespace Advent2022
{
    public class Day10
    {
        private readonly ITestOutputHelper output;

        public Day10(ITestOutputHelper output)
        {
            this.output = output;
        }

        enum Instruction
        {
            NOOP,
            ADDX
        }

        class VM
        {
            public long cycle;
            public long nextOutput;
            public long x;

            public Action<VM> OnClock;
            

            public VM()
            {
                x = 1;
                nextOutput = 20;
            }

            public void Execute(string[] prog)
            {
                foreach (var line in prog)
                {
                    var op = Decode(line);
                    Dispatch(op.Item1, op.Item2);
                }
            }

            (Instruction, long) Decode(string line)
            {
                var groups = line.Split(' ');
                switch (groups[0])
                {
                    case "noop": return (Instruction.NOOP, 0);
                    case "addx": return (Instruction.ADDX, long.Parse(groups[1]));
                }
                throw new Exception();
            }

            void Dispatch(Instruction instruction, long operand)
            {
                switch (instruction)
                {
                    case Instruction.NOOP:
                        Clock();
                        return;

                    case Instruction.ADDX:
                        Clock();
                        Clock();
                        x += operand;
                        return;
                }

                throw new Exception();
            }

            void Clock()
            {
                cycle++;
                OnClock(this);
            }
        }

        [Theory]
        [InlineData("Data/Day10_Test.txt", 13140)]
        [InlineData("Data/Day10.txt", 15680)]
        public void Part1(string filename, long expectedAnswer)
        {
            var prog = FileIterator.LoadLines<string>(filename);
            long sum = 0;
            long nextOutput = 20;

            VM vm = new()
            {
                OnClock = vm => {
                    if (vm.cycle == nextOutput)
                    {
                        sum += vm.cycle * vm.x;
                        nextOutput += 40;
                    }
                }
            };
            vm.Execute(prog);

            sum.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day10_Test.txt")]
        [InlineData("Data/Day10.txt")]
        public void Part2(string filename)
        {
            var prog = FileIterator.LoadLines<string>(filename);
            long y = 0;
            long x = 0;
            bool[,] display = new bool[40, 6];
            VM vm = new()
            {
                OnClock = vm => {
                    if (vm.x - 1 <= x && x <= vm.x + 1)
                    {
                        display[x, y] = true;
                    }
                    x++;

                    if (vm.cycle % 40 == 0)
                    {
                        y++;
                        x = 0;
                    }
                }
            };
            vm.Execute(prog);

            display.DebugDump(output.WriteLine, v => v ? '█' : ' ');
        }
    }
}
