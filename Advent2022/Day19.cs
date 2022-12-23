using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2022
{
    public class Day19
    {
        class State
        {
            public static int TIME_LIMIT = 24;

            public readonly int Id;
            public int Time = 0;

            public readonly int OreRobotCost; // Cost in ore
            public readonly int ClayRobotCost; // Cost in ore
            public readonly (int ore, int clay) ObsidianRobotCost;
            public readonly (int ore, int obsidian) GeodeRobotCost;

            public readonly int MaxOreUsage;

            int OreCount;
            int ClayCount;
            int ObsidianCount;
            int GeodeCount;

            int OreDelta = 1;
            int ClayDelta;
            int ObsidianDelta;
            int GeodeDelta;

            public long MaxGeodes;

            static int TimeRequired(int target, int delta)
            {
                var t = target / delta;
                if (target % delta != 0) t++;
                return t;
            }

            public State(string line)
            {
                var groups = line.Groups(@"Blueprint (\d+): Each ore robot costs (\d+) ore. Each clay robot costs (\d+) ore. Each obsidian robot costs (\d+) ore and (\d+) clay. Each geode robot costs (\d+) ore and (\d+) obsidian.");
                Id = int.Parse(groups[0]);
                OreRobotCost = int.Parse(groups[1]);
                ClayRobotCost = int.Parse(groups[2]);
                ObsidianRobotCost = (int.Parse(groups[3]), int.Parse(groups[4]));
                GeodeRobotCost = (int.Parse(groups[5]), int.Parse(groups[6]));

                MaxOreUsage = OreRobotCost;
                if (MaxOreUsage < ClayRobotCost) MaxOreUsage = ClayRobotCost;
                if (MaxOreUsage < ObsidianRobotCost.ore) MaxOreUsage = ObsidianRobotCost.ore;
                if (MaxOreUsage < GeodeRobotCost.ore) MaxOreUsage = GeodeRobotCost.ore;
            }

            private State(State other)
            {
                Id = other.Id;
                Time = other.Time;
                OreRobotCost = other.OreRobotCost;
                ClayRobotCost = other.ClayRobotCost;
                ObsidianRobotCost = other.ObsidianRobotCost;
                GeodeRobotCost = other.GeodeRobotCost;
                MaxOreUsage = other.MaxOreUsage;
                OreCount = other.OreCount;
                ClayCount = other.ClayCount;
                ObsidianCount = other.ObsidianCount;
                GeodeCount = other.GeodeCount;
                OreDelta = other.OreDelta;
                ClayDelta = other.ClayDelta;
                ObsidianDelta = other.ObsidianDelta;
                GeodeDelta = other.GeodeDelta;
                MaxGeodes = other.MaxGeodes;
            }

            public State Clone()
            {
                return new(this);
            }

            public long Solve()
            {
                var remainingTime = TIME_LIMIT - Time;
                if (remainingTime != 0)
                {
                    int maxPotentialGeodes = GeodeCount + (remainingTime * GeodeDelta);
                    maxPotentialGeodes += (remainingTime * (remainingTime + 1)) / 2;
                    if (maxPotentialGeodes < MaxGeodes)
                    {
                        return MaxGeodes* Id;
                    }

                    bool acted = false;
                    acted |= SolveFor(remainingTime, NextOreRobotTime, s => s.BuildOreRobot());
                    acted |= SolveFor(remainingTime, NextClayRobotTime, s => s.BuildClayRobot());
                    acted |= SolveFor(remainingTime, NextObsidianRobotTime, s => s.BuildObsidianRobot());
                    acted |= SolveFor(remainingTime, NextGeodeRobotTime, s => s.BuildGeodeRobot());

                    if (!acted) AdvanceTime(remainingTime);
                }
                return MaxGeodes * Id;
            }

            private bool SolveFor(int remainingTime, Func<int, int> nextTime, Action<State> build)
            {
                var t = nextTime(remainingTime);
                if (t == -1) return false;
                if (Time + t >= TIME_LIMIT) return false;

                var s = Clone();
                s.AdvanceTime(t);
                build(s);
                s.Solve();
                if (s.MaxGeodes > MaxGeodes) MaxGeodes = s.MaxGeodes;

                return true;
            }

            public void AdvanceTime(int minutes)
            {
                OreCount += OreDelta * minutes;
                ClayCount += ClayDelta * minutes;
                ObsidianCount += ObsidianDelta * minutes;
                GeodeCount += GeodeDelta * minutes;
                if (MaxGeodes < GeodeCount) MaxGeodes = GeodeCount;
                Time += minutes;
            }

            public int NextOreRobotTime(int remainingTime)
            {
                if (OreDelta >= MaxOreUsage) return -1;
                if (remainingTime * MaxOreUsage < OreCount + remainingTime * OreDelta) return -1;
                if (OreCount >= OreRobotCost) return 0;
                int t = TimeRequired(OreRobotCost - OreCount, OreDelta);
                return t;
            }

            public int NextClayRobotTime(int remainingTime)
            {
                if (remainingTime * ObsidianRobotCost.clay < ClayCount + remainingTime * ClayDelta) return -1;
                if (ClayDelta >= ObsidianRobotCost.clay) return -1;
                if (OreCount >= ClayRobotCost) return 0;
                int t = TimeRequired(ClayRobotCost - OreCount, OreDelta);
                return t;
            }

            public int NextObsidianRobotTime(int remainingTime)
            {
                if (ClayDelta == 0) return -1;
                if (remainingTime * GeodeRobotCost.obsidian < ObsidianCount + remainingTime * ObsidianDelta) return -1;
                if (ObsidianDelta >= GeodeRobotCost.obsidian) return -1;
                if (OreCount >= ObsidianRobotCost.ore && ClayCount >= ObsidianRobotCost.clay) return 0;
                int t = TimeRequired(ObsidianRobotCost.ore - OreCount, OreDelta);
                int t2 = TimeRequired(ObsidianRobotCost.clay - ClayCount, ClayDelta);
                if (t < t2) t = t2;
                return t;
            }

            public int NextGeodeRobotTime(int remainingTime)
            {
                if (ObsidianDelta == 0) return -1;
                if (OreCount >= GeodeRobotCost.ore && ObsidianCount >= GeodeRobotCost.obsidian) return 0;
                int t = TimeRequired(GeodeRobotCost.ore - OreCount, OreDelta);
                int t2 = TimeRequired(GeodeRobotCost.obsidian - ObsidianCount, ObsidianDelta);
                if (t < t2) t = t2;
                return t;
            }

            public void BuildOreRobot()
            {
                AdvanceTime(1);
                OreDelta++;
                OreCount -= OreRobotCost;
            }

            public void BuildClayRobot()
            {
                AdvanceTime(1);
                ClayDelta++;
                OreCount -= ClayRobotCost;
            }

            public void BuildObsidianRobot()
            {
                AdvanceTime(1);
                ObsidianDelta++;
                OreCount -= ObsidianRobotCost.ore;
                ClayCount -= ObsidianRobotCost.clay;
            }

            public void BuildGeodeRobot()
            {
                AdvanceTime(1);
                GeodeDelta++;
                OreCount -= GeodeRobotCost.ore;
                ObsidianCount -= GeodeRobotCost.obsidian;
            }

            public override string ToString()
            {
                return $"T={Time}: Ore={OreCount}(+{OreDelta}), Clay={ClayCount}(+{ClayDelta}), Obsidian={ObsidianCount}(+{ObsidianDelta}), Geode={GeodeCount}(+{GeodeDelta})";
            }
        }

        [Theory]
        [InlineData("Data/Day19_Test.txt", 33)]
        [InlineData("Data/Day19.txt", 978)]
        public void Part1(string filename, long expectedAnswer)
        {
            State.TIME_LIMIT = 24;
            var states = new List<State>();
            foreach (var line in FileIterator.Lines(filename))
            {
                states.Add(new(line));
            }

            var result = states.Select(s => s.Solve()).Sum();
            result.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day19_Test.txt", 56 * 62)]
        [InlineData("Data/Day19.txt", 15939)]
        public void Part2(string filename, long expectedAnswer)
        {
            State.TIME_LIMIT = 32;
            var states = new List<State>();
            foreach (var line in FileIterator.Lines(filename).Take(3))
            {
                states.Add(new(line));
            }

            long total = 1;
            foreach (var state in states)
            {
                state.Solve();
                total *= state.MaxGeodes;
            }
            total.Should().Be(expectedAnswer);
        }
    }
}
