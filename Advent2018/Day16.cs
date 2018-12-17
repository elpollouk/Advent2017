using FluentAssertions;
using FluentAssertions.Equivalency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;
using Xunit;

namespace Advent2018
{
    public class Day16
    {
        enum OpCodes
        {
            ADDR,
            ADDI,
            MULR,
            MULI,
            BANR,
            BANI,
            BORR,
            BORI,
            SETR,
            SETI,
            GTIR,
            GTRI,
            GTRR,
            EQIR,
            EQRI,
            EQRR
        }

        class Cpu
        {
            public int[] registers = new int[4];

            public void Execute((OpCodes opcode, int a, int b, int c)[] program)
            {
                foreach (var (opcode, a, b, c) in program)
                {
                    s_OpCodeImplementations[opcode](registers, a, b, c);
                }
            }

            public static Dictionary<OpCodes, Action<int[], int, int, int>> s_OpCodeImplementations;

            static Cpu()
            {
                s_OpCodeImplementations = new Dictionary<OpCodes, Action<int[], int, int, int>>
                {
                    [OpCodes.ADDR] = (r, a, b, c) => r[c] = r[a] + r[b],
                    [OpCodes.ADDI] = (r, a, b, c) => r[c] = r[a] + b,
                    [OpCodes.MULR] = (r, a, b, c) => r[c] = r[a] * r[b],
                    [OpCodes.MULI] = (r, a, b, c) => r[c] = r[a] * b,
                    [OpCodes.BANR] = (r, a, b, c) => r[c] = r[a] & r[b],
                    [OpCodes.BANI] = (r, a, b, c) => r[c] = r[a] & b,
                    [OpCodes.BORR] = (r, a, b, c) => r[c] = r[a] | r[b],
                    [OpCodes.BORI] = (r, a, b, c) => r[c] = r[a] | b,
                    [OpCodes.SETR] = (r, a, b, c) => r[c] = r[a],
                    [OpCodes.SETI] = (r, a, b, c) => r[c] = a,
                    [OpCodes.GTIR] = (r, a, b, c) => r[c] = a > r[b] ? 1 : 0,
                    [OpCodes.GTRI] = (r, a, b, c) => r[c] = r[a] > b ? 1 : 0,
                    [OpCodes.GTRR] = (r, a, b, c) => r[c] = r[a] > r[b] ? 1 : 0,
                    [OpCodes.EQIR] = (r, a, b, c) => r[c] = a == r[b] ? 1 : 0,
                    [OpCodes.EQRI] = (r, a, b, c) => r[c] = r[a] == b ? 1 : 0,
                    [OpCodes.EQRR] = (r, a, b, c) => r[c] = r[a] == r[b] ? 1 : 0
                };
            }
        }

