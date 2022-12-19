using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Utils;
using Utils.Alogrithms;
using Xunit;

namespace Advent2022
{
    public class Day16
    {
        class Valve
        {
            public readonly string Id;
            public readonly int Flag;
            public readonly int Rate;
            public readonly List<Valve> Links = new();
            public readonly Dictionary<Valve, int> DistanceTo = new();
            public bool IsOpen = false;

            public Valve(string id, int flag, int rate)
            {
                Id = id;
                Flag = flag;
                Rate = rate;
            }

            public override string ToString()
            {
                var s = $"{Id}: Rate={Rate}, Links: ";
                bool first = true;
                foreach (var link in Links)
                {
                    if (!first)
                    {
                        s += ", ";
                    }
                    else
                    {
                        first = false;
                    }
                    s += link.Id;
                }
                return s;
            }
        }

        class ValveGraphAdapter : Astar.IGraphAdapter<Valve>
        {
            public IEnumerable<Valve> GetLinked(Valve node)
            {
                return node.Links;
            }

            public int GetMoveCost(Valve from, Valve to)
            {
                return 1;
            }

            public int GetScore(Valve from, Valve to)
            {
                return 1;
            }
        }

        Dictionary<string, Valve> ParseValves(string filename)
        {
            Dictionary<string, Valve> valves = new();
            Dictionary<string, string[]> tempLinks = new();

            int flag = 0;
            foreach (var line in FileIterator.Lines(filename))
            {
                var groups = line.Groups(@"Valve (.+) has flow rate=(\d+)\; tunnels? leads? to valves? (.+)");
                Valve valve = new(groups[0], flag++, int.Parse(groups[1]));
                valves[valve.Id] = valve;
                tempLinks[valve.Id] = groups[2].Split(", ");
            }

            foreach (var (id, links) in tempLinks)
            {
                var valve = valves[id];
                foreach (var link in links)
                {
                    valve.Links.Add(valves[link]);
                }
            }

            // Pre-cache distances
            foreach (var src in valves.Values)
            {
                foreach (var dst in valves.Values)
                {
                    if (src == dst) continue;
                    src.DistanceTo[dst] = Astar.FindPath(adapter, src, dst).Count - 1;
                }
            }

            return valves;
        }

        readonly Dictionary<(long, Valve, int), long> cache = new();
        readonly ValveGraphAdapter adapter = new();

        IEnumerable<Valve> unopened(Dictionary<string, Valve> valves)
        {
            foreach (var valve in valves.Values)
            {
                if (valve.Rate != 0 && !valve.IsOpen)
                {
                    yield return valve;
                }
            }
        }

        long FindMax(Dictionary<string, Valve> valves, long openValves, Valve valve, int timeRemaining)
        {
            if (timeRemaining <= 0)
                return 0;

            if (cache.TryGetValue((openValves, valve, timeRemaining), out long score))
            {
                return score;
            }

            long myScore = (valve.Rate * timeRemaining);
            long maxScore = 0;

            if (valve.Rate != 0) // Special case for starting valve
            {
                timeRemaining--;
                valve.IsOpen = true;
                openValves.SetFlag(valve.Flag);
                myScore = (valve.Rate * timeRemaining);
            }

            foreach (var link in unopened(valves))
            {
                var distance = valve.DistanceTo[link];
                score = FindMax(valves, openValves, link, timeRemaining - distance);
                if (maxScore < score)
                {
                    maxScore = score;
                }
            }
            
            valve.IsOpen = false;
            openValves.ClearFlag(valve.Flag);
            score = myScore + maxScore;

            cache[(openValves, valve, timeRemaining)] = score;

            return score;
        }

        [Theory]
        [InlineData("Data/Day16_Test.txt", 1651)]
        [InlineData("Data/Day16.txt", 1940)]
        public void Part1(string filename, long expectedAnswer)
        {
            cache.Clear();
            var valves = ParseValves(filename);
            var max = FindMax(valves, 0, valves["AA"], 30);
            max.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day16_Test.txt", 1707)]
        [InlineData("Data/Day16.txt", 2469)]
        public void Part2(string filename, long expectedAnswer)
        {
            var valves = ParseValves(filename);
            var availableValves = valves.Values.Where(v => v.Rate > 0).ToArray();

            // Reduce the search space slightly by rounding up odd sized lengths
            var length = availableValves.Length;
            if ((length & 1) != 0) length++;

            long max = 0;
            foreach (var combination in availableValves.Combinations(length / 2))
            {
                cache.Clear();
                foreach (var valve in availableValves)
                {
                    valve.IsOpen = combination.Contains(valve);
                }
                long a = FindMax(valves, 0, valves["AA"], 26);

                cache.Clear();
                foreach (var valve in availableValves)
                {
                    valve.IsOpen = !combination.Contains(valve);
                }
                long b = FindMax(valves, 0, valves["AA"], 26);

                if (max < a + b)
                {
                    max = a + b;
                }
            }

            max.Should().Be(expectedAnswer);
        }
    }
}
