using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Utils.DataStructures;
using Xunit;

namespace Advent2018
{
    public class Day23
    {
        class Partition
        {
            public readonly (int x, int y, int z) Pos;
            public readonly (int width, int height, int depth) Size;
            public readonly (int x, int y, int z)[] Extents = new (int x, int y, int z)[8];
            public int Volume => Size.width * Size.height * Size.depth;
            public readonly ICollection<(int x, int y, int z, int r)> Bots;

            public Partition((int x, int y, int z) pos, (int width, int height, int depth) size)
            {
                if (size.width * size.height * size.depth == 0) throw new Exception("Invalid partition size");

                Pos = pos;
                Size = size;

                Extents[0] = Pos;
                Extents[1] = (Pos.x + Size.width - 1, Pos.y, Pos.z);
                Extents[2] = (Pos.x, Pos.y + Size.height - 1, Pos.z);
                Extents[3] = (Pos.x + Size.width - 1, Pos.y + Size.height - 1, Pos.z);
                Extents[4] = (Pos.x, Pos.y, Pos.z + Size.depth - 1);
                Extents[5] = (Pos.x + Size.width - 1, Pos.y, Pos.z + Size.depth - 1);
                Extents[6] = (Pos.x, Pos.y + Size.height - 1, Pos.z + Size.depth - 1);
                Extents[7] = (Pos.x + Size.width - 1, Pos.y + Size.height - 1, Pos.z + Size.depth - 1);

                Bots = new List<(int x, int y, int z, int r)>();
            }

            IEnumerable<(int x, int y, int z)> GetBotExtents((int x, int y, int z, int r) bot)
            {
                yield return (bot.x + bot.r, bot.y, bot.z);
                yield return (bot.x - bot.r, bot.y, bot.z);
                yield return (bot.x, bot.y + bot.r, bot.z);
                yield return (bot.x, bot.y - bot.r, bot.z);
                yield return (bot.x, bot.y, bot.z + bot.r);
                yield return (bot.x, bot.y, bot.z - bot.r);
            }

            public bool ContainsPoint(int x, int y, int z)
            {
                return (Pos.x <= x) && (x < Pos.x + Size.width)
                     && (Pos.y <= y) && (y < Pos.y + Size.height)
                     && (Pos.z <= z) && (z < Pos.z + Size.depth);
            }

            private static bool WithinBotRange((int x, int y, int z) point, (int x, int y, int z, int r) bot)
            {
                var distance = Math.Abs(point.x - bot.x) + Math.Abs(point.y - bot.y) + Math.Abs(point.z - bot.z);
                return distance <= bot.r;
            }

            public bool WithinRange((int x, int y, int z, int r) bot)
            {
                // Check if bot is actually inside the partition
                if (ContainsPoint(bot.x, bot.y, bot.z))
                    return true;

                // Check if the range extents of the bot fall within the partition
                foreach (var (x, y, z) in GetBotExtents(bot))
                    if (ContainsPoint(x, y, z))
                        return true;

                // Check if the extents of this partition fall within range of the bot
                foreach (var extent in Extents)
                    if (WithinBotRange(extent, bot))
                        return true;

                return false;
            }

            public void AddBotIfWithinRange((int x, int y, int z, int r) bot)
            {
                if (WithinRange(bot))
                    Bots.Add(bot);
            }

            public IEnumerable<Partition> CreateSubPartitions()
            {
                if (Volume == 1) throw new Exception("Partition already at minimum size");

                var halfWidth = Size.width / 2;
                var halfHeight = Size.height / 2;
                var halfDepth = Size.depth / 2;

                if (halfWidth != 0 && halfHeight != 0 && halfDepth != 0)
                    yield return new Partition((Pos), (halfWidth, halfHeight, halfDepth));
                if (halfHeight != 0 && halfDepth != 0)
                    yield return new Partition((Pos.x + halfWidth, Pos.y, Pos.z), (Size.width - halfWidth, halfHeight, halfDepth));
                if (halfWidth != 0 && halfDepth != 0)
                    yield return new Partition((Pos.x, Pos.y + halfHeight, Pos.z), (halfWidth, Size.height - halfHeight, halfDepth));
                if (halfDepth != 0)
                    yield return new Partition((Pos.x + halfWidth, Pos.y + halfHeight, Pos.z), (Size.width - halfWidth, Size.height - halfHeight, halfDepth));
                if (halfWidth != 0 && halfHeight != 0)
                    yield return new Partition((Pos.x, Pos.y, Pos.z + halfDepth), (halfWidth, halfHeight, Size.depth - halfDepth));
                if (halfHeight != 0)
                    yield return new Partition((Pos.x + halfWidth, Pos.y, Pos.z + halfDepth), (Size.width - halfWidth, halfHeight, Size.depth - halfDepth));
                if (halfWidth != 0)
                    yield return new Partition((Pos.x, Pos.y + halfHeight, Pos.z + halfDepth), (halfWidth, Size.height - halfHeight, Size.depth - halfDepth));

                yield return new Partition((Pos.x + halfWidth, Pos.y + halfHeight, Pos.z + halfDepth), (Size.width - halfWidth, Size.height - halfHeight, Size.depth - halfDepth));
            }

