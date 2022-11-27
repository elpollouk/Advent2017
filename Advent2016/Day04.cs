using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2016
{
    public class Day04
    {
        class CountComparer : IComparer<KeyValuePair<char, int>>
        {
            public readonly static CountComparer Inst = new();

            public int Compare(KeyValuePair<char, int> x, KeyValuePair<char, int> y)
            {
                var order = y.Value - x.Value;
                if (order == 0)
                {
                    order = x.Key < y.Key ? -1 : 1;
                }
                return order;
            }
        }

        class Room
        {
            public readonly string Name;
            public readonly long SectorId;
            public readonly string Checksum;

            public bool IsReal {
                get
                {
                    Dictionary<char, int> counts = new();
                    foreach (char c in Name)
                    {
                        if (c == '-') continue;
                        counts.Increment(c);
                    }

                    var cs = counts.OrderBy(kv => kv, CountComparer.Inst)
                        .Take(5)
                        .Select(kv => kv.Key)
                        .ToArray()
                        .Join("");
                    return cs == Checksum;
                }
            }

            public string Decrypt()
            {
                long shift = SectorId % 26;
                char[] output = new char[Name.Length];
                for (int i = 0; i < output.Length; i++)
                {
                    if (Name[i] == '-')
                    {
                        output[i] = ' ';
                        continue;
                    }
                    output[i] = (char)(Name[i] + shift);
                    if ('z' < output[i]) output[i] = (char)(output[i] - 26);
                }

                return output.Join("");
            }

            public Room(string encoded)
            {
                var groups = encoded.Groups("^(.+)-(\\d+)\\[(.+)]$");
                Name = groups[0];
                SectorId = long.Parse(groups[1]);
                Checksum = groups[2];
            }
        }

        [Theory]
        [InlineData("aaaaa-bbb-z-y-x-123[abxyz]", true)]
        [InlineData("a-b-c-d-e-f-g-h-987[abcde]", true)]
        [InlineData("not-a-real-room-404[oarel]", true)]
        [InlineData("totally-real-room-200[decoy]", false)]
        public void IsReal(string text, bool expectedAnswer)
        {
            Room room = new(text);
            room.IsReal.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day04_Test.txt", 1514)]
        [InlineData("Data/Day04.txt", 173787)]
        public void Part1(string filename, long expectedAnswer)
        {
            long sum = 0;
            foreach (var line in FileIterator.Lines(filename))
            {
                Room room = new(line);
                if (room.IsReal)
                {
                    sum += room.SectorId;
                }
            }

            sum.Should().Be(expectedAnswer);
        }

        [Fact]
        public void Part2Example()
        {
            Room room = new("qzmt-zixmtkozy-ivhz-343[abcdef]");
            room.Decrypt().Should().Be("very encrypted name");
        }

        [Theory]
        [InlineData("Data/Day04.txt", 548)]
        public void Part2(string filename, long expectedAnswer)
        {
            long secordId = -1;
            foreach (var line in FileIterator.Lines(filename))
            {
                Room room = new(line);
                if (room.IsReal && room.Decrypt() == "northpole object storage")
                {
                    secordId = room.SectorId;
                    break;
                }
            }

            secordId.Should().Be(expectedAnswer);
        }
    }
}
