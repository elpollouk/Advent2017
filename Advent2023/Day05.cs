using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;
using Xunit;

namespace Advent2023
{
    public class Day05
    {
        record Range(long start, long end, long destinationBase);

        class RangeMapper
        {
            private readonly List<Range> ranges = [];

            public void AddRange(long source, long desitination, long length)
            {
                ranges.Add(new(source, source + length, desitination));
            }

            public long Map(long value)
            {
                foreach (var range in ranges)
                {
                    if (range.start <= value && value < range.end)
                    {
                        var offset = value - range.start;
                        return range.destinationBase + offset;
                    }
                }
                return value;
            }
        }

        RangeMapper ParseMapper(Func<string> reader)
        {
            RangeMapper mapper = new();

            var line = reader();
            line = reader();
            while (line != null && line.Length > 0)
            {
                var numbers = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                mapper.AddRange(long.Parse(numbers[1]), long.Parse(numbers[0]), long.Parse(numbers[2]));
                line = reader();
            }

            return mapper;
        }

        Func<long, long> ParseMappers(Func<string> reader)
        {
            RangeMapper[] mappers = {
                ParseMapper(reader), // seed-to-soil
                ParseMapper(reader), // soil-to-fertilizer
                ParseMapper(reader), // fertilizer-to-water
                ParseMapper(reader), // water-to-light
                ParseMapper(reader), // light-to-temperature
                ParseMapper(reader), // temperature-to-humidity
                ParseMapper(reader)  // humidity-to-location
            };

            return value =>
            {
                foreach (var mapper in mappers)
                {
                    value = mapper.Map(value);
                }
                return value;
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
            var seeds = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(v => long.Parse(v)).ToArray();
            reader();

            var mapper = ParseMappers(reader);

            long minLocation = long.MaxValue;

            foreach (var seed in seeds)
            {
                var value = mapper(seed);
                if (value < minLocation) minLocation = value;
            }

            minLocation.Should().Be(expectedAnswer);
        }

        Task<long> FindMin(Func<long, long> mapper, (long from, long to) range)
        {
            return Task.Run(() =>
            {
                long minLocation = long.MaxValue;
                for (long seed = range.from; seed <= range.to; seed++)
                {
                    var value = mapper(seed);
                    if (value < minLocation) minLocation = value;
                }

                return minLocation;
            });
        }

        [Theory]
        [InlineData("Data/Day05_Test.txt", 46)]
        //[InlineData("Data/Day05.txt", 56931769)]
        async public void Part2(string filename, long expectedAnswer)

        {
            var reader = FileIterator.CreateLineReader(filename);
            var line = reader();
            line = line.Split(':')[1];
            var seeds = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(v => long.Parse(v)).ToArray();
            reader();

            var mapper = ParseMappers(reader);

            // Divide each see range in half so that we can run across more cores
            var seedRanges = new (long from, long to)[seeds.Length];
            for (int i = 0; i < seeds.Length / 2; i++)
            {
                var length = seeds[i * 2 + 1] / 2;
                var from = seeds[i * 2];
                var to = from + length;
                seedRanges[i * 2] = (from, to);
                seedRanges[i * 2 + 1] = (from + length, from + seeds[i * 2 + 1]);
            }

            long minLocation = long.MaxValue;

            List<Task<long>> tasks = [];

            foreach (var range in seedRanges)
            {
                tasks.Add(FindMin(mapper, range));
            }

            foreach (var task in tasks)
            {
                var value = await task;
                if (value < minLocation) minLocation = value;
            }

            minLocation.Should().Be(expectedAnswer);
        }
    }
}
