using FluentAssertions;
using Utils;
using Utils.VM;
using Xunit;

namespace Advent2019
{
    public static class IntCode
    {
        public class VmState
        {
            public readonly int[] Mem;
            public int IP = 0;

            public VmState(int[] mem)
            {
                Mem = mem;
            }
        }

        class Program : IProgram<VmState, int, (int, int, int)>
        {
            public (int, (int, int, int)) Fetch(VmState vmState)
            {
                var mem = vmState.Mem;
                var ip = vmState.IP;
                var instruction = mem[ip];

                if (instruction == 99) throw new Halt();
                vmState.IP += 4;

                return (mem[ip], (mem[ip + 1], mem[ip + 2], mem[ip + 3]));
            }
        }

        private static readonly InstructionSet<VmState, int, (int a, int b, int c)> s_InstructionSet = new InstructionSet<VmState, int, (int a, int b, int c)>();
        private static readonly Program s_Program = new Program();

        static IntCode()
        {
            s_InstructionSet[1] = (vm, ops) => vm.Mem[ops.c] = vm.Mem[ops.a] + vm.Mem[ops.b];
            s_InstructionSet[2] = (vm, ops) => vm.Mem[ops.c] = vm.Mem[ops.a] * vm.Mem[ops.b];
        }

        public static Executor<VmState, int, (int, int, int)> CreateVM(int[] mem)
        {
            var vmState = new VmState(mem);

            return new Executor<VmState, int, (int, int, int)>(s_InstructionSet, s_Program, vmState);
        }
    }

    public class Day02
    {
        int Exec(int[] prog)
        {
            var vm = IntCode.CreateVM(prog);
            vm.Execute();

            return vm.State.Mem[0];
        }

        int ExecWithPatch(int[] prog, int noun, int verb)
        {
            prog = (int[])prog.Clone();
            prog[1] = noun;
            prog[2] = verb;
            return Exec(prog);
        }

        [Theory]
        [InlineData("Data/Day02-example.txt", 9, 10, 3500)]
        [InlineData("Data/Day02-example2.txt", 1, 1, 30)]
        [InlineData("Data/Day02.txt", 12, 2, 3654868)]
        public void Problem1(string input, int verb, int noun, int answer)
        {
            var prog = FileIterator.LoadCSV<int>(input);
            ExecWithPatch(prog, verb, noun).Should().Be(answer);
        }

        [Theory]
        [InlineData("Data/Day02.txt", 19690720, 7014)]
        [InlineData("Data/Day02.txt", 3654868, 1202)]
        public void Problem2(string input, int target, int answer)
        {
            var prog = FileIterator.LoadCSV<int>(input);

            var baseValue = ExecWithPatch(prog, 0, 0);
            // Calculate the deltas for verb and noun changes
            var dNoun = ExecWithPatch(prog, 1, 0) - baseValue;
            var dVerb = ExecWithPatch(prog, 0, 1) - baseValue;

            // Calculate the verb and noun values required to hit the target
            var noun = (target - baseValue) / dNoun;
            var verb = target - (baseValue + dNoun * noun);
            verb /= dVerb;

            // Verify we have correct verb/noun values and calculate the final answer
            ExecWithPatch(prog, noun, verb).Should().Be(target);
            (100 * noun + verb).Should().Be(answer);
        }
    }
}