            public IEnumerable<Partition> CreateSubPartitionsAndPopulate()
            {
                foreach (var partition in CreateSubPartitions())
                {
                    foreach (var bot in Bots)
                        partition.AddBotIfWithinRange(bot);
                    yield return partition;
                }
            }
        }

        (int x, int y, int z, int r) ParseNode(string input)
        {
            var matches = input.Match(@"pos=<(\-?\d+),(\-?\d+),(\-?\d+)>, r=(\d+)");
            return (
                int.Parse(matches.Groups[1].Value),
                int.Parse(matches.Groups[2].Value),
                int.Parse(matches.Groups[3].Value),
                int.Parse(matches.Groups[4].Value)
            );
        }

        Partition BuildInitialPartition((int x, int y, int z, int r)[] bots)
        {
            var minX = int.MaxValue;
            var minY = int.MaxValue;
            var minZ = int.MaxValue;
            var maxX = int.MinValue;
            var maxY = int.MinValue;
            var maxZ = int.MinValue;

            foreach (var (x, y, z, r) in bots)
            {
                minX = Math.Min(minX, x);
                maxX = Math.Max(maxX, x);
                minY = Math.Min(minY, y);
                maxY = Math.Max(maxY, y);
                minZ = Math.Min(minZ, z);
                maxZ = Math.Max(maxZ, z);
            }

            var partition = new Partition(
                (minX, minY, minZ),
                (maxX - minX + 1, maxY - minY + 1, maxZ - minZ + 1)
            );

            foreach (var bot in bots)
                partition.Bots.Add(bot);

            return partition;
        }

        bool WithinRange((int x, int y, int z, int r) sourceBot, (int x, int y, int z, int r) targetBot)
        {
            var distance = Math.Abs(sourceBot.x - targetBot.x) + Math.Abs(sourceBot.y - targetBot.y) + Math.Abs(sourceBot.z - targetBot.z);
            return distance <= sourceBot.r;
        }

        [Theory]
        [InlineData(7, "Data/Day23-Test.txt")]
        [InlineData(248, "Data/Day23.txt")] // Solution
        void Problem1(int expectedAnswer, string inputFile)
        {
            var bots = FileIterator.Lines(inputFile).Select(l => ParseNode(l)).ToArray();
            var strongest = bots.MaxItem(b => b.r);

            var count = 0;
            for (var i = 0; i < bots.Length; i++)
            {
                if (WithinRange(strongest, bots[i]))
                    count++;
            }

            count.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData(false, 2, 2, 0, 3, 2, 1, 0, 0, 0, 2)] // No point should fall in range
        [InlineData(true, 2, 2, 0, 3, 2, 1, 3, 2, 0, 9)]  // Bot inside partition, but extents outside
        [InlineData(true, 2, 2, 0, 3, 2, 2, 3, 2, 1, 9)]  // Bot inside partition, offset z-axis
        [InlineData(true, 2, 2, 0, 3, 2, 1, 3, 0, 0, 2)]  // Only the extent should be in range
        [InlineData(true, 2, 2, 0, 3, 2, 1, 3, 4, 0, 1)]  // Only the extent should be in range
        [InlineData(true, 2, 2, 0, 3, 2, 1, 0, 2, 0, 4)]  // Slightly longer extent range
        [InlineData(true, 2, 2, 0, 3, 2, 1, 5, 4, 0, 2)]  // No extents in range, but diagonals are
        [InlineData(false, 2, 2, 0, 3, 2, 1, 5, 4, 1, 2)] // Z axis offset taking the bot just out of range
        [InlineData(true, 1, 2, 0, 3, 2, 1, 0, 0, 0, 3)]  // No bot extents are in range, but ranged extents are within partition
        [InlineData(true, 1, 2, 0, 3, 2, 1, 4, 5, 0, 3)]  // Same as above, but bot is to the bottom-left of the partition
        [InlineData(true, 1, 2, 0, 3, 2, 1, -10, -10, -10, 10000)] // Bot has a range the completely overshoots partition
        [InlineData(true, -3, -3, -3, 3, 3, 3, -2, -4, -2, 1)] // Sanity check space and bot in negative area
        [InlineData(false, -3, -3, -3, 3, 3, 3, -2, -5, -2, 1)] // Same as above, except bot is slightly out of range
        [InlineData(true, -4, -4, -4, 1, 1, 1, -3, -3, -3, 3)] // Smallest partition, none of the axis match, but still in range
        void Partition_WithinRange_Test(bool expectedInRange, int partX, int partY, int partZ, int partWidth, int partHeight, int partDepth, int botX, int botY, int botZ, int botRange)
        {
            var partition = new Partition((partX, partY, partZ), (partWidth, partHeight, partDepth));
            (int x, int y, int z, int r) bot = (botX, botY, botZ, botRange);
            partition.WithinRange(bot).Should().Be(expectedInRange);
        }

