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

        (XY, XY) ParseLine(string line)
        {
            var groups = line.Groups(@"Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)");
            return (
                new(int.Parse(groups[0]), int.Parse(groups[1])),
                new(int.Parse(groups[2]), int.Parse(groups[3]))
            );
        }

        (XY beacon, XY sensor)[] LoadFile(string filename)
        {
            List<(XY beacon, XY sensor)> list = new();

            foreach (var line in FileIterator.Lines(filename))
            {
                list.Add(ParseLine(line));
            }

            return list.ToArray();
        }

        int CountBeaconsAtY((XY sensor, XY beacon)[] data, int y)
        {
            HashSet<(int, int)> set = new();

            foreach (var (_, beacon) in data)
                if (beacon.y == y)
                    set.Add(beacon.ToTuple());

            return set.Count;
        }

        void MergeIntoSet(HashSet<Span> set, Span span)
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

        HashSet<Span> GetLineSpans((XY sensor, XY beacon)[] data, int y)
        {
            HashSet<Span> spans = new();

            foreach (var (sensor, beacon) in data)
            {
                var distanceToBeacon = sensor.ManhattenDistanceTo(beacon);
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
    }
}
