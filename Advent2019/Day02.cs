using FluentAssertions;
using System;
using System.Collections.Generic;
using Utils;
using Utils.VM;
using Xunit;

namespace Advent2019
{
    public static class IntCode
    {
        const int ADD = 1;
        const int MUL = 2;
        const int JNZ = 5;
        const int JZ = 6;
        const int IN = 3;
        const int OUT = 4;
        const int LT = 7;
        const int EQ = 8;
        const int HALT = 99;

        const int MODE_POSITION = 0;
        const int MODE_IMMEDIATE = 1;

        public class VmState
        {
            public readonly int[] Mem;
            public int IP = 0;
            public readonly int[] Modes = { MODE_POSITION, MODE_POSITION };
            public Func<int> Input;
            public Action<int> Output;

            public readonly Queue<int> InputQueue = new Queue<int>();
            public readonly Queue<int> OutputQueue = new Queue<int>();

            public VmState(int[] mem)
            {
                Mem = mem;
                Input = InputQueue.Dequeue;
                Output = OutputQueue.Enqueue;
            }

            public int Fetch(int value, int modeReg) => Modes[modeReg] == MODE_IMMEDIATE ? value : Mem[value];
        }

        class Program : IProgram<VmState, int, (int, int, int)>
        {
            public (int, (int, int, int)) Fetch(VmState vmState)
            {
                var mem = vmState.Mem;
                var ip = vmState.IP;
                var instruction = mem[ip] % 100;
                var modes = mem[ip] / 100;

                switch (modes)
                {
                    case 0:
                        vmState.Modes[0] = MODE_POSITION;
                        vmState.Modes[1] = MODE_POSITION;
                        break;

                    case 1:
                        vmState.Modes[0] = MODE_IMMEDIATE;
                        vmState.Modes[1] = MODE_POSITION;
                        break;

                    case 10:
                        vmState.Modes[0] = MODE_POSITION;
                        vmState.Modes[1] = MODE_IMMEDIATE;
                        break;

                    case 11:
                        vmState.Modes[0] = MODE_IMMEDIATE;
                        vmState.Modes[1] = MODE_IMMEDIATE;
                        break;

                    default:
                        throw new InvalidOperationException("Invalid mode");
                }

                switch (instruction)
                {
                    case ADD:
                    case MUL:
                    case LT:
                    case EQ:
                        vmState.IP += 4;
                        return (instruction, (mem[ip + 1], mem[ip + 2], mem[ip + 3]));

                    case IN:
                    case OUT:
                        vmState.IP += 2;
                        return (instruction, (mem[ip + 1], 0, 0));

                    case JNZ:
                    case JZ:
                        vmState.IP += 3;
                        return (instruction, (mem[ip + 1], mem[ip + 2], 0));

                    case HALT:
                        throw new Halt();

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        private static readonly InstructionSet<VmState, int, (int a, int b, int c)> s_InstructionSet = new InstructionSet<VmState, int, (int a, int b, int c)>();
        private static readonly Program s_Program = new Program();

        static IntCode()
        {
            s_InstructionSet[ADD] = (vm, ops) => vm.Mem[ops.c] = vm.Fetch(ops.a, 0) + vm.Fetch(ops.b, 1);
            s_InstructionSet[MUL] = (vm, ops) => vm.Mem[ops.c] = vm.Fetch(ops.a, 0) * vm.Fetch(ops.b, 1);
            s_InstructionSet[IN] = (vm, ops) => vm.Mem[ops.a] = vm.Input();
            s_InstructionSet[OUT] = (vm, ops) => vm.Output(vm.Fetch(ops.a, 0));
            s_InstructionSet[JNZ] = (vm, ops) => { if (vm.Fetch(ops.a, 0) != 0) { vm.IP = vm.Fetch(ops.b, 1); } };
            s_InstructionSet[JZ] = (vm, ops) => { if (vm.Fetch(ops.a, 0) == 0) { vm.IP = vm.Fetch(ops.b, 1); } };
            s_InstructionSet[LT] = (vm, ops) => vm.Mem[ops.c] = vm.Fetch(ops.a, 0) < vm.Fetch(ops.b, 1) ? 1 : 0;
            s_InstructionSet[EQ] = (vm, ops) => vm.Mem[ops.c] = vm.Fetch(ops.a, 0) == vm.Fetch(ops.b, 1) ? 1 : 0;
        }

        public static Executor<VmState, int, (int, int, int)> CreateVM(int[] mem)
        {
            var vmState = new VmState((int[])mem.Clone());

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