        [Theory]
        [InlineData(OpCodes.GTIR, 0, 1, 2, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 })]
        [InlineData(OpCodes.GTIR, 1, 2, 2, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 1, 0 })]
        [InlineData(OpCodes.BORI, 1, 2, 0, new int[] { 4, 1, 0, 0 }, new int[] { 3, 1, 0, 0 })]
        [InlineData(OpCodes.BANR, 3, 2, 0, new int[] { 7, 0, 3, 6 }, new int[] { 2, 0, 3, 6 })]
        void OpCode_Test(OpCodes opcode, int a, int b, int c, int[] registers, int[] expected)
        {
            Cpu.s_OpCodeImplementations[opcode](registers, a, b, c);
            registers.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        class Observation
        {
            public int[] Before;
            public int[] Action;
            public int[] After;
        }

        int[] MatchToArray(Match match)
        {
            var r = new int[4];
            for (var i = 0; i < 4; i++)
                r[i] = int.Parse(match.Groups[1 + i].Value);
            return r;
        }

        bool ArraysMatch(int[] a, int[] b)
        {
            if (a.Length != b.Length)
                return false;

            for (var i = 0; i < a.Length; i++)
                if (a[i] != b[i])
                    return false;

            return true;
        }

        [Fact]
        void Problem1_Test()
        {
            var obs = new Observation
            {
                Before = new int[] { 3, 2, 1, 1 },
                Action = new int[] { 9, 2, 1, 2 },
                After = new int[] { 3, 2, 2, 1 }
            };

            var count = 0;
            foreach (var opcode in Cpu.s_OpCodeImplementations.Values)
            {
                var registers = (int[])obs.Before.Clone();
                opcode(registers, obs.Action[1], obs.Action[2], obs.Action[3]);
                if (ArraysMatch(registers, obs.After))
                    count++;
            }

            count.Should().Be(3);
        }

        HashSet<OpCodes>[] CountMatches(List<Observation> observations)
        {
            var matchCounts = new HashSet<OpCodes>[observations.Count];
            for (var i = 0; i < matchCounts.Length; i++)
                matchCounts[i] = new HashSet<OpCodes>();

            for (var i = 0; i < observations.Count; i++)
            {
                var obs = observations[i];
                foreach (var opcode in Enum.GetValues(typeof(OpCodes)).Cast<OpCodes>())
                {
                    var registers = (int[])obs.Before.Clone();
                    Cpu.s_OpCodeImplementations[opcode](registers, obs.Action[1], obs.Action[2], obs.Action[3]);
                    if (ArraysMatch(registers, obs.After))
                        matchCounts[i].Add(opcode);
                }
            }

            return matchCounts;
        }

        List<Observation> LoadObservations(string filename)
        {
            var lines = FileIterator.LoadLines<string>(filename);
            var observations = new List<Observation>();

            var currentLine = 0;
            while (currentLine < lines.Length)
            {
                var obs = new Observation
                {
                    Before = MatchToArray(lines[currentLine++].Match(@"\[(\d+), (\d+), (\d+), (\d+)\]")),
                    Action = MatchToArray(lines[currentLine++].Match(@"(\d+) (\d+) (\d+) (\d+)")),
                    After = MatchToArray(lines[currentLine++].Match(@"\[(\d+), (\d+), (\d+), (\d+)\]")),
                };
                observations.Add(obs);
                currentLine++;
            }

            return observations;
        }

        (OpCodes opcode, int a, int b, int c)[] LoadProgram(string filename, Dictionary<int, OpCodes> knownCodes)
        {
            var lines = FileIterator.LoadLines<string>(filename);
            var program = new (OpCodes opcode, int a, int b, int c)[lines.Length];

            for (var i = 0; i < lines.Length; i++)
            {
                var match = lines[i].Match(@"(\d+) (\d+) (\d+) (\d+)");
                program[i].opcode = knownCodes[int.Parse(match.Groups[1].Value)];
                program[i].a = int.Parse(match.Groups[2].Value);
                program[i].b = int.Parse(match.Groups[3].Value);
                program[i].c = int.Parse(match.Groups[4].Value);
            }

            return program;
        }

        [Fact]
        void Problem1()
        {
            var observations = LoadObservations("Data/Day16-1.txt");
            var matchCounts = CountMatches(observations);

            var multipleMatched = matchCounts.Where(c => c.Count >= 3).Count();
            multipleMatched.Should().Be(509); // Problem 1 solution
        }

        [Fact]
        void Problem2()
        {
            var observations = LoadObservations("Data/Day16-1.txt");
            var matchedOpCodes = CountMatches(observations);
            var knownCodes = new Dictionary<int, OpCodes>();
            var knownCodesReverse = new Dictionary<OpCodes, int>();

            while (knownCodes.Count != 16)
            {
                for (var i = 0; i < matchedOpCodes.Length; i++)
                {
                    var observationOpCode = observations[i].Action[0];

                    if (matchedOpCodes[i].Count == 1)
                    {
                        knownCodes[observationOpCode] = matchedOpCodes[i].First();
                        knownCodesReverse[matchedOpCodes[i].First()] = observationOpCode;
                    }
                    else
                    {
                        var removeList = new List<OpCodes>();
                        foreach (var opcode in matchedOpCodes[i])
                            if (knownCodesReverse.TryGetValue(opcode, out int value))
                                if (observationOpCode != value)
                                    removeList.Add(opcode);

                        foreach (var opcode in removeList)
                            matchedOpCodes[i].Remove(opcode);
                    }
                }
            }

            var program = LoadProgram("Data/Day16-2.txt", knownCodes);
            var cpu = new Cpu();
            cpu.Execute(program);
            cpu.registers[0].Should().Be(496);
        }
    }
}
