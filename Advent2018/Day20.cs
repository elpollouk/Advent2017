using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Utils.DataStructures;
using Xunit;

namespace Advent2018
{
    public class Day20
    {
        struct PathStep
        {
            public static readonly PathStep MaxDistance = new PathStep(int.MaxValue, (0, 0));

            public readonly int DistanceFromStart;
            public readonly (int x, int y) Pos;

            public PathStep(int distance, (int x, int y) pos)
            {
                DistanceFromStart = distance;
                Pos = pos;
            }

            public bool IsQuickerThan(PathStep other) => DistanceFromStart < other.DistanceFromStart;
        }

        void Parse(int x, int y, string input, ref int readPos, Graph<(int x, int y)> graph)
        {
            var originalX = x;
            var originalY = y;
            while (readPos < input.Length)
            {
                switch (input[readPos++])
                {
                    case 'N':
                        graph.AddNodeIfNotInGraph((x, y - 1));
                        graph.AddTwoWayLink((x, y), (x, y - 1));
                        y--;
                        break;

                    case 'S':
                        graph.AddNodeIfNotInGraph((x, y + 1));
                        graph.AddTwoWayLink((x, y), (x, y + 1));
                        y++;
                        break;

                    case 'E':
                        graph.AddNodeIfNotInGraph((x + 1, y));
                        graph.AddTwoWayLink((x, y), (x + 1, y));
                        x++;
                        break;

                    case 'W':
                        graph.AddNodeIfNotInGraph((x - 1, y));
                        graph.AddTwoWayLink((x, y), (x - 1, y));
                        x--;
                        break;

                    case '(':
                        Parse(x, y, input, ref readPos, graph);
                        break;

                    case ')':
                        return;

                    case '|':
                        x = originalX;
                        y = originalY;
                        break;

                    default:
                        Oh.Bugger();
                        break;
                }
            }
        }

        Graph<(int x, int y)> BuildGraph(string input)
        {
            var graph = new Graph<(int x, int y)>();
            input = input.Substring(1, input.Length - 2); // Trim off ^ and $

            graph.AddNode((0, 0));
            var readpos = 0;
            Parse(0, 0, input, ref readpos, graph);

            return graph;
        }

        IEnumerable<PathStep> GetAreasAboveDoorCount(int count, Graph<(int x, int y)> graph)
        {
            var pathMap = new Dictionary<(int x, int y), PathStep>();
            var frontier = new Queue<PathStep>();
            frontier.Enqueue(new PathStep(0, (0, 0)));

            while (frontier.Count != 0)
            {
                var step = frontier.Dequeue();

                var existingStep = pathMap.GetOrDefault(step.Pos, PathStep.MaxDistance);

                if (step.IsQuickerThan(existingStep))
                {
                    pathMap[step.Pos] = step;

                    foreach (var linkedArea in graph.GetLinked(step.Pos))
                        frontier.Enqueue(new PathStep(step.DistanceFromStart + 1, linkedArea));
                }
            }

            return pathMap.Values.Where(s => s.DistanceFromStart >= count);
        }

        [Theory]
        [InlineData(3, "^WNE$")]
        [InlineData(10, "^ENWWW(NEEE|SSE(EE|N))$")]
        [InlineData(18, "^ENNWSWW(NEWS|)SSSEEN(WNSE|)EE(SWEN|)NNN$")]
        [InlineData(23, "^ESSWWN(E|NNENN(EESS(WNSE|)SSS|WWWSSSSE(SW|NNNE)))$")]
        [InlineData(31, "^WSSEESWWWNW(S|NENNEEEENN(ESSSSW(NWSW|SSEN)|WSWWN(E|WWS(E|SS))))$")]
        void Problem1_Test(int expectedDoorCount, string input)
        {
            var graph = BuildGraph(input);
            GetAreasAboveDoorCount(expectedDoorCount, graph).Select(s => s.DistanceFromStart)
                                                            .First().Should().Be(expectedDoorCount);
        }

        [Fact]
        void Problem1_Solution()
        {
            var input = FileIterator.Lines("Data/Day20.txt").First();
            Problem1_Test(3476, input);
        }

        [Fact]
        void Problem2_Solution()
        {
            var input = FileIterator.Lines("Data/Day20.txt").First();
            var graph = BuildGraph(input);
            GetAreasAboveDoorCount(1000, graph).Count().Should().Be(8514);
        }
    }
}
