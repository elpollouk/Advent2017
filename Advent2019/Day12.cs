using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2019
{
    public class Day12
    {
        class Moon
        {
            public int x;
            public int y;
            public int z;
            public int dX = 0;
            public int dY = 0;
            public int dZ = 0;

            public int Energy => (Math.Abs(x) + Math.Abs(y) + Math.Abs(z)) * (Math.Abs(dX) + Math.Abs(dY) + Math.Abs(dZ));

            public Moon(int x, int y, int z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public void ApplyGravity(Moon other)
            {
                if (other == this) return;
                if (x < other.x) dX++;
                else if (x > other.x) dX--;
                if (y < other.y) dY++;
                else if (y > other.y) dY--;
                if (z < other.z) dZ++;
                else if (z > other.z) dZ--;
            }

            public void ApplyVelocity()
            {
                x += dX;
                y += dY;
                z += dZ;
            }
        }

        static Moon[] LoadMoons(string filename)
        {
            List<Moon> moons = new();

            foreach (var line in FileIterator.Lines(filename))
            {
                var groups = line.Groups(@"<x=([-\d]+), y=([-\d]+), z=([-\d]+)>");
                var moon = new Moon(int.Parse(groups[0]), int.Parse(groups[1]), int.Parse(groups[2]));
                moons.Add(moon);
            }

            return moons.ToArray();
        }

        static void SimStep(Moon[] moons)
        {
            foreach (var moon in moons)
                foreach (var other in moons)
                    moon.ApplyGravity(other);

            foreach (var moon in moons)
                moon.ApplyVelocity();
        }

        static long DetectPeriod(Moon[] moons, Func<Moon, (int, int)> pos)
        {
            var startPositions = moons.Select(pos).ToArray();

            bool HasCycled()
            {
                var currentPos = moons.Select(pos).ToArray();
                for (var i = 0; i < currentPos.Length; i++)
                    if (currentPos[i] != startPositions[i]) return false;

                return true;
            }

            var step = 0L;
            do
            {
                step++;
                SimStep(moons);
            }
            while (!HasCycled());

            return step;
        }

        [Theory]
        [InlineData("Data/Day12_Test.txt", 10, 179)]
        [InlineData("Data/Day12_Test2.txt", 100, 1940)]
        [InlineData("Data/Day12.txt", 1000, 10198)]
        public void Part1(string filename, int steps, int expectedAnswer)
        {
            var moons = LoadMoons(filename);

            while (steps --> 0)
                SimStep(moons);

            moons.Select(m => m.Energy).Sum().Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day12_Test.txt", 2772)]
        [InlineData("Data/Day12_Test2.txt", 4686774924)]
        [InlineData("Data/Day12.txt", 271442326847376)]
        public void Part2(string filename, long expectedAnswer)
        {
            var moons = LoadMoons(filename);
            var periodX = DetectPeriod(moons, m => (m.x, m.dX));
            var periodY = DetectPeriod(moons, m => (m.y, m.dY));
            var periodZ = DetectPeriod(moons, m => (m.z, m.dZ));

            var stepSize = periodX;
            var step = periodX;
            while (step % periodY != 0) step += stepSize;
            stepSize = step;
            while (step % periodZ != 0) step += stepSize;
            step.Should().Be(expectedAnswer);
        }
    }
}
