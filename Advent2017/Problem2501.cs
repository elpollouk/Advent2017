using Utils;
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
                Tape = new int[tapeSize];
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
                    if (Tape[i] == 1)
                        sum++;

                return sum;
            }

            public int Read() => Tape[TapeIndex];
            public int Write(int value) => Tape[TapeIndex] = value;
            public int Left(int count = 1) => TapeIndex -= count;
            public int Right(int count = 1) => TapeIndex += count;

            public Dictionary<State, Action<VM>> InstructionSet;
            public State State;
            public int[] Tape;
            public int TapeIndex;
        }

        [Fact]
        public void TestPart1()
        {
            var instructionSet = new Dictionary<State, Action<VM>>();
            instructionSet[State.A] = vm =>
            {
                var value = vm.Read();
                switch (value)
                {
                    case 0:
                        vm.Write(1);
                        vm.Right();
                        vm.State = State.B;
                        break;

                    case 1:
                        vm.Write(0);
                        vm.Left();
                        vm.State = State.B;
                        break;
                }
            };
            instructionSet[State.B] = vm =>
            {
                var value = vm.Read();
                switch (value)
                {
                    case 0:
                        vm.Write(1);
                        vm.Left();
                        vm.State = State.A;
                        break;

                    case 1:
                        vm.Right();
                        vm.State = State.A;
                        break;
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
                switch (vm.Read())
                {
                    case 0:
                        vm.Write(1);
                        vm.Right();
                        vm.State = State.B;
                        break;

                    case 1:
                        vm.Left();
                        vm.State = State.E;
                        break;
                }
            };
            instructionSet[State.B] = vm =>
            {
                switch (vm.Read())
                {
                    case 0:
                        vm.Write(1);
                        vm.Right();
                        vm.State = State.C;
                        break;
                    case 1:
                        vm.Right();
                        vm.State = State.F;
                        break;
                }
            };
            instructionSet[State.C] = vm =>
            {
                switch (vm.Read())
                {
                    case 0:
                        vm.Write(1);
                        vm.Left();
                        vm.State = State.D;
                        break;

                    case 1:
                        vm.Write(0);
                        vm.Right();
                        vm.State = State.B;
                        break;
                }
            };
            instructionSet[State.D] = vm =>
            {
                switch (vm.Read())
                {
                    case 0:
                        vm.Write(1);
                        vm.Right();
                        vm.State = State.E;
                        break;

                    case 1:
                        vm.Write(0);
                        vm.Left();
                        vm.State = State.C;
                        break;
                }
            };
            instructionSet[State.E] = vm =>
            {
                switch (vm.Read())
                {
                    case 0:
                        vm.Write(1);
                        vm.Left();
                        vm.State = State.A;
                        break;

                    case 1:
                        vm.Write(0);
                        vm.Right();
                        vm.State = State.D;
                        break;
                }
            };
            instructionSet[State.F] = vm =>
            {
                switch (vm.Read())
                {
                    case 0:
                        vm.Write(1);
                        vm.Right();
                        vm.State = State.A;
                        break;

                    case 1:
                        vm.Right();
                        vm.State = State.C;
                        break;
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