        [Theory]
        [InlineData(0, 1, 1)]
        [InlineData(1, 0, 1)]
        [InlineData(1, 1, 0)]
        void Partition_ZeroSizeThrows(int width, int height, int depth)
        {
            Assert.Throws<Exception>(() => new Partition((0, 0, 0), (width, height, depth)));
        }

        [Fact]
        void Partition_CreateSubPartitions_ThrowsIfTooSmall()
        {
            var partition = new Partition((0, 0, 0), (1, 1, 1));
            var ex = Assert.Throws<Exception>(() => partition.CreateSubPartitions().First());
            ex.Message.Should().Be("Partition already at minimum size");
        }

        [Fact]
        void Partition_CreateSubPartitions_2x2x2()
        {
            var partition = new Partition((-1, -1, -1), (2, 2, 2));
            var subPartitons = partition.CreateSubPartitions().ToArray();
            subPartitons.Length.Should().Be(8);

            subPartitons[0].Pos.Should().Be((-1, -1, -1));
            subPartitons[0].Size.Should().Be((1, 1, 1));

            subPartitons[1].Pos.Should().Be((0, -1, -1));
            subPartitons[1].Size.Should().Be((1, 1, 1));

            subPartitons[2].Pos.Should().Be((-1, 0, -1));
            subPartitons[2].Size.Should().Be((1, 1, 1));

            subPartitons[3].Pos.Should().Be((0, 0, -1));
            subPartitons[3].Size.Should().Be((1, 1, 1));

            subPartitons[4].Pos.Should().Be((-1, -1, 0));
            subPartitons[4].Size.Should().Be((1, 1, 1));

            subPartitons[5].Pos.Should().Be((0, -1, 0));
            subPartitons[5].Size.Should().Be((1, 1, 1));

            subPartitons[6].Pos.Should().Be((-1, 0, 0));
            subPartitons[6].Size.Should().Be((1, 1, 1));

            subPartitons[7].Pos.Should().Be((0, 0, 0));
            subPartitons[7].Size.Should().Be((1, 1, 1));
        }

        [Fact]
        void Partition_CreateSubPartitions_4x6x8()
        {
            var partition = new Partition((5, 6, 0), (4, 6, 8));
            var subPartitons = partition.CreateSubPartitions().ToArray();
            subPartitons.Length.Should().Be(8);

            subPartitons[0].Pos.Should().Be((5, 6, 0));
            subPartitons[0].Size.Should().Be((2, 3, 4));

            subPartitons[1].Pos.Should().Be((7, 6, 0));
            subPartitons[1].Size.Should().Be((2, 3, 4));

            subPartitons[2].Pos.Should().Be((5, 9, 0));
            subPartitons[2].Size.Should().Be((2, 3, 4));

            subPartitons[3].Pos.Should().Be((7, 9, 0));
            subPartitons[3].Size.Should().Be((2, 3, 4));

            subPartitons[4].Pos.Should().Be((5, 6, 4));
            subPartitons[4].Size.Should().Be((2, 3, 4));

            subPartitons[5].Pos.Should().Be((7, 6, 4));
            subPartitons[5].Size.Should().Be((2, 3, 4));

            subPartitons[6].Pos.Should().Be((5, 9, 4));
            subPartitons[6].Size.Should().Be((2, 3, 4));

            subPartitons[7].Pos.Should().Be((7, 9, 4));
            subPartitons[7].Size.Should().Be((2, 3, 4));
        }

        [Fact]
        void Partition_CreateSubPartitions_3x3x3()
        {
            var partition = new Partition((0, 0, 0), (3, 3, 3));
            var subPartitons = partition.CreateSubPartitions().ToArray();
            subPartitons.Length.Should().Be(8);

            subPartitons[0].Pos.Should().Be((0, 0, 0));
            subPartitons[0].Size.Should().Be((1, 1, 1));

            subPartitons[1].Pos.Should().Be((1, 0, 0));
            subPartitons[1].Size.Should().Be((2, 1, 1));

            subPartitons[2].Pos.Should().Be((0, 1, 0));
            subPartitons[2].Size.Should().Be((1, 2, 1));

            subPartitons[3].Pos.Should().Be((1, 1, 0));
            subPartitons[3].Size.Should().Be((2, 2, 1));

            subPartitons[4].Pos.Should().Be((0, 0, 1));
            subPartitons[4].Size.Should().Be((1, 1, 2));

            subPartitons[5].Pos.Should().Be((1, 0, 1));
            subPartitons[5].Size.Should().Be((2, 1, 2));

            subPartitons[6].Pos.Should().Be((0, 1, 1));
            subPartitons[6].Size.Should().Be((1, 2, 2));

            subPartitons[7].Pos.Should().Be((1, 1, 1));
            subPartitons[7].Size.Should().Be((2, 2, 2));
        }

