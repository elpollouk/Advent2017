using FluentAssertions;
using System.Linq;
using Utils;
using Utils.DataStructures;
using Xunit;

namespace Advent2023
{
    public class Day08
    {
        const int LEFT = 0;
        const int RIGHT = 1;

        (Graph<string> graph, string steps) Parse(string filename)
        {
            var reader = FileIterator.CreateLineReader(filename);
            var steps = reader();
            reader();

            Graph<string> graph = new();

            var line = reader();
            while (line != null)
            {
                var match = line.Match(@"^([\dA-Z]+) = \(([\dA-Z]+), ([\dA-Z]+)\)$");
                var from = match.Groups[1].Value;
                var left = match.Groups[2].Value;
                var right = match.Groups[3].Value;

                graph.AddNodeIfNotInGraph(from);
                graph.AddNodeIfNotInGraph(left);
                graph.AddNodeIfNotInGraph(right);

                graph.AddOneWayLink(from, left);
                graph.AddOneWayLink(from, right);

                line = reader();
            }

            return (graph, steps);
        }

        [Theory]
        [InlineData("Data/Day08_Test1.txt", 2)]
        [InlineData("Data/Day08_Test2.txt", 6)]
        [InlineData("Data/Day08.txt", 21251)]
        public void Part1(string filename, long expectedAnswer)
        {
            var (graph, steps) = Parse(filename);
            var stepCycler = steps.CreateCycler();
            var pos = "AAA";
            long count = 0;
            while (pos != "ZZZ")
            {
                var move = stepCycler() == 'L' ? LEFT : RIGHT;
                if (graph.GetNumLinks(pos) == 1) move = LEFT;
                pos = graph.GetLinkedItem(pos, move);
                count++;
            }

            count.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day08.txt", 11678319315857L)]
        public void Part2(string filename, long expectedAnswer)
        {
            var (graph, steps) = Parse(filename);
            var starts = graph.Items.Where(i => i[2] == 'A').ToArray();
            var cycles = new long[starts.Length];

            // Get the cycle lengths
            for (int i = 0; i < starts.Length; i++)
            {
                var stepCycler = steps.CreateCycler();
                var pos = starts[i];
                long count = 0;
                while (pos[2] != 'Z')
                {
                    var move = stepCycler() == 'L' ? LEFT : RIGHT;
                    if (graph.GetNumLinks(pos) == 1) move = LEFT;
                    pos = graph.GetLinkedItem(pos, move);
                    count++;
                }

                cycles[i] = count;
            }

            cycles = [..cycles.OrderByDescending(v => v)];
            var stepSize = cycles[0];
            for (int i = 1; i < cycles.Length; i++)
            {
                // We want to find the next number that is a common multiple of the current cycle time and all the previous cycle times
                var proposedStepSize = stepSize;
                while (proposedStepSize % cycles[i] != 0)
                    proposedStepSize += stepSize;
                // Once found, it becomes our new step size to use when searching for the next common multiplier
                stepSize = proposedStepSize;
            }
            stepSize.Should().Be(expectedAnswer);
        }
    }
}
