using FluentAssertions;
using Xunit;
using System.Collections.Generic;
using System;

namespace Advent2017
{
    public class Problem2501
    {
        enum State
        {
            A, B, C, D, E, F
        }

        class VM
        {
            public VM(Dictionary<State, Action<VM>> instructionSet, int tapeSize, State initialState)
            {
                InstructionSet = instructionSet;
                Tape = new bool[tapeSize];
                TapeIndex = tapeSize / 2;
                State = initialState;
            }

            public void Execute(int numCycles)
            {
                for (var i = 0; i < numCycles; i++)
                    InstructionSet[State](this);
            }

            public int CheckSum()
            {
                var sum = 0;
                for (var i = 0; i < Tape.Length; i++)
                    if (Tape[i])
                        sum++;

                return sum;
            }

            public bool Read() => Tape[TapeIndex];
            public void Write(bool value) => Tape[TapeIndex] = value;
            public void Left(int count = 1) => TapeIndex -= count;
            public void Right(int count = 1) => TapeIndex += count;

            public Dictionary<State, Action<VM>> InstructionSet;
            public State State;
            public bool[] Tape;
            public int TapeIndex;
        }

        [Fact]
        public void TestPart1()
        {
            var instructionSet = new Dictionary<State, Action<VM>>();
            instructionSet[State.A] = vm =>
            {
                if (vm.Read())
                {
                    vm.Write(false);
                    vm.Left();
                    vm.State = State.B;
                }
                else
                {
                    vm.Write(true);
                    vm.Right();
                    vm.State = State.B;
                }
            };
            instructionSet[State.B] = vm =>
            {
                if (vm.Read())
                {
                    vm.Right();
                    vm.State = State.A;
                }
                else
                {
                    vm.Write(true);
                    vm.Left();
                    vm.State = State.A;
                }
            };

            var _vm = new VM(instructionSet, 4, State.A);
            _vm.Execute(6);
            _vm.CheckSum().Should().Be(3);
        }

        VM CreateVm(int tapeSize)
        {
            var instructionSet = new Dictionary<State, Action<VM>>();
            instructionSet[State.A] = vm =>
            {
                if (vm.Read())
                {
                    vm.Left();
                    vm.State = State.E;
                }
                else
                {
                    vm.Write(true);
                    vm.Right();
                    vm.State = State.B;
                }
            };
            instructionSet[State.B] = vm =>
            {
                if (vm.Read())
                {
                    vm.Right();
                    vm.State = State.F;
                }
                else
                {
                    vm.Write(true);
                    vm.Right();
                    vm.State = State.C;
                }
            };
            instructionSet[State.C] = vm =>
            {
                if (vm.Read())
                {
                    vm.Write(false);
                    vm.Right();
                    vm.State = State.B;
                }
                else
                {
                    vm.Write(true);
                    vm.Left();
                    vm.State = State.D;
                }
            };
            instructionSet[State.D] = vm =>
            {
                if (vm.Read())
                {
                    vm.Write(false);
                    vm.Left();
                    vm.State = State.C;
                }
                else
                {
                    vm.Write(true);
                    vm.Right();
                    vm.State = State.E;
                }
            };
            instructionSet[State.E] = vm =>
            {
                if (vm.Read())
                {
                    vm.Write(false);
                    vm.Right();
                    vm.State = State.D;
                }
                else
                {
                    vm.Write(true);
                    vm.Left();
                    vm.State = State.A;
                }
            };
            instructionSet[State.F] = vm =>
            {
                if (vm.Read())
                {
                    vm.Right();
                    vm.State = State.C;
                }
                else
                {
                    vm.Write(true);
                    vm.Right();
                    vm.State = State.A;
                }
            };

            return new VM(instructionSet, tapeSize, State.A);
        }

        [Fact]
        public void Part1()
        {
            var vm = CreateVm(17*1024);
            vm.Execute(12523873);
            vm.CheckSum().Should().Be(4225);
        }
    }
}
