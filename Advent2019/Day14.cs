using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2019
{
    public class Day14
    {

        class Node
        {
            public readonly string Name;
            public readonly long OutputCount;
            public readonly (string name, long count)[] Inputs;

            public Node(string name, long outputCount, (string, long)[] inputs)
            {
                Name = name;
                OutputCount = outputCount;
                Inputs = inputs;
            }
        }

        static Dictionary<string, Node> LoadReactions(string filename)
        {
            Dictionary<string, Node> reactions = new();

            foreach (var line in FileIterator.Lines(filename))
            {
                var groups = line.Groups(@"(.+) => (\d+) ([A-Z]+)");
                var name = groups[2];
                var outputCount = long.Parse(groups[1]);
                List<(string, long)> inputs = new();
                foreach (var input in groups[0].Split(','))
                {
                    groups = input.Groups(@"(\d+) ([A-Z]+)");
                    inputs.Add((groups[1], long.Parse(groups[0])));
                }
                reactions[name] = new(name, outputCount, inputs.ToArray());
            }

            return reactions;
        }

        static long OreRequired(Dictionary<string, Node> reactions, Dictionary<string, long> overflow, string chemical, long required)
        {
            var node = reactions[chemical];
            if (required % node.OutputCount == 0) required /= node.OutputCount;
            else required = (required / node.OutputCount) + 1;

            var oreCount = 0L;
            foreach (var (name, count) in node.Inputs)
            {
                var totalRequired = count * required;
                if (name == "ORE")
                {
                    overflow.Sum(node.Name, node.OutputCount * required);
                    return totalRequired;
                }

                var numAvailable = overflow.GetOrDefault(name, 0);
                if (totalRequired <= numAvailable)
                {
                    overflow[name] = numAvailable - totalRequired;
                    continue;
                }

                oreCount += OreRequired(reactions, overflow, name, totalRequired - numAvailable);
                overflow.Sum(name, -totalRequired);
            }

            overflow.Sum(node.Name, node.OutputCount * required);
            return oreCount;
        }

        static long OreRequired(Dictionary<string, Node> reactions, long fuel)
        {
            Dictionary<string, long> overflow = new();
            return OreRequired(reactions, overflow, "FUEL", fuel);
        }

        static int CheckNumber(Dictionary<string, Node> reactions, long fuelCount)
        {
            if (OreRequired(reactions, fuelCount) > 1000000000000) return 1;
            if (OreRequired(reactions, fuelCount + 1) < 1000000000000) return -1;
            return 0;
        }

        [Theory]
        [InlineData("Data/Day14_Test1.txt", 31)]
        [InlineData("Data/Day14_Test2.txt", 165)]
        [InlineData("Data/Day14_Test3.txt", 13312)]
        [InlineData("Data/Day14_Test4.txt", 180697)]
        [InlineData("Data/Day14_Test5.txt", 2210736)]
        [InlineData("Data/Day14.txt", 431448)]
        public void Part1(string filename, long expectedAnswer)
        {
            var reactions = LoadReactions(filename);
            OreRequired(reactions, 1).Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day14_Test3.txt", 82892753)]
        [InlineData("Data/Day14_Test4.txt", 5586022)]
        [InlineData("Data/Day14_Test5.txt", 460664)]
        [InlineData("Data/Day14.txt", 3279311)]
        public void Part2(string filename, long expectedAnswer)
        {
            var reactions = LoadReactions(filename);

            long guessMin = 0;
            long guessMax = 100000000;
            long guess = guessMax / 2;

            while (true)
            {
                var result = CheckNumber(reactions, guess);
                if (result == 0) break;
                if (result > 0)
                {
                    guessMax = guess;
                }
                else
                {
                    guessMin = guess;
                }
                guess = (guessMin + guessMax) / 2;
            }
            guess.Should().Be(expectedAnswer);
        }
    }
}
