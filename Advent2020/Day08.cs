using FluentAssertions;
using System.Collections.Generic;
using Utils;
using Xunit;
using Xunit.Abstractions;

namespace Advent2020
{
    public class Day08
    {
        enum OpCode
        {
            NOP,
            ACC,
            JMP
        }

        struct Instruction
        {
            readonly public OpCode OpCode;
            readonly public long Value;

            public Instruction(OpCode opCode, long value)
            {
                OpCode = opCode;
                Value = value;
            }
        }

        class VmState
        {
            public long Acc = 0;
            public long Ip = 0;
        }

        readonly ITestOutputHelper Output;
        long OpCodeCount = 0;

        public Day08(ITestOutputHelper output)
        {
            Output = output;
        }

        static Instruction[] LoadProgram(string input)
        {
            var program = new List<Instruction>();
            
            foreach (var line in FileIterator.Lines(input))
            {
                var split = line.Split(' ');
                OpCode opCode = split[0] switch {
                    "acc" => OpCode.ACC,
                    "jmp" => OpCode.JMP,
                    _ => OpCode.NOP
                };

                var value = long.Parse(split[1]);
                program.Add(new Instruction(opCode, value));
            }

            return program.ToArray();
        }

        void ExecuteUntilLoop(VmState vmState, Instruction[] program)
        {
            var visited = new HashSet<long>();

            while (true)
            {
                if (vmState.Ip >= program.Length) return;
                if (visited.Contains(vmState.Ip)) throw new Utils.VM.Halt();
                visited.Add(vmState.Ip);
                var instruction = program[vmState.Ip];

                OpCodeCount++;

                switch (instruction.OpCode)
                {
                    case OpCode.NOP:
                        break;

                    case OpCode.ACC:
                        vmState.Acc += instruction.Value;
                        break;

                    case OpCode.JMP:
                        vmState.Ip += (instruction.Value - 1); // We need to take into account that the IP is auto incremented
                        break;
                }

                vmState.Ip++;
            }
        }

        long PatchAndExecute(Instruction[] program)
        {
            for (var i = 0; i < program.Length; i++)
            {
                var patchedInstruction = program[i];
                switch (patchedInstruction.OpCode)
                {
                    case OpCode.NOP:
                        program[i] = new Instruction(OpCode.JMP, patchedInstruction.Value);
                        break;

                    case OpCode.JMP:
                        program[i] = new Instruction(OpCode.NOP, patchedInstruction.Value);
                        break;

                    default:
                        continue;
                }

                try
                {
                    var vmState = new VmState();
                    ExecuteUntilLoop(vmState, program);
                    return vmState.Acc;
                }
                catch (Utils.VM.Halt) {} // Loop detected, so continue the search...

                // Restore the program
                program[i] = patchedInstruction;
            }
            throw new Expletive("Bugger");
        }

        long ExecutionGraph(Instruction[] program, HashSet<long> visited, long ip, long acc, bool branching)
        {
            if (ip >= program.Length) return acc;
            if (visited.Contains(ip)) throw new Utils.VM.Halt();
            visited.Add(ip);
            var instuction = program[ip];
            try
            {
                OpCodeCount++;
                switch (instuction.OpCode)
                {
                    case OpCode.NOP:
                        try
                        {
                            return ExecutionGraph(program, visited, ip + 1, acc, branching);
                        }
                        catch (Utils.VM.Halt)
                        {
                            if (!branching)
                            {
                                OpCodeCount++;
                                return ExecutionGraph(program, visited, ip + instuction.Value, acc, true);
                            }
                            else
                                throw;
                        }

                    case OpCode.JMP:
                        try
                        {
                            return ExecutionGraph(program, visited, ip + instuction.Value, acc, branching);
                        }
                        catch (Utils.VM.Halt)
                        {
                            if (!branching)
                            {
                                OpCodeCount++;
                                return ExecutionGraph(program, visited, ip + 1, acc, true);
                            }
                            else
                                throw;
                        }

                    case OpCode.ACC:
                        return ExecutionGraph(program, visited, ip + 1, acc + instuction.Value, branching);

                    default:
                        throw new Expletive("Shit");
                }
            }
            finally
            {
                visited.Remove(ip);
            }
        }

        [Theory]
        [InlineData("Data/Day08_test.txt", 5)]
        [InlineData("Data/Day08.txt", 1420)]
        public void Problem1(string input, long expected)
        {
            var vmState = new VmState();
            var prog = LoadProgram(input);

            try
            {
                ExecuteUntilLoop(vmState, prog);
                Oh.Bugger();
            }
            catch (Utils.VM.Halt)
            {
                vmState.Acc.Should().Be(expected);
            }
        }

        [Theory]
        [InlineData("Data/Day08_test.txt", 8)]
        [InlineData("Data/Day08.txt", 1245)]
        public void Problem2(string input, int expected)
        {
            var program = LoadProgram(input);
            PatchAndExecute(program).Should().Be(expected);

            Output.WriteLine($"Op Code Count = {OpCodeCount}");
        }

        [Theory]
        [InlineData("Data/Day08_test.txt", 8)]
        [InlineData("Data/Day08.txt", 1245)]
        public void Problem2_Smart(string input, int expected)
        {
            var visited = new HashSet<long>();
            var program = LoadProgram(input);
            ExecutionGraph(program, visited, 0, 0, false).Should().Be(expected);

            Output.WriteLine($"Op Code Count = {OpCodeCount}");
        }
    }
}
