using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Utils;
using Xunit;

namespace Advent2023
{
    public class Day18
    {
        //---------------------------------------------------------------------------------------//
        // Part 1
        //---------------------------------------------------------------------------------------//

        record Instruction(char Direction, int Distance, int Colour);

        static int FromHex(string hex)
        {
            int value = 0;
            foreach (var c in hex)
            {
                value <<= 4;
                if ('0' <= c && c <= '9') value += c - '0';
                else value += 10 + (c - 'a');
            }
            return value;
        }

        List<Instruction> Load(string filename)
        {
            List<Instruction> instructions = [];

            foreach (var line in FileIterator.Lines(filename))
            {
                var match = line.Match(@"^(.) (\d+) \(#(......)\)$");
                char direction = match.Groups[1].Value[0];
                int distance = int.Parse(match.Groups[2].Value);
                int colour = FromHex(match.Groups[3].Value);

                instructions.Add(new(direction, distance, colour));
            }

            return instructions;
        }

        Dictionary<(int x, int y), int> Execute(List<Instruction> instructions)
        {
            Dictionary<(int x, int y), int> surface = [];
            surface[(0, 0)] = 0xFFFFFF;
            XY pos = new(0, 0);

            foreach (var instruction in instructions)
            {
                var (dX, dY) = instruction.Direction switch
                {
                    'U' => (0, -1),
                    'D' => (0, 1),
                    'L' => (-1, 0),
                    'R' => (1, 0),
                    _ => throw new Exception()
                };

                for (int i = 0; i < instruction.Distance; i++)
                {
                    pos.Add(dX, dY);
                    surface[pos.ToTuple()] = instruction.Colour;
                }
            }

            return surface;
        }

        void Fill(Dictionary<(int x, int y), int> surface, int x, int y, int colour)
        {
            Queue<(int x, int y)> queue = [];
            queue.Enqueue((x, y));

            while (queue.Count > 0)
            {
                var pos = queue.Dequeue();
                if (surface.ContainsKey(pos)) continue;
                surface[pos] = colour;
                queue.Enqueue((pos.x - 1, pos.y));
                queue.Enqueue((pos.x + 1, pos.y));
                queue.Enqueue((pos.x, pos.y - 1));
                queue.Enqueue((pos.x, pos.y + 1));
            }
        }

        [Theory]
        [InlineData("Data/Day18_Test.txt", 62)]
        [InlineData("Data/Day18.txt", 40131)]
        public void Part1(string filename, int expectedAnswer)
        {
            var instructions = Load(filename);
            var surface = Execute(instructions);
            Fill(surface, 1, 1, 0xC4C4C4);
            surface.Count.Should().Be(expectedAnswer);
        }


        //---------------------------------------------------------------------------------------//
        // Part 2
        //---------------------------------------------------------------------------------------//

        record Line((long x, long y) from, (long x, long y) to);
        record Partition((long x, long y) min, (long x, long y) max, List<Line> shape);

        ((long x, long y) min, (long x, long y) max) GetExtents(List<Line> shape)
        {
            long minX = long.MaxValue;
            long minY = long.MaxValue;
            long maxX = long.MinValue;
            long maxY = long.MinValue;

            foreach (var line in shape)
            {
                minX = long.Min(minX, line.from.x);
                minY = long.Min(minY, line.from.y);
                maxX = long.Max(maxX, line.from.x);
                maxY = long.Max(maxY, line.from.y);
            }

            return (
                (minX, minY),
                (maxX, maxY)
            );
        }

        static bool IsOneByOne(Line line)
        {
            return line.from == line.to;
        }

        static bool IsL(Partition part)
        {
            HashSet<(long x, long y)> vertices = [];
            foreach (var line in part.shape)
            {
                vertices.Add(line.from);
                vertices.Add(line.to);
            }

            return vertices.Count == 3;
        }

        // Cap off open parts of the shape so that we still have a continuous line
        void CreateVerticalSeals(Partition part, IEnumerable<(long x, long y)> exits, bool ascending)
        {
            (exits.Count() % 2).Should().Be(0);
            IEnumerable<(long x, long y)> vertices;
            if (ascending)
            {
                vertices = exits.OrderBy(v => v.y);
            }
            else
            {
                vertices = exits.OrderByDescending(v => v.y);
            }

            var it = vertices.GetEnumerator();
            while (it.MoveNext())
            {
                var from = it.Current;
                it.MoveNext();
                var to = it.Current;
                part.shape.Add(new(from, to));
            }
        }

