﻿using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2023
{
    public class Day05
    {
        class Range(long start, long end)
        {
            public readonly long start = start;
            public readonly long end = end;
            public long length => end - start;

            public bool Before(Range other)
            {
                return end <= other.start;
            }

            public bool Contains(long value)
            {
                return value >= start && value < end;
            }

            public bool Contains(Range other)
            {
                return start <= other.start && other.end <= end;
            }

            public (Range lower, Range upper) Split(long atValue)
            {
                return (
                    new(start, atValue),
                    new(atValue, end)
                );
            }

            public override string ToString()
            {
                return $"{start}-{end}";
            }
        }

        class Mapping(Range range, long destinationBase)
        {
            public readonly Range range = range;
            public readonly long destinationBase = destinationBase;

            public bool Contains(long value) => range.Contains(value);
            public bool Contains(Range other) => range.Contains(other);

            public long Map(long value)
            {
                long offset = value - range.start;
                return destinationBase + offset;
            }

            public Range Map(Range other)
            {
                long offset = other.start - range.start;
                long start = destinationBase + offset;
                long end = start + other.length;
                return new(start, end);
            }

            public override string ToString()
            {
                return $"{range} -> {destinationBase}";
            }
        }

        class RangeMapper
        {
            private readonly List<Mapping> mappings = [];

            public void AddRange(long source, long destination, long length)
            {
                mappings.Add(new(new(source, source + length), destination));
            }

            public long Map(long value)
            {
                foreach (var mapping in mappings)
                {
                    if (mapping.Contains(value))
                    {
                        return mapping.Map(value);
                    }
                }
                return value;
            }

            public List<Range> Map(List<Range> ranges)
            {
                List<Range> result = [];
                int mappingIndex = 0;
                int rangeIndex = 0;
                Range bufferedRange = null;
                Mapping mapping = mappings[mappingIndex++];

                // Pull house keeping logic into lambdas to keep the main loop logic as clean as possible
                Range NextRange()
                {
                    // Consume either a buffered half of a split range or a new range from the input list
                    Range range = bufferedRange;
                    if (range == null)
                    {
                        range = ranges[rangeIndex++];
                    }
                    else
                    {
                        bufferedRange = null;
                    }
                    return range;
                }

                Mapping NextMapping()
                {
                    if (mappingIndex < mappings.Count)
                    {
                        return mappings[mappingIndex++];
                    }
                    else
                    {
                        return null;
                    }
                }

                while (rangeIndex < ranges.Count || bufferedRange != null)
                {
                    Range range = NextRange();

                    if (mapping == null)
                    {
                        // Range is after all the mappings
                        result.Add(range);
                    }
                    else if (range.Before(mapping.range))
                    {
                        // Range is completely before the current mapping
                        result.Add(range);
                    }
                    else if (mapping.range.Before(range))
                    {
                        // The mapping is completely before the current range
                        // Buffer this range, update the mapping and re-evaluate again on the next iteration
                        bufferedRange = range;
                        mapping = NextMapping();
                    }
                    else if (mapping.Contains(range))
                    {
                        // Mapping contains the entire range
                        result.Add(mapping.Map(range));
                    }
                    else if (mapping.Contains(range.start))
                    {
                        // Range starts part way through the mapping
                        // Split the range and map the lower part that it contained within the current mapper range
                        var (lower, upper) = range.Split(mapping.range.end);
                        result.Add(mapping.Map(lower));
                        // Buffer the upper part to re-evaluate on the next iteration
                        bufferedRange = upper;
                        // Update the mapping as we've now consumed it via this range
                        mapping = NextMapping();
                    }
                    else
                    {
                        // Range ends part way through or after the mapping
                        // Split the range and pass the non-mapped lower part through to the results
                        var (lower, upper) = range.Split(mapping.range.start);
                        result.Add(lower);
                        // Buffer the overlapping upper part for re-evaluation on the next iteration
                        bufferedRange = upper;
                    }
                }

                result.Sort((r1, r2) => r1.start < r2.start ? -1 : 1);
                return result;
            }

            public void Sort()
            {
                mappings.Sort((m1, m2) => m1.range.start < m2.range.start ? -1 : 1);
            }
        }

        RangeMapper ParseMapper(Func<string> reader)
        {
            RangeMapper mapper = new();

            reader();
            var line = reader();
            while (line != null && line.Length > 0)
            {
                var numbers = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                mapper.AddRange(long.Parse(numbers[1]), long.Parse(numbers[0]), long.Parse(numbers[2]));
                line = reader();
            }
            mapper.Sort();

            return mapper;
        }

        RangeMapper[] ParseMappers(Func<string> reader)
        {
            return [
                ParseMapper(reader), // seed-to-soil
                ParseMapper(reader), // soil-to-fertilizer
                ParseMapper(reader), // fertilizer-to-water
                ParseMapper(reader), // water-to-light
                ParseMapper(reader), // light-to-temperature
                ParseMapper(reader), // temperature-to-humidity
                ParseMapper(reader)  // humidity-to-location
            ];
        }

        Func<long, long> BuildPipeline1(Func<string> reader)
        {
            var mappers = ParseMappers(reader);

            return value =>
            {
                foreach (var mapper in mappers)
                {
                    value = mapper.Map(value);
                }
                return value;
            };
        }

        Func<List<Range>, List<Range>> BuildPipeline2(Func<string> reader)
        {
            var mappers = ParseMappers(reader);

            return ranges =>
            {
                foreach (var mapper in mappers)
                {
                    ranges = mapper.Map(ranges);
                }
                return ranges;
            };
        }

        [Theory]
        [InlineData("Data/Day05_Test.txt", 35)]
        [InlineData("Data/Day05.txt", 486613012)]
        public void Part1(string filename, long expectedAnswer)
        {
            var reader = FileIterator.CreateLineReader(filename);
            var line = reader();
            line = line.Split(':')[1];
            var seeds = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
            reader();

            var mapper = BuildPipeline1(reader);

            long minLocation = long.MaxValue;

            foreach (var seed in seeds)
            {
                var value = mapper(seed);
                if (value < minLocation) minLocation = value;
            }

            minLocation.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day05_Test.txt", 46)]
        [InlineData("Data/Day05.txt", 56931769)]
        public void Part2(string filename, long expectedAnswer)

        {
            var reader = FileIterator.CreateLineReader(filename);
            var line = reader();
            line = line.Split(':')[1];
            var seeds = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
            reader();

            var mapper = BuildPipeline2(reader);

            List<Range> seedRanges = [];
            for (int i = 0; i < seeds.Length / 2; i++)
            {
                var length = seeds[i * 2 + 1];
                var from = seeds[i * 2];
                var to = from + length;
                seedRanges.Add(new(from, to));
            }
            seedRanges.Sort((r1, r2) => r1.start < r2.start ? -1 : 1);

            var locations = mapper(seedRanges);
            locations[0].start.Should().Be(expectedAnswer);
        }
    }
}
