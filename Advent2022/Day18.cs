using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2022
{
    public class Day18
    {
        XYZ Parse(string line)
        {
            var parts = line.Split(',');
            return new(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
        }

        int AddToSpace(HashSet<(int,int,int)> space, XYZ point)
        {
            space.Add(point.ToTuple());

            int delta = 6;
            foreach (var adjacent in point.GetAdjacent())
            {
                if (space.Contains(adjacent.ToTuple()))
                {
                    delta -= 2;
                }
            }
            
            return delta;
        }

        [Theory]
        [InlineData("Data/Day18_Test.txt", 64)]
        [InlineData("Data/Day18.txt", 3636)]
        public void Part1(string filename, long expectedAnswer)
        {
            long total = 0;
            HashSet<(int, int, int)> space = new();
            
            foreach (var point in FileIterator.Lines(filename).Select(Parse))
            {
                total += AddToSpace(space, point);
            }

            total.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day18_Test.txt", 58)]
        [InlineData("Data/Day18.txt", 2102)]
        public void Part2(string filename, long expectedAnswer)
        {
            HashSet<(int, int, int)> space = new();
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int minZ = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;
            int maxZ = int.MinValue;

            foreach (var point in FileIterator.Lines(filename).Select(Parse))
            {
                AddToSpace(space, point);
                if (point.x < minX) minX = point.x;
                if (point.x > maxX) maxX = point.x;
                if (point.y < minY) minY = point.y;
                if (point.y > maxY) maxY = point.y;
                if (point.z < minZ) minZ = point.z;
                if (point.z > maxZ) maxZ = point.z;
            }

            minX--; maxX++;
            minY--; maxY++;
            minZ--; maxZ++;

            var IsInBounds = (XYZ point) =>
            {
                if (point.x < minX || point.x > maxX) return false;
                if (point.y < minY || point.y > maxY) return false;
                if (point.z < minZ || point.z > maxZ) return false;
                return true;
            };

            long total = 0;
            HashSet<(int, int, int)> visited = new();
            Queue<XYZ> Q = new();
            Q.Enqueue(new(minX, minY, minZ));

            while (Q.Count > 0)
            {
                var point = Q.Dequeue();
                if (!IsInBounds(point)) continue;
                if (visited.Contains(point.ToTuple())) continue;
                visited.Add(point.ToTuple());

                foreach (var adjacent in point.GetAdjacent())
                {
                    if (space.Contains(adjacent.ToTuple()))
                    {
                        total++;
                    }
                    else
                    {
                        Q.Enqueue(adjacent);
                    }
                }
            }

            total.Should().Be(expectedAnswer);
        }
    }
}
