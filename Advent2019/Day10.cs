using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2019
{
    public class Day10
    {
        Dictionary<(int, int), int> LoadAsteroids(string input)
        {
            var x = 0;
            var y = 0;
            var asteroids = new Dictionary<(int, int), int>();
            foreach (var line in FileIterator.Lines(input))
            {
                foreach (var c in line)
                {
                    if (c == '#')
                    {
                        asteroids[(x, y)] = 0;
                    }
                    x++;
                }
                y++;
                x = 0;
            }

            return asteroids;
        }

        static double Atan2Fudge(double y, double x)
        {
            var angle = Math.Atan2(y, x);
            if (angle < -Math.PI/2.0) angle += 2 * Math.PI;

            angle += Math.PI / 2;
            return angle;
        }

        void CalcVisibility(Dictionary<(int, int), int> asteroids)
        {
            foreach (var (x, y) in asteroids.Keys.ToList())
            {
                var angles = new HashSet<double>();
                foreach (var (x2, y2) in asteroids.Keys)
                {
                    if (x == x2 && y == y2) continue;
                    var dX = x2 - x;
                    var dY = y2 - y;
                    angles.Add(Atan2Fudge(dY, dX));
                }
                asteroids[(x, y)] = angles.Count();
            }
        }

        List<(int, int)> BuildVisibilityList((int x, int y) asterroid, Dictionary<(int, int), int> asteroids)
        {
            var otherAsteroids = new Dictionary<double, List<(int, int, int)>>();

            foreach (var (x2, y2) in asteroids.Keys)
            {
                if (asterroid.x == x2 &&  asterroid.y == y2) continue;
                var dX = x2 - asterroid.x;
                var dY = y2 - asterroid.y;
                otherAsteroids.GetOrCreate(Atan2Fudge(dY, dX), () => new List<(int, int, int)>()).Add((x2, y2, dX * dX + dY * dY));
            }

            foreach (var details in otherAsteroids.Values)
                details.Sort((a, b) => a.Item3 - b.Item3);

            return otherAsteroids.Keys
                .OrderBy(k => k)
                .Select(k => otherAsteroids[k].First())
                .Select(details => (details.Item1, details.Item2))
                .ToList();
        }

        [Theory]
        [InlineData(1, -1, -(Math.PI/4))]
        [InlineData(-4, -4, -(3*Math.PI / 4))]
        public void TestAtan2(int x, int y, double angle) => Math.Atan2(y, x).Should().Be(angle);

        [Theory]
        [InlineData(0, -1, 0)]
        [InlineData(1, -1, 1*Math.PI / 4)]
        [InlineData(1, 0, 2*Math.PI / 4)]
        [InlineData(1, 1, 3*Math.PI / 4)]
        [InlineData(0, 1, 4*Math.PI / 4)]
        [InlineData(-1, 1, 5*Math.PI / 4)]
        [InlineData(-1, 0, 6*Math.PI / 4)]
        [InlineData(-1, -1, 7*Math.PI / 4)]
        public void TestAtan2Fudge(int x, int y, double angle) => Atan2Fudge(y, x).Should().Be(angle);

        [Theory]
        [InlineData(5, 5, 3, 3)]
        [InlineData(1, 2, 2, 4)]
        [InlineData(-3, 7, -33, 77)]
        public void Atan2Equal(int x1, int y1, int x2, int y2) => Math.Atan2(y1, x1).Should().Be(Math.Atan2(y2, x2));

        [Theory]
        [InlineData("Data/Day10-example.txt", 8)]
        [InlineData("Data/Day10.txt", 221)]
        public void Part1(string input, int answer)
        {
            var asteroids = LoadAsteroids(input);
            CalcVisibility(asteroids);
            var maxVis = asteroids.Values.Max();
            maxVis.Should().Be(answer);

            var maxAsteroids = asteroids.Keys.Where(k => asteroids[k] == maxVis);
            maxAsteroids.Count().Should().Be(1);
            var maxAsteroid = maxAsteroids.First();
            var visibility = BuildVisibilityList(maxAsteroid, asteroids);

            if (visibility.Count() >= 200)
            {
                var (x, y) = visibility[199];
                (x * 100 + y).Should().Be(806);
            }
        }
    }
}
