using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2021
{
    public class Day22
    {
        class Cube
        {
            static readonly Cube ZERO_CUBE = new Cube(false, (0, 0, 0), (0, 0, 0));

            public bool State { get; set; }

            public (int x, int y, int z) From { get; private set; }
            public (int x, int y, int z) To { get; private set; }

            public (int x, int y, int z) Shape { get; private set; }

            public int Volume => Shape.x * Shape.y * Shape.z;

            public override string ToString()
            {
                return $"{From}-{To}";
            }

            public Cube(bool state, (int x, int y, int z) from, (int x, int y, int z) to)
            {
                State = state;
                From = from;
                To = to;
                UpdateShape();
            }

            void UpdateShape()
            {
                Shape = (
                    Math.Abs(To.x - From.x) + 1,
                    Math.Abs(To.y - From.y) + 1,
                    Math.Abs(To.z - From.z) + 1
                );
            }

            public bool Within((int x, int y, int z) point)
            {
                return From.x <= point.x && point.x <= To.x
                    && From.y <= point.y && point.y <= To.y
                    && From.z <= point.z && point.z <= To.z;
            }

            public bool Overlaps(Cube other)
            {
                return (From.x <= other.To.x && To.x >= other.From.x)
                    && (From.y <= other.To.y && To.y >= other.From.y)
                    && (From.z <= other.To.z && To.z >= other.From.z);
            }

            public bool Contains(Cube other)
            {
                return (From.x <= other.From.x && other.To.x <= To.x)
                    && (From.y <= other.From.y && other.To.y <= To.y)
                    && (From.z <= other.From.z && other.To.z <= To.z);
            }

            public Cube Intersect(Cube other)
            {
                if (!Overlaps(other)) return ZERO_CUBE;

                var FromX = Math.Max(From.x, other.From.x);
                var ToX = Math.Min(To.x, other.To.x);
                var FromY = Math.Max(From.y, other.From.y);
                var ToY = Math.Min(To.x, other.To.x);
                var FromZ = Math.Max(From.z, other.From.z);
                var ToZ = Math.Min(To.z, other.To.z);

                return new Cube(other.State, (FromX, FromY, FromZ), (ToX, ToY, ToZ));
            }

            public IEnumerable<Cube> SliceX(int x)
            {
                if (x <= From.x || To.x < x)
                {
                    yield return this;
                }
                else
                {
                    yield return new Cube(
                        State,
                        From,
                        (x - 1,To.y, To.z)
                    );
                    yield return new Cube(
                        State,
                        (x, From.y, From.z),
                        To
                    );
                }
            }

            public IEnumerable<Cube> SliceY(int y)
            {
                if (y <= From.y || To.y < y)
                {
                    yield return this;
                }
                else
                {
                    yield return new Cube(
                        State,
                        From,
                        (To.x, y - 1, To.z)
                    );
                    yield return new Cube(
                        State,
                        (From.x, y, From.z),
                        To
                    );
                }
            }

            public IEnumerable<Cube> SliceZ(int z)
            {
                if (z <= From.z || To.z < z)
                {
                    yield return this;
                }
                else
                {
                    yield return new Cube(
                        State,
                        From,
                        (To.x, To.y, z - 1)
                    );
                    yield return new Cube(
                        State,
                        (From.x, From.y, z),
                        To
                    );
                }
            }

            public IEnumerable<Cube> SliceXYZ((int x, int y, int z) v)
            {
                foreach (var c1 in SliceX(v.x))
                {
                    foreach (var c2 in c1.SliceY(v.y))
                    {
                        foreach (var c3 in c2.SliceZ(v.z))
                        {
                            yield return c3;
                        }
                    }
                }
            }

            public IEnumerable<Cube> SliceCube(Cube other)
            {
                var slice1 = other.From;
                var slice2 = (other.To.x + 1, other.To.y + 1, other.To.z + 1);

                foreach (var c1 in SliceXYZ(slice1))
                {
                    foreach (var c2 in c1.SliceXYZ(slice2))
                    {
                        yield return c2;
                    }
                }
            }
        }

        [Theory]
        [InlineData(10, 10, 10, 12, 12, 12, 9, 9, 9, 11, 11, 11, true)]
        [InlineData(10, 10, 10, 12, 12, 12, 7, 10, 10, 9, 12, 12, false)]
        [InlineData(10, 10, 10, 12, 12, 12, 7, 7, 7, 15, 15, 15, true)]
        public void OverlapTest(int fromX1, int fromY1, int fromZ1, int toX1, int toY1, int toZ1, int fromX2, int fromY2, int fromZ2, int toX2, int toY2, int toZ2, bool expectedResult)
        {
            Cube c1 = new(true, (fromX1, fromY1, fromZ1), (toX1, toY1, toZ1));
            Cube c2 = new(true, (fromX2, fromY2, fromZ2), (toX2, toY2, toZ2));
            c1.Overlaps(c2).Should().Be(expectedResult);
            c2.Overlaps(c1).Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(10, 10, 10, 12, 12, 12, 9, 9, 9, 11, 11, 11, false)]
        [InlineData(10, 10, 10, 12, 12, 12, 7, 10, 10, 9, 12, 12, false)]
        [InlineData(10, 10, 10, 12, 12, 12, 10, 10, 10, 12, 12, 12, true)]
        [InlineData(9, 9, 9, 13, 13, 13, 10, 10, 10, 12, 12, 12, true)]
        public void ContainsTest(int fromX1, int fromY1, int fromZ1, int toX1, int toY1, int toZ1, int fromX2, int fromY2, int fromZ2, int toX2, int toY2, int toZ2, bool expectedResult)
        {
            Cube c1 = new(true, (fromX1, fromY1, fromZ1), (toX1, toY1, toZ1));
            Cube c2 = new(true, (fromX2, fromY2, fromZ2), (toX2, toY2, toZ2));
            c1.Contains(c2).Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(10, 10, 10, 12, 12, 12, 9, 9, 9, 11, 11, 11, 10, 10, 10, 11, 11, 11)]
        [InlineData(10, 10, 10, 12, 12, 12, 7, 10, 10, 9, 12, 12, 0, 0, 0, 0, 0, 0)]
        [InlineData(10, 10, 10, 12, 12, 12, 7, 7, 7, 15, 15, 15, 10, 10, 10, 12, 12, 12)]
        public void IntersectTest(int fromX1, int fromY1, int fromZ1, int toX1, int toY1, int toZ1, int fromX2, int fromY2, int fromZ2, int toX2, int toY2, int toZ2, int fromX3, int fromY3, int fromZ3, int toX3, int toY3, int toZ3)
        {
            Cube c1 = new(true, (fromX1, fromY1, fromZ1), (toX1, toY1, toZ1));
            Cube c2 = new(true, (fromX2, fromY2, fromZ2), (toX2, toY2, toZ2));

            Cube c3 = c1.Intersect(c2);
            c3.From.Should().Be((fromX3, fromY3, fromZ3));
            c3.To.Should().Be((toX3, toY3, toZ3));

            c3 = c2.Intersect(c1);
            c3.From.Should().Be((fromX3, fromY3, fromZ3));
            c3.To.Should().Be((toX3, toY3, toZ3));
        }

        [Fact]
        public void SliceXTest()
        {
            // Case 1 - X too small
            var cube = new Cube(true, (10, 10, 10), (12, 12, 12));
            var cubes = cube.SliceX(10).ToArray();
            cubes.Length.Should().Be(1);
            cubes[0].Should().Be(cube);

            // Case 2 - X is middle of range
            cubes = cube.SliceX(11).ToArray();
            cubes.Length.Should().Be(2);
            cubes[0].From.Should().Be((10, 10, 10));
            cubes[0].To.Should().Be((10, 12, 12));
            cubes[1].From.Should().Be((11, 10, 10));
            cubes[1].To.Should().Be((12, 12, 12));

            // Case 3 - X is last element
            cubes = cube.SliceX(12).ToArray();
            cubes.Length.Should().Be(2);
            cubes[0].From.Should().Be((10, 10, 10));
            cubes[0].To.Should().Be((11, 12, 12));
            cubes[1].From.Should().Be((12, 10, 10));
            cubes[1].To.Should().Be((12, 12, 12));

            // Case 4 - X too large
            cubes = cube.SliceX(13).ToArray();
            cubes.Length.Should().Be(1);
            cubes[0].Should().Be(cube);
        }

        [Fact]
        public void SliceYTest()
        {
            // Case 1 - Y too small
            var cube = new Cube(true, (10, 10, 10), (12, 12, 12));
            var cubes = cube.SliceY(10).ToArray();
            cubes.Length.Should().Be(1);
            cubes[0].Should().Be(cube);

            // Case 2 - Y is middle of range
            cubes = cube.SliceY(11).ToArray();
            cubes.Length.Should().Be(2);
            cubes[0].From.Should().Be((10, 10, 10));
            cubes[0].To.Should().Be((12, 10, 12));
            cubes[1].From.Should().Be((10, 11, 10));
            cubes[1].To.Should().Be((12, 12, 12));

            // Case 3 - Y is last element
            cubes = cube.SliceY(12).ToArray();
            cubes.Length.Should().Be(2);
            cubes[0].From.Should().Be((10, 10, 10));
            cubes[0].To.Should().Be((12, 11, 12));
            cubes[1].From.Should().Be((10, 12, 10));
            cubes[1].To.Should().Be((12, 12, 12));

            // Case 4 - Y too large
            cubes = cube.SliceY(13).ToArray();
            cubes.Length.Should().Be(1);
            cubes[0].Should().Be(cube);
        }

        [Fact]
        public void SliceZTest()
        {
            // Case 1 - Y too small
            var cube = new Cube(true, (10, 10, 10), (12, 12, 12));
            var cubes = cube.SliceZ(10).ToArray();
            cubes.Length.Should().Be(1);
            cubes[0].Should().Be(cube);

            // Case 2 - Z is middle of range
            cubes = cube.SliceZ(11).ToArray();
            cubes.Length.Should().Be(2);
            cubes[0].From.Should().Be((10, 10, 10));
            cubes[0].To.Should().Be((12, 12, 10));
            cubes[1].From.Should().Be((10, 10, 11));
            cubes[1].To.Should().Be((12, 12, 12));

            // Case 3 - Z is last element
            cubes = cube.SliceZ(12).ToArray();
            cubes.Length.Should().Be(2);
            cubes[0].From.Should().Be((10, 10, 10));
            cubes[0].To.Should().Be((12, 12, 11));
            cubes[1].From.Should().Be((10, 10, 12));
            cubes[1].To.Should().Be((12, 12, 12));

            // Case 4 - Z too large
            cubes = cube.SliceZ(13).ToArray();
            cubes.Length.Should().Be(1);
            cubes[0].Should().Be(cube);
        }

        [Fact]
        public void SliceCubTest_MiddleCube()
        {
            var cube = new Cube(true, (10, 10, 10), (12, 12, 12));
            var slice = new Cube(true, (11, 11, 11), (11, 11, 11));

            var cubes = cube.SliceCube(slice)
                .OrderBy(c => c.From.x)
                .ThenBy(c => c.From.y)
                .ThenBy(c => c.From.z)
                .ToArray();

            cubes.Length.Should().Be(27);

            var i = 0;
            for (int x = 10; x <= 12; x++)
            {
                for (int y = 10; y <= 12; y++)
                {
                    for (int z = 10; z <= 2; z++)
                    {
                        var c = cubes[i++];
                        c.From.Should().Be(c.To);
                        c.From.Should().Be((x, y, z));
                    }
                }
            }
        }

        [Fact]
        public void SliceCubTest_FlatCube()
        {
            /*
             * #####
             * #####
             * ###...
             * ###...
             * #####
             * 
             */

            var cube = new Cube(true, (-1, -1, 0), (3, 3, 0));
            var slice = new Cube(true, (2, 1, -5), (4, 2, 5));

            var cubes = cube.SliceCube(slice)
                .OrderBy(c => c.From.x)
                .ThenBy(c => c.From.y)
                .ThenBy(c => c.From.z)
                .ToArray();

            cubes.Length.Should().Be(6);
            cubes[0].From.Should().Be((-1, -1, 0)); cubes[0].To.Should().Be((1, 0, 0));
            cubes[1].From.Should().Be((-1,  1, 0)); cubes[1].To.Should().Be((1, 2, 0));
            cubes[2].From.Should().Be((-1,  3, 0)); cubes[2].To.Should().Be((1, 3, 0));
            cubes[3].From.Should().Be(( 2, -1, 0)); cubes[3].To.Should().Be((3, 0, 0));
            cubes[4].From.Should().Be(( 2,  1, 0)); cubes[4].To.Should().Be((3, 2, 0));
            cubes[5].From.Should().Be(( 2,  3, 0)); cubes[5].To.Should().Be((3, 3, 0));
        }

        static IList<Cube> LoadCubes(string filename, int limit)
        {
            List<Cube> cubes = new();

            void sort(ref int a, ref int b)
            {
                if (b < a)
                {
                    var t = a;
                    a = b;
                    b = t;
                }
            }

            foreach (var line in FileIterator.Lines(filename))
            {
                var groups = line.Groups(@"(.+) x=([-\d]+)..([-\d]+),y=([-\d]+)..([-\d]+),z=([-\d]+)..([-\d]+)");
                var state = groups[0] == "on";
                int fromX = int.Parse(groups[1]);
                if (Math.Abs(fromX) > limit) continue;
                int toX = int.Parse(groups[2]);
                if (Math.Abs(toX) > limit) continue;
                int fromY = int.Parse(groups[3]);
                if (Math.Abs(fromY) > limit) continue;
                int toY = int.Parse(groups[4]);
                if (Math.Abs(toY) > limit) continue;
                int fromZ = int.Parse(groups[5]);
                if (Math.Abs(fromZ) > limit) continue;
                int toZ = int.Parse(groups[6]);
                if (Math.Abs(toZ) > limit) continue;

                sort(ref fromX, ref toX);
                sort(ref fromY, ref toY);
                sort(ref fromZ, ref toZ);

                cubes.Add(new Cube(
                    state,
                    (fromX, fromY, fromZ),
                    (toX, toY, toZ)
                ));

            }
            return cubes;
        }

        static void ApplyCube(HashSet<Cube> cubes, Cube cube)
        {
            List<Cube> newCubes = new();
            List<Cube> removedCubes = new();

            foreach (var c in cubes)
            {
                var intersect = c.Intersect(cube);
            }

            if (removedCubes.Count == 0)
            {
                cubes.Add(cube);
            }
            else
            {
                foreach (var c in removedCubes)
                    cubes.Remove(c);

                foreach (var c in newCubes)
                    cubes.Add(c);
            }
        }

        [Theory]
        [InlineData("Data/Day22_Test.txt", 590784)]
        [InlineData("Data/Day22.txt", 0)]
        public void Part1(string filename, long expectedAnswer)
        {
            var cubes = LoadCubes(filename, 50);
        }

        [Theory]
        [InlineData("Data/Day22_Test.txt", 0)]
        [InlineData("Data/Day22.txt", 0)]
        public void Part2(string filename, long expectedAnswer)
        {

        }
    }
}