        void CreateHorizontalSeals(Partition part, IEnumerable<(long x, long y)> exits, bool ascending)
        {
            (exits.Count() % 2).Should().Be(0);
            IEnumerable<(long x, long y)> vertices;
            if (ascending)
            {
                vertices = exits.OrderBy(v => v.x);
            }
            else
            {
                vertices = exits.OrderByDescending(v => v.x);
            }

            var it = vertices.GetEnumerator();
            while (it.MoveNext())
            {
                var from = it.Current;
                it.MoveNext();
                var to = it.Current;
                part.shape.Add(new(from, to));
            }
        }

        Partition FilterOneByOne(Partition part, ref long count)
        {
            List<Line> newShape = [];
            HashSet<(long x, long y)> seen = [];
            List<(long x, long y)> potentialSurvivors = [];

            foreach (var line in part.shape)
            {
                if (!IsOneByOne(line))
                {
                    newShape.Add(line);
                    seen.Add(line.from);
                    continue;
                }

                if (seen.Contains(line.from))
                {
                    continue;
                }

                potentialSurvivors.Add(line.from);
            }

            foreach (var point in potentialSurvivors)
            {
                if (seen.Contains(point))
                {
                    continue;
                }

                seen.Add(point);
                count += 1;
            }

            return new Partition(part.min, part.max, newShape);
        }

        long SplitVertical(Partition part)
        {
            if (part.shape.Count == 0)
            {
                return 0;
            }

            if (part.shape.Count <= 4)
            {
                var (min, max) = GetExtents(part.shape);
                if (IsL(part))
                {
                    return (max.x - min.x) + (max.y - min.y) + 1;
                }
                else
                {
                    return (max.x - min.x + 1) * (max.y - min.y + 1);
                }
            }

            var splitX = (part.max.x + part.min.x) / 2;

            Partition left = new(part.min, (splitX, part.max.y), []);
            Partition right = new((splitX+1, part.min.y), part.max, []);

            List<(long x, long y)> leftExits = [];
            List<(long x, long y)> rightExits = [];

            foreach (var line in part.shape)
            {
                if (line.from.x <= splitX && line.to.x <= splitX)
                {
                    left.shape.Add(line);
                    continue;
                }

                if (splitX < line.from.x && splitX < line.to.x)
                {
                    right.shape.Add(line);
                    continue;
                }

                if (line.from.x <= splitX)
                {
                    Line newLine = new(line.from, (splitX, line.to.y));
                    left.shape.Add(newLine);
                    leftExits.Add(newLine.to);

                    newLine = new((splitX + 1, line.from.y), line.to);
                    right.shape.Add(newLine);
                    rightExits.Add(newLine.from);
                }
                else
                {
                    Line newLine = new(line.from, (splitX + 1, line.to.y));
                    right.shape.Add(newLine);
                    rightExits.Add(newLine.to);

                    newLine = new((splitX, line.from.y), line.to);
                    left.shape.Add(newLine);
                    leftExits.Add(newLine.from);
                }
            }

            CreateVerticalSeals(left, leftExits, true);
            CreateVerticalSeals(right, rightExits, false);

            long area = 0;
            left = FilterOneByOne(left, ref area);
            right = FilterOneByOne(right, ref area);

            area += SplitHorizontal(left);
            area += SplitHorizontal(right);

            return area;
        }

        long SplitHorizontal(Partition part)
        {
            if (part.shape.Count == 0)
            {
                return 0;
            }

            if (part.shape.Count <= 4)
            {
                var (min, max) = GetExtents(part.shape);
                if (IsL(part))
                {
                    return (max.x - min.x) + (max.y - min.y) + 1;
                }
                else
                {
                    return (max.x - min.x + 1) * (max.y - min.y + 1);
                }
            }

            var splitY = (part.max.y + part.min.y) / 2;

            Partition top = new(part.min, (part.max.x, splitY), []);
            Partition bottom = new((part.min.x, splitY + 1), part.max, []);

            List<(long x, long y)> topExits = [];
            List<(long x, long y)> bottomExits = [];

            foreach (var line in part.shape)
            {
                if (line.from.y <= splitY && line.to.y <= splitY)
                {
                    top.shape.Add(line);
                    continue;
                }

                if (splitY < line.from.y && splitY < line.to.y)
                {
                    bottom.shape.Add(line);
                    continue;
                }

                if (line.from.y <= splitY)
                {
                    Line newLine = new(line.from, (line.to.x, splitY));
                    top.shape.Add(newLine);
                    topExits.Add(newLine.to);

                    newLine = new((line.from.x, splitY + 1), line.to);
                    bottom.shape.Add(newLine);
                    bottomExits.Add(newLine.from);
                }
                else
                {
                    Line newLine = new(line.from, (line.to.x, splitY + 1));
                    bottom.shape.Add(newLine);
                    bottomExits.Add(newLine.to);

                    newLine = new((line.from.x, splitY), line.to);
                    top.shape.Add(newLine);
                    topExits.Add(newLine.from);
                }
            }

            CreateHorizontalSeals(top, topExits, false);
            CreateHorizontalSeals(bottom, bottomExits, true);

            long area = 0;
            top = FilterOneByOne(top, ref area);
            bottom = FilterOneByOne(bottom, ref area);

            area += SplitVertical(top);
            area += SplitVertical(bottom);

            return area;
        }