        [Fact]
        void Partition_CreateSubPartitions_1x1x3()
        {
            var partition = new Partition((5, 5, 5), (1, 1, 3));
            var subPartitons = partition.CreateSubPartitions().ToArray();
            subPartitons.Length.Should().Be(2);

            subPartitons[0].Pos.Should().Be((5, 5, 5));
            subPartitons[0].Size.Should().Be((1, 1, 1));

            subPartitons[1].Pos.Should().Be((5, 5, 6));
            subPartitons[1].Size.Should().Be((1, 1, 2));
        }

        [Fact]
        void Partition_CreateSubPartitions_5x4x1()
        {
            var partition = new Partition((5, 5, 5), (5, 4, 1));
            var subPartitons = partition.CreateSubPartitions().ToArray();
            subPartitons.Length.Should().Be(4);

            subPartitons[0].Pos.Should().Be((5, 5, 5));
            subPartitons[0].Size.Should().Be((2, 2, 1));

            subPartitons[1].Pos.Should().Be((7, 5, 5));
            subPartitons[1].Size.Should().Be((3, 2, 1));

            subPartitons[2].Pos.Should().Be((5, 7, 5));
            subPartitons[2].Size.Should().Be((2, 2, 1));

            subPartitons[3].Pos.Should().Be((7, 7, 5));
            subPartitons[3].Size.Should().Be((3, 2, 1));
        }

        [Fact]
        void Partition_AddBotIfWithinRange()
        {
            var partition = new Partition((2, 2, 0), (3, 2, 1));
            partition.AddBotIfWithinRange((0, 0, 0, 2));
            partition.AddBotIfWithinRange((3, 0, 0, 2));
            partition.AddBotIfWithinRange((3, 1, 0, 1));
            partition.AddBotIfWithinRange((5, 4, 0, 2));

            partition.Bots.Count.Should().Be(3);
        }

        IEnumerable<Partition> GetHighestPopulationSubPartitions(Partition partition)
        {

            var subPartitions = partition.CreateSubPartitionsAndPopulate().OrderByDescending(p => p.Bots.Count);

            var highest = subPartitions.First().Bots.Count;
            foreach (var subPartition in subPartitions)
            {
                if (subPartition.Bots.Count != highest) break;
                yield return subPartition;
            }
        }

        [Theory]
        [InlineData(36, "Data/Day23-Test2.txt")]
        [InlineData(124623002, "Data/Day23.txt")] // Solution
        void Problem2(int expectedAnswer, string inputFile)
        {
            var bots = FileIterator.Lines(inputFile).Select(l => ParseNode(l)).ToArray();
            var partition = BuildInitialPartition(bots);
            var frontier = new PriorityQueue<Partition>();

            // We want to explore the highest population partition in the hope we can reduce it down to a 1x1x1 size
            frontier.Enqueue(partition, bots.Length - partition.Bots.Count);

            var maxCount = int.MinValue;
            var winningestPartitions = new List<Partition>();

            while (frontier.Count != 0)
            {
                partition = frontier.Dequeue();
                if (partition.Bots.Count < maxCount) continue;

                foreach (var subPartition in GetHighestPopulationSubPartitions(partition))
                {
                    if (subPartition.Volume == 1)
                    {
                        if (maxCount < subPartition.Bots.Count)
                        {
                            maxCount = subPartition.Bots.Count;
                            winningestPartitions = new List<Partition>();
                            winningestPartitions.Add(subPartition);
                        }
                        else if (maxCount == subPartition.Bots.Count)
                        {
                            winningestPartitions.Add(subPartition);
                        }
                    }
                    else
                    {
                        frontier.Enqueue(subPartition, bots.Length - subPartition.Bots.Count);
                    }
                }
            }

            var minDistance = int.MaxValue;
            foreach (var part in winningestPartitions)
            {
                var distance = Math.Abs(part.Pos.x) + Math.Abs(part.Pos.y) + Math.Abs(part.Pos.z);
                if (distance < minDistance)
                    minDistance = distance;
            }

            minDistance.Should().Be(expectedAnswer);
        }
    }
}
