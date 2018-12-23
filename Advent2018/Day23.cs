using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Xunit;

namespace Advent2018
{
    public class Day23
    {
        class Partition
        {
            public readonly (int x, int y, int z) Pos;
            public readonly (int width, int height, int depth) Size;

            public Partition((int x, int y, int z) pos, (int width, int height, int depth) size)
            {
                Pos = pos;
                Size = size;
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

            IEnumerable<(int x, int y, int z)> GetPartitionExtents()
            {
                yield return Pos;
                yield return (Pos.x + Size.width - 1, Pos.y, Pos.z);
                yield return (Pos.x, Pos.y + Size.height -1, Pos.z);
                yield return (Pos.x + Size.width - 1, Pos.y + Size.height - 1, Pos.z);
                yield return (Pos.x, Pos.y, Pos.z + Size.depth);
                yield return (Pos.x + Size.width - 1, Pos.y, Pos.z + Size.depth - 1);
                yield return (Pos.x, Pos.y + Size.height - 1, Pos.z + Size.depth - 1);
                yield return (Pos.x + Size.width - 1, Pos.y + Size.height - 1, Pos.z + Size.depth - 1);
            }

            public bool ContainsPoint(int x, int y, int z)
            {
                return (Pos.x <= x) && (x < Pos.x + Size.width)
                     && (Pos.y <= y) && (y < Pos.y + Size.height)
                     && (Pos.z <= z) && (z < Pos.z + Size.depth);
            }

            private bool WithinBotRange(int x, int y, int z, (int x, int y, int z, int r) bot)
            {
                var distance = Math.Abs(x - bot.x) + Math.Abs(y - bot.y) + Math.Abs(z - bot.z);
                return distance <= bot.r;
            }

            public bool WithinRange((int x, int y, int z, int r) bot)
            {
                if (ContainsPoint(bot.x, bot.y, bot.z))
                    return true;

                // Check if the range extents of the bot fall within the partition
                foreach (var (x, y, z) in GetBotExtents(bot))
                    if (ContainsPoint(x, y, z))
                        return true;

                // Check if the extents of this partition fall within range of the bot
                foreach (var (x, y, z) in GetPartitionExtents())
                    if (WithinBotRange(x, y, z, bot))
                        return true;

                return false;
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

        ((int minX, int minY, int minZ), (int maxX, int maxY, int maxZ)) BuildProblemSpace((int x, int y, int z, int r)[] bots)
        {
            var minX = int.MaxValue;
            var minY = int.MaxValue;
            var minZ = int.MaxValue;
            var maxX = int.MaxValue;
            var maxY = int.MaxValue;
            var maxZ = int.MaxValue;

            foreach (var (x, y, z, r) in bots)
            {
                minX = Math.Min(minX, x);
                maxX = Math.Max(maxX, x);
                minY = Math.Min(minY, y);
                maxY = Math.Max(maxY, y);
                minZ = Math.Min(minZ, z);
                maxZ = Math.Max(maxZ, z);
            }

            return (
                (minX, minY, minZ),
                (maxX, maxY, maxZ)
            );
        }

        bool WithinRange((int x, int y, int z, int r) sourceBot, (int x, int y, int z, int r) targetBot)
        {
            var distance = Math.Abs(sourceBot.x - targetBot.x) + Math.Abs(sourceBot.y - targetBot.y) + Math.Abs(sourceBot.z - targetBot.z);
            return distance <= sourceBot.r;
        }

        [Theory]
        [InlineData(7, "Data/Day23-Test.txt")]
        [InlineData(248, "Data/Day23.txt")]
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
        void Parition_WithinRange_Test(bool expectedInRange, int partX, int partY, int partZ, int partWidth, int partHeight, int partDepth, int botX, int botY, int botZ, int botRange)
        {
            var partition = new Partition((partX, partY, partZ), (partWidth, partHeight, partDepth));
            (int x, int y, int z, int r) bot = (botX, botY, botZ, botRange);
            partition.WithinRange(bot).Should().Be(expectedInRange);
        }
    }
}