        List<Line> LoadShape(string filename)
        {
            List<Line> shape = [];

            XY pos = new(0, 0);
            foreach (var line in FileIterator.Lines(filename))
            {
                var match = line.Match(@"#(.....)(\d)\)$");
                var distance = FromHex(match.Groups[1].Value);
                var direction = match.Groups[2].Value[0];

                // R:0, D:1, L:2, U:3
                var (dX, dy) = direction switch
                {
                    '0' => (distance, 0),
                    '1' => (0, distance),
                    '2' => (-distance, 0),
                    '3' => (0, -distance),
                    _ => throw new Exception()
                };

                var from = pos.ToTuple();
                pos.Add(dX, dy);
                shape.Add(new(from, pos.ToTuple()));
            }

            return shape;
        }

        [Fact]
        public void SimpleShape()
        {
            // ..##..
            // ..##..
            // ..####
            // ######
            // ######
            List<Line> shape = [
                new((0, 0), (1, 0)),
                new((1, 0), (1, 2)),
                new((1, 2), (3, 2)),
                new((3, 2), (3, 4)),
                new((3, 4), (-2, 4)),
                new((-2, 4), (-2, 3)),
                new((-2, 3), (0, 3)),
                new((0, 3), (0, 0))
            ];

            var (min, max) = GetExtents(shape);
            Partition part = new(min, max, shape);
            var total = SplitVertical(part);
            total.Should().Be(20);
        }

        [Theory]
        [InlineData("Data/Day18_TestSimple.txt", 20)]
        [InlineData("Data/Day18_TestComplex.txt", 44)]
        [InlineData("Data/Day18_Test.txt", 952408144115)]
        [InlineData("Data/Day18.txt", 104454050898331)]
        public void Part2(string filename, long expectedAnswer)
        {
            var shape = LoadShape(filename);
            var (min, max) = GetExtents(shape);
            Partition part = new(min, max, shape);
            var total = SplitVertical(part);
            total.Should().Be(expectedAnswer);
        }


        //---------------------------------------------------------------------------------------//
        // Shoelace Formula
        // https://www.themathdoctors.org/polygon-coordinates-and-areas/
        //---------------------------------------------------------------------------------------//

        List<(long x, long y)> LoadVertices(string filename)
        {
            XY pos = new(0, 0);
            List<(long x, long y)> verts = [pos.ToTuple()];

            foreach (var line in FileIterator.Lines(filename))
            {
                var match = line.Match(@"#(.....)(\d)\)$");
                var distance = FromHex(match.Groups[1].Value);
                var direction = match.Groups[2].Value[0];

                // R:0, D:1, L:2, U:3
                var (dX, dy) = direction switch
                {
                    '0' => (distance, 0),
                    '1' => (0, distance),
                    '2' => (-distance, 0),
                    '3' => (0, -distance),
                    _ => throw new Exception()
                };

                var from = pos.ToTuple();
                pos.Add(dX, dy);
                verts.Add(pos.ToTuple());
            }

            return verts;
        }

        [Theory]
        [InlineData("Data/Day18_TestSimple.txt", 20)]
        [InlineData("Data/Day18_TestComplex.txt", 44)]
        [InlineData("Data/Day18_Test.txt", 952408144115)]
        [InlineData("Data/Day18.txt", 104454050898331)]
        public void Part2_Shoelace(string filename, long expectedAnswer)
        {
            var verts = LoadVertices(filename);
            long area = 0;

            for (int i = 0; i <  verts.Count - 1; i++)
            {
                area += (verts[i].x * verts[i + 1].y) - (verts[i].y * verts[i + 1].x);

                // We need to factor in the volume of the line itself as well
                // https://www.reddit.com/r/adventofcode/comments/18l2nk2/2023_day_18_easiest_way_to_solve_both_parts/kdv9bfk/
                area += long.Abs(verts[i].x - verts[i+1].x + verts[i].y - verts[i+1].y);
            }

            area = (area / 2) + 1;

            area.Should().Be(expectedAnswer);
        }
    }
}
