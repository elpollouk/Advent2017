using FluentAssertions;
using System.Linq;
using Xunit;
using Utils.VM;

namespace Advent2017
{
    public class Problem2501
    {
        enum Instruction
        {
            A, B, C, D, E, F
        }

        class VmState
        {
            public Instruction NextInstruction = Instruction.A;
            public readonly Tape<bool> Tape;

            public VmState(int tapeSize)
            {
                Tape = new Tape<bool>(tapeSize);
            }

            public int CheckSum() => Tape.Where(v => v).Count();
        }

        class Program : IProgram<Instruction, VmState>
        {
            public Instruction Fetch(VmState vmState) => vmState.NextInstruction;
        }

        [Fact]
        public void TestPart1()
        {
            var instructionSet = new InstructionSet<Instruction, VmState>();
            instructionSet[Instruction.A] = vm =>
            {
                if (vm.Tape.Read())
                {
                    vm.Tape.Write(false);
                    vm.Tape.Left();
                    vm.NextInstruction = Instruction.B;
                }
                else
                {
                    vm.Tape.Write(true);
                    vm.Tape.Right();
                    vm.NextInstruction = Instruction.B;
                }
            };
            instructionSet[Instruction.B] = vm =>
            {
                if (vm.Tape.Read())
                {
                    vm.Tape.Right();
                    vm.NextInstruction = Instruction.A;
                }
                else
                {
                    vm.Tape.Write(true);
                    vm.Tape.Left();
                    vm.NextInstruction = Instruction.A;
                }
            };

            var _vm = new Executor<Instruction, VmState>(instructionSet, new Program(), new VmState(4));
            _vm.Execute(6);
            _vm.State.CheckSum().Should().Be(3);
        }

        Executor<Instruction, VmState> CreateVm(int tapeSize)
        {
            var instructionSet = new InstructionSet<Instruction, VmState>();
            instructionSet[Instruction.A] = vm =>
            {
                if (vm.Tape.Read())
                {
                    vm.Tape.Left();
                    vm.NextInstruction = Instruction.E;
                }
                else
                {
                    vm.Tape.Write(true);
                    vm.Tape.Right();
                    vm.NextInstruction = Instruction.B;
                }
            };
            instructionSet[Instruction.B] = vm =>
            {
                if (vm.Tape.Read())
                {
                    vm.Tape.Right();
                    vm.NextInstruction = Instruction.F;
                }
                else
                {
                    vm.Tape.Write(true);
                    vm.Tape.Right();
                    vm.NextInstruction = Instruction.C;
                }
            };
            instructionSet[Instruction.C] = vm =>
            {
                if (vm.Tape.Read())
                {
                    vm.Tape.Write(false);
                    vm.Tape.Right();
                    vm.NextInstruction = Instruction.B;
                }
                else
                {
                    vm.Tape.Write(true);
                    vm.Tape.Left();
                    vm.NextInstruction = Instruction.D;
                }
            };
            instructionSet[Instruction.D] = vm =>
            {
                if (vm.Tape.Read())
                {
                    vm.Tape.Write(false);
                    vm.Tape.Left();
                    vm.NextInstruction = Instruction.C;
                }
                else
                {
                    vm.Tape.Write(true);
                    vm.Tape.Right();
                    vm.NextInstruction = Instruction.E;
                }
            };
            instructionSet[Instruction.E] = vm =>
            {
                if (vm.Tape.Read())
                {
                    vm.Tape.Write(false);
                    vm.Tape.Right();
                    vm.NextInstruction = Instruction.D;
                }
                else
                {
                    vm.Tape.Write(true);
                    vm.Tape.Left();
                    vm.NextInstruction = Instruction.A;
                }
            };
            instructionSet[Instruction.F] = vm =>
            {
                if (vm.Tape.Read())
                {
                    vm.Tape.Right();
                    vm.NextInstruction = Instruction.C;
                }
                else
                {
                    vm.Tape.Write(true);
                    vm.Tape.Right();
                    vm.NextInstruction = Instruction.A;
                }
            };

            return new Executor<Instruction, VmState>(instructionSet, new Program(), new VmState(tapeSize));
        }

        [Fact]
        public void Part1()
        {
            var vm = CreateVm(17*1024);
            vm.Execute(12523873);
            vm.State.CheckSum().Should().Be(4225);
        }
    }
}
