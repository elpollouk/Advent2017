﻿using FluentAssertions;
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
        const int GP = 9;
        const int HALT = 99;

        const int MODE_POSITION = 0;
        const int MODE_IMMEDIATE = 1;
        const int MODE_RELATIVE = 2;

        public class VmMem
        {
            private readonly Dictionary<long, long> _Mem = new Dictionary<long, long>();
            public long this[long address]
            {
                get => _Mem.GetOrDefault(address, 0);
                set => _Mem[address] = value;
            }
        }

        public class VmState
        {
            public readonly VmMem Mem = new VmMem();
            public long IP = 0;
            public long GP = 0;
            public readonly int[] Modes = { MODE_POSITION, MODE_POSITION, MODE_POSITION };
            public Func<long> Input;
            public Action<long> Output;

            public readonly Queue<long> InputQueue = new Queue<long>();
            public readonly Queue<long> OutputQueue = new Queue<long>();

            public VmState(int[] mem)
            {
                for (var i = 0; i < mem.Length; i++)
                    Mem[i] = mem[i];
                Input = InputQueue.Dequeue;
                Output = OutputQueue.Enqueue;
            }

            public VmState(long[] mem)
            {
                for (var i = 0; i < mem.Length; i++)
                    Mem[i] = mem[i];
                Input = InputQueue.Dequeue;
                Output = OutputQueue.Enqueue;
            }

            public long Fetch(long value, long modeReg) => Modes[modeReg] switch
            {
                MODE_IMMEDIATE => value,
                MODE_POSITION => Mem[value],
                MODE_RELATIVE => Mem[GP + value],
                _ => throw new InvalidOperationException()
            };

            public long FetchOutput(long value, long modeReg) => Modes[modeReg] switch
            {
                MODE_POSITION => value,
                MODE_RELATIVE => GP + value,
                _ => throw new InvalidOperationException()
            };
        }

        class Program : IProgram<VmState, int, (long, long, long)>
        {
            public (int, (long, long, long)) Fetch(VmState vmState)
            {
                var mem = vmState.Mem;
                var ip = vmState.IP;
                var instruction = (int)mem[ip] % 100;
                var modes = mem[ip] / 100;

                vmState.Modes[0] = (int)modes % 10;
                vmState.Modes[1] = (int)(modes / 10) % 10;
                vmState.Modes[2] = (int)modes / 100;

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
                    case GP:
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

        private static readonly InstructionSet<VmState, int, (long a, long b, long c)> s_InstructionSet = new InstructionSet<VmState, int, (long a, long b, long c)>();
        private static readonly Program s_Program = new Program();

        static IntCode()
        {
            s_InstructionSet[ADD] = (vm, ops) => vm.Mem[vm.FetchOutput(ops.c, 2)] = vm.Fetch(ops.a, 0) + vm.Fetch(ops.b, 1);
            s_InstructionSet[MUL] = (vm, ops) => vm.Mem[vm.FetchOutput(ops.c, 2)] = vm.Fetch(ops.a, 0) * vm.Fetch(ops.b, 1);
            s_InstructionSet[IN] = (vm, ops) => vm.Mem[vm.FetchOutput(ops.a, 0)] = vm.Input();
            s_InstructionSet[OUT] = (vm, ops) => vm.Output(vm.Fetch(ops.a, 0));
            s_InstructionSet[JNZ] = (vm, ops) => { if (vm.Fetch(ops.a, 0) != 0) { vm.IP = vm.Fetch(ops.b, 1); } };
            s_InstructionSet[JZ] = (vm, ops) => { if (vm.Fetch(ops.a, 0) == 0) { vm.IP = vm.Fetch(ops.b, 1); } };
            s_InstructionSet[LT] = (vm, ops) => vm.Mem[vm.FetchOutput(ops.c, 2)] = vm.Fetch(ops.a, 0) < vm.Fetch(ops.b, 1) ? 1 : 0;
            s_InstructionSet[EQ] = (vm, ops) => vm.Mem[vm.FetchOutput(ops.c, 2)] = vm.Fetch(ops.a, 0) == vm.Fetch(ops.b, 1) ? 1 : 0;
            s_InstructionSet[GP] = (vm, ops) => vm.GP += vm.Fetch(ops.a, 0);
        }

        public static Executor<VmState, int, (long, long, long)> CreateVM(int[] mem)
        {
            var vmState = new VmState((int[])mem.Clone());

            return new Executor<VmState, int, (long, long, long)>(s_InstructionSet, s_Program, vmState);
        }

        public static Executor<VmState, int, (long, long, long)> CreateVM(long[] mem)
        {
            var vmState = new VmState((long[])mem.Clone());

            return new Executor<VmState, int, (long, long, long)>(s_InstructionSet, s_Program, vmState);
        }

        public static bool ExecuteUntilOutput(this Executor<VmState, int, (long, long, long)> executor, ref long output)
        {
            long _output = 0;
            bool hasOutput = false;

            executor.State.Output = o =>
            {
                _output = o;
                hasOutput = true;
            };

            executor.Execute(step => {
                while (!hasOutput)
                    step();
            });

            if (hasOutput)
                output = _output;
            return hasOutput;
        }

    }

    public class Day02
    {
        long Exec(int[] prog)
        {
            var vm = IntCode.CreateVM(prog);
            vm.Execute();

            return vm.State.Mem[0];
        }

        long ExecWithPatch(int[] prog, int noun, int verb)
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

            var baseValue = (int)ExecWithPatch(prog, 0, 0);
            // Calculate the deltas for verb and noun changes
            var dNoun = (int)ExecWithPatch(prog, 1, 0) - baseValue;
            var dVerb = (int)ExecWithPatch(prog, 0, 1) - baseValue;

            // Calculate the verb and noun values required to hit the target
            int noun = (target - baseValue) / dNoun;
            int verb = target - (baseValue + dNoun * noun);
            verb /= dVerb;

            // Verify we have correct verb/noun values and calculate the final answer
            ExecWithPatch(prog, noun, verb).Should().Be(target);
            (100 * noun + verb).Should().Be(answer);
        }
    }
}
