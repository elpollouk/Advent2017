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
    }
}
