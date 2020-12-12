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
            public int x;
            public int y;
            public int dX;
            public int dY;

            private readonly Dictionary<ShipAction, Action<int>> actions = new();

            public Ship()
            {
                x = 0;
                y = 0;
                dX = 1;
                dY = 0;

                actions[ShipAction.North] = v => y += v;
                actions[ShipAction.South] = v => y -= v;
                actions[ShipAction.East] = v => x += v;
                actions[ShipAction.West] = v => x -= v;
                actions[ShipAction.Forward] = v =>
                {
                    x += dX * v;
                    y += dY * v;
                };
                actions[ShipAction.Left] = v =>
                {
                    while (v --> 0)
                        (dX, dY) = (-dY, dX);
                };
                actions[ShipAction.Right] = v =>
                {
                    while (v --> 0)
                        (dX, dY) = (dY, -dX);
                };
            }

            public void EnableAltActions()
            {
                dX = 10;
                dY = 1;

                actions[ShipAction.North] = v => dY += v;
                actions[ShipAction.South] = v => dY -= v;
                actions[ShipAction.East] = v => dX += v;
                actions[ShipAction.West] = v => dX -= v;
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
        [InlineData("Data/Day12_test.txt", 25)]
        [InlineData("Data/Day12.txt", 1457)]
        public void Problem1(string input, int expected)
        {
            var course = LoadCourse(input);
            var ship = new Ship();
            ship.Execute(course);
            (Math.Abs(ship.x) + Math.Abs(ship.y)).Should().Be(expected);
        }

        [Theory]
        [InlineData("Data/Day12_test.txt", 286)]
        [InlineData("Data/Day12.txt", 106860)]
        public void Problem2(string input, int expected)
        {
            var course = LoadCourse(input);
            var ship = new Ship();
            ship.EnableAltActions();
            ship.Execute(course);
            (Math.Abs(ship.x) + Math.Abs(ship.y)).Should().Be(expected);
        }
    }
}
