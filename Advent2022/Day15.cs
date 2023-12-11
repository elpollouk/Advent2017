using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2022
{
    public class Day15
    {
        record Span(int from, int to);

        static (XY, XY) ParseLine(string line)
        {
            var groups = line.Groups(@"Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)");
            return (
                new(int.Parse(groups[0]), int.Parse(groups[1])),
                new(int.Parse(groups[2]), int.Parse(groups[3]))
            );
        }

        static (XY sensor, XY beacon)[] LoadFile(string filename)
        {
            List<(XY sensor, XY beacon)> list = new();

            foreach (var line in FileIterator.Lines(filename))
            {
                list.Add(ParseLine(line));
            }

            return list.ToArray();
        }

        static int CountBeaconsAtY((XY sensor, XY beacon)[] data, int y)
        {
            HashSet<(int, int)> set = new();

            foreach (var (_, beacon) in data)
                if (beacon.y == y)
                    set.Add(beacon.ToTuple());

            return set.Count;
        }

        static void MergeIntoSet(HashSet<Span> set, Span span)
        {
            HashSet<Span> remove = new();

            foreach (var existing in set)
            {
                // Span already fully covered by existing span
                if (existing.from <= span.from && span.to <= existing.to) return;

                if (span.from <= existing.from)
                {
                    if (span.to >= existing.to)
                    {
                        // Span completely covers existing
                        remove.Add(existing);
                    }
                    else if (span.to >= existing.from - 1) // -1 to handle "touching" spans
                    {
                        span = new(span.from, existing.to);
                        remove.Add(existing);
                    }
                }
                else if (existing.to >= span.from - 1)
                {
                    span = new(existing.from, span.to);
                    remove.Add(existing);
                }
            }

            foreach (var s in remove)
                set.Remove(s);
            
            set.Add(span);
        }

        static HashSet<Span> GetLineSpans((XY sensor, XY beacon)[] data, int y)
        {
            HashSet<Span> spans = new();

            foreach (var (sensor, beacon) in data)
            {
                var distanceToBeacon = sensor.ManhattanDistanceTo(beacon);
                var distanceToY = Math.Abs(sensor.y - y);

                var dy = distanceToBeacon - distanceToY;
                if (dy < 0) continue;
                
                Span span = new(sensor.x - dy, sensor.x + dy);
                MergeIntoSet(spans, span);
            }

            return spans;
        }

        [Theory]
        [InlineData("Data/Day15_Test.txt", 10, 26)]
        [InlineData("Data/Day15.txt", 2000000, 6425133)]
        public void Part1(string filename, int y, long expectedAnswer)
        {
            var data = LoadFile(filename);
            var numBeaconsAtY = CountBeaconsAtY(data, y);
            var spans = GetLineSpans(data, y);

            long empty = -numBeaconsAtY;
            foreach (var span in spans)
            {
                empty += span.to - span.from + 1;
            }

            empty.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day15_Test.txt", 20, 56000011)]
        [InlineData("Data/Day15.txt", 4000000, 10996191429555)]
        public void Part2(string filename, int extent,long expectedAnswer)
        {
            long result = 0;
            var data = LoadFile(filename);

            // Gamble on the answer being closer to the extent rather than the start
            for (int y = extent; y >= -1; y--)
            {
                var spans = GetLineSpans(data, y);
                if (spans.Count == 2)
                {
                    var spansArray = spans.ToArray();
                    long x = (spansArray[0].to < spansArray[1].to) ? spansArray[0].to : spansArray[1].to;
                    x++;
                    result = (x * 4000000) + y;
                    break;
                }
            }
            
            result.Should().Be(expectedAnswer);
        }


        //---------------------------------------------------------------------------------------//
        // An alternative solution for Part 2 that runs significantly faster!
        // NOTE: There is little value in pre-sorting any of the beacons as you need to iterate
        //       over all of them anyway to calulate their span ranges for the given row, so you
        //       may as well perform the point check at the same time.
        //---------------------------------------------------------------------------------------//
        class Sensor
        {
            public readonly XY Pos;
            public readonly int Range;

            public Sensor(XY pos, XY beacon)
            {
                Pos = pos;
                Range = pos.ManhattanDistanceTo(beacon);
            }

            // Return 0 if point is not in range, otherwise return the next x value that would be outside of the range of this sensor on this row
            public int CheckPoint(int x, int y)
            {
                var distanceToY = Math.Abs(Pos.y - y);
                var dy = Range - distanceToY;
                if (dy < 0) return 0;

                var minX = Pos.x - dy;
                var maxX = Pos.x + dy;
                if (x < minX || maxX < x) return 0;

                return maxX + 1;
            }
        }

        static Sensor[] LoadSensors(string filename)
        {
            return LoadFile(filename)
                .Select(d => new Sensor(d.sensor, d.beacon))
                .ToArray();
        }

        static int CheckPoint(Sensor[] sensors, int x, int y)
        {
            foreach (var sensor in sensors)
            {
                int r = sensor.CheckPoint(x, y);
                if (r != 0) return r;
            }
            return 0;
        }

        [Theory]
        [InlineData("Data/Day15_Test.txt", 20, 56000011)]
        [InlineData("Data/Day15.txt", 4000000, 10996191429555)]
        public void Part2_Alternative(string filename, int extent, long expectedAnswer)
        {
            long result = 0;
            var seonsors = LoadSensors(filename);

            // Gamble on the answer being closer to the extent rather than the start
            for (int y = extent; y >= -1; y--)
            {
                int x = 0;
                while (x < extent)
                {
                    var r = CheckPoint(seonsors, x, y);
                    if (r == 0)
                    {
                        result = (x * 4000000L) + y;
                        goto END;
                    }
                    x = r;
                }
            }

            END:
            result.Should().Be(expectedAnswer);
        }
    }
}
