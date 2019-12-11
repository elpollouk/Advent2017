using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2019
{
    public class Day11
    {
        class Robot
        {
            const int BLACK = 0;
            const int WHITE = 1;

            const int FACING_UP = 0;
            const int FACING_RIGHT = 1;
            const int FACING_DOWN = 2;
            const int FACING_LEFT = 3;

            enum State
            {
                PAINT,
                MOVE
            }

            public readonly Dictionary<(int, int), int> Hull = new Dictionary<(int, int), int>();
            private (int x, int y) _pos = (0, 0);
            private int _facing = FACING_UP;
            private State _state = State.PAINT;

            public long Camera()
            {
                return Hull.GetOrDefault(_pos, BLACK);
            }

            public void Act(long action)
            {
                switch (_state)
                {
                    case State.PAINT:
                        Hull[_pos] = (int)action;
                        _state = State.MOVE;
                        break;

                    case State.MOVE:
                        Rotate(action);
                        Move();
                        _state = State.PAINT;
                        break;

                    default:
                        throw new InvalidOperationException("Invalid robot state");
                }
            }

            void Rotate(long direction)
            {
                switch (direction)
                {
                    case 0:
                        _facing--;
                        if (_facing == -1) _facing = FACING_LEFT;
                        break;

                    case 1:
                        _facing++;
                        if (_facing == 4) _facing = FACING_UP;
                        break;

                    default:
                        throw new InvalidOperationException("Invalid direction");
                }
            }

            void Move()
            {
                _pos = _facing switch
                {
                    FACING_UP => (_pos.x, _pos.y - 1),
                    FACING_DOWN => (_pos.x, _pos.y + 1),
                    FACING_LEFT => (_pos.x - 1, _pos.y),
                    FACING_RIGHT => (_pos.x + 1, _pos.y),
                    _ => throw new InvalidOperationException("Invalid facing")
                };
            }
        }

        [Theory]
        //[InlineData("Data/Day10-example.txt", 8)]
        [InlineData("Data/Day11.txt", 2343)]
        public void Part1(string input, int answer)
        {
            var prog = FileIterator.LoadCSV<long>(input);
            var vm = IntCode.CreateVM(prog);
            var robot = new Robot();
            vm.State.Input = robot.Camera;
            vm.State.Output = robot.Act;

            vm.Execute();

            robot.Hull.Count.Should().Be(answer);
        }
    }
}
