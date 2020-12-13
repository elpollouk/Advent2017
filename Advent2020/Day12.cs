using FluentAssertions;
using System;
using System.Collections.Generic;
using Utils;
using Xunit;

namespace Advent2020
{
    public class Day12
    {
        enum ShipAction
        {
            North,
            South,
            East,
            West,
            Forward,
            Left,
            Right,
        }

        class Ship
        {
            public (int x, int y) pos = (0, 0);
            public (int x, int y) facing;

            private readonly Dictionary<ShipAction, Action<int>> actions = new();

            public Ship(bool part2)
            {
                if (part2)
                {
                    facing = (10, 1);
                    actions[ShipAction.North] = v => facing = (facing.x, facing.y + v);
                    actions[ShipAction.South] = v => facing = (facing.x, facing.y - v);
                    actions[ShipAction.East] = v => facing = (facing.x + v, facing.y);
                    actions[ShipAction.West] = v => facing = (facing.x - v, facing.y);
                }
                else
                {
                    facing = (1, 0);
                    actions[ShipAction.North] = v => pos = (pos.x, pos.y + v);
                    actions[ShipAction.South] = v => pos = (pos.x, pos.y - v);
                    actions[ShipAction.East] = v => pos = (pos.x + v, pos.y);
                    actions[ShipAction.West] = v => pos = (pos.x - v, pos.y);
                }

                actions[ShipAction.Forward] = v =>
                {
                    pos = (
                        pos.x + facing.x * v,
                        pos.y + facing.y * v
                    );
                };
                actions[ShipAction.Left] = v =>
                {
                    while (v-- > 0)
                        facing = facing.RotateLeft();
                };
                actions[ShipAction.Right] = v =>
                {
                    while (v-- > 0)
                        facing = facing.RotateRight();
                };
            }

            public void Act(ShipAction action, int value) => actions[action](value);

            public void Execute(IEnumerable<(ShipAction, int)> course)
            {
                foreach (var (action, value) in course)
                    Act(action, value);
            }
        }

        static IEnumerable<(ShipAction, int)> LoadCourse(string input)
        {
            var course = new List<(ShipAction, int)>();

            foreach (var line in FileIterator.Lines(input))
            {
                var action = line[0] switch {
                    'N' => ShipAction.North,
                    'S' => ShipAction.South,
                    'E' => ShipAction.East,
                    'W' => ShipAction.West,
                    'F' => ShipAction.Forward,
                    'L' => ShipAction.Left,
                    'R' => ShipAction.Right,
                    _ => throw new InvalidOperationException($"Invalid action '{line[0]}'")
                };

                var value = int.Parse(line[1..]);

                if (action is ShipAction.Left or ShipAction.Right)
                {
                    value = value switch {
                        0 => 0,
                        90 => 1,
                        180 => 2,
                        270 => 3,
                        _ => throw new InvalidOperationException($"Invalid turn angle {value}")
                    };
                }

                course.Add((action, value));
            }

            return course;
        }

        [Theory]
        [InlineData("Data/Day12_test.txt", false, 25)]
        [InlineData("Data/Day12.txt", false, 1457)]
        [InlineData("Data/Day12_test.txt", true, 286)]
        [InlineData("Data/Day12.txt", true, 106860)]
        public void Problem1(string input, bool part2, int expected)
        {
            var course = LoadCourse(input);
            var ship = new Ship(part2);
            ship.Execute(course);
            (Math.Abs(ship.pos.x) + Math.Abs(ship.pos.y)).Should().Be(expected);
        }
    }
}
