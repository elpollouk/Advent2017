using FluentAssertions;
using Utils;
using Utils.VM;
using Xunit;

namespace Advent2019
{
    public class Day02
    {
        class VmState
        {
            public int[] Mem;
            public int IP = 0;
        }

        class Program : IProgram<VmState, int, (int a, int b, int c)>
        {
            public (int, (int a, int b, int c)) Fetch(VmState vmState)
            {
                var mem = vmState.Mem;
                var ip = vmState.IP;
                var instruction = mem[ip];
                if (instruction == 99) throw new Halt();
                vmState.IP += 4;

                return (mem[ip], (mem[ip + 1], mem[ip + 2], mem[ip + 3]));
            }
        }

        int Exec(int[] prog)
        {
            var instructionSet = new InstructionSet<VmState, int, (int a, int b, int c)>();
            instructionSet[1] = (vm, ops) => vm.Mem[ops.c] = vm.Mem[ops.a] + vm.Mem[ops.b];
            instructionSet[2] = (vm, ops) => vm.Mem[ops.c] = vm.Mem[ops.a] * vm.Mem[ops.b];

            var program = new Program();
            var vmState = new VmState();
            vmState.Mem = prog;

            var vm = new Executor<VmState, int, (int a, int b, int c)>(instructionSet, program, vmState);
            vm.Execute();

            return vm.State.Mem[0];
        }

        [Theory]
        [InlineData("Data/Day02-example.txt", 3500)]
        [InlineData("Data/Day02-example2.txt", 30)]
        [InlineData("Data/Day02.txt", 3654868)]
        void Problem1(string input, int answer)
        {
            var prog = FileIterator.LoadCSV<int>(input);
            // Patch as per problem instructions
            if (input.EndsWith("/Day02.txt"))
            {
                prog[1] = 12;
                prog[2] = 2;
            }

            Exec(prog).Should().Be(answer);
        }

        [Theory]
        [InlineData("Data/Day02.txt", 70, 14, 19690720)]
        void Problem2(string input, int noun, int verb, int target)
        {
            var prog = FileIterator.LoadCSV<int>(input);
            prog[1] = noun;
            prog[2] = verb;

            Exec(prog).Should().Be(target);
        }
    }
}
