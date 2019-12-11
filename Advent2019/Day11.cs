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
        const int BLACK = 0;
        const int WHITE = 1;

        class Turtle
        {
            public const int FACING_UP = 0;
            public const int FACING_RIGHT = 1;
            public const int FACING_DOWN = 2;
            public const int FACING_LEFT = 3;
            public const int TURN_LEFT = 0;
            public const int TURN_RIGHT = 1;

            enum State
            {
                PAINT,
                MOVE
            }

            private readonly Dictionary<(int, int), int> _paper;
            private (int x, int y) _pos = (0, 0);
            private int _facing = FACING_UP;
            private State _state = State.PAINT;

            public Turtle(Dictionary<(int, int), int> paper)
            {
                _paper = paper;
            }

            public void TurnLeft()
            {
                _facing--;
                if (_facing == -1) _facing = FACING_LEFT;
            }

            public void TurnRight()
            {
                _facing++;
                if (_facing == 4) _facing = FACING_UP;
            }

            public void Turn(int direction)
            {
                switch (direction)
                {
                    case TURN_LEFT:
                        TurnLeft();
                        break;

                    case TURN_RIGHT:
                        TurnRight();
                        break;

                    default:
                        throw new InvalidOperationException("Invalid direction");
                }
            }

            public void Forward()
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

            public void Paint(int ink)
            {
                _paper[_pos] = ink;
            }

            public long Observe() => _paper.GetOrDefault(_pos, BLACK);

            public void Act(long action)
            {
                switch (_state)
                {
                    case State.PAINT:
                        Paint((int)action);
                        _state = State.MOVE;
                        break;

                    case State.MOVE:
                        Turn((int)action);
                        Forward();
                        _state = State.PAINT;
                        break;

                    default:
                        throw new InvalidOperationException("Invalid robot state");
                }
            }
        }

        [Theory]
        [InlineData("Data/Day11.txt", 2343)]
        public void Part1(string input, int answer)
        {
            var prog = FileIterator.LoadCSV<long>(input);
            var vm = IntCode.CreateVM(prog);
            var hull = new Dictionary<(int, int), int>();
            var robot = new Turtle(hull);
            vm.State.Input = robot.Observe;
            vm.State.Output = robot.Act;

            vm.Execute();

            hull.Count.Should().Be(answer);
        }

        [Theory]
        [InlineData("Data/Day11.txt", 249)]
        public void Part2(string input, int answer)
        {
            var prog = FileIterator.LoadCSV<long>(input);
            var vm = IntCode.CreateVM(prog);
            var hull = new Dictionary<(int, int), int>();
            hull[(0, 0)] = WHITE;
            var robot = new Turtle(hull);
            vm.State.Input = robot.Observe;
            vm.State.Output = robot.Act;

            vm.Execute();

            hull.Count.Should().Be(answer);

            var minX = hull.Keys.Select(k => k.Item1).Min();
            var maxX = hull.Keys.Select(k => k.Item1).Max();
            var minY = hull.Keys.Select(k => k.Item2).Min();
            var maxY = hull.Keys.Select(k => k.Item2).Max();

            var output = "";
            for (var y = minY; y <= maxY; y++)
            {
                for (var x = minX; x <= maxX; x++)
                {
                    output += hull.GetOrDefault((x, y), BLACK) == BLACK ? ' ' : '#';
                }
                output += '\n';
            }

            System.Diagnostics.Debug.Write(output);
        }
    }
}
