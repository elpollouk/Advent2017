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


            public static Dictionary<OpCodes, Action<int[], int, int, int>> s_OpCodeImplementations;

            static Cpu()
            {
                s_OpCodeImplementations = new Dictionary<OpCodes, Action<int[], int, int, int>>();
                s_OpCodeImplementations[OpCodes.ADDR] = (r, a, b, c) => r[c] = r[a] + r[b];
                s_OpCodeImplementations[OpCodes.ADDI] = (r, a, b, c) => r[c] = r[a] + b;
                s_OpCodeImplementations[OpCodes.MULR] = (r, a, b, c) => r[c] = r[a] * r[b];
                s_OpCodeImplementations[OpCodes.MULI] = (r, a, b, c) => r[c] = r[a] * b;
                s_OpCodeImplementations[OpCodes.BANR] = (r, a, b, c) => r[c] = r[a] & r[b];
                s_OpCodeImplementations[OpCodes.BANI] = (r, a, b, c) => r[c] = r[a] & b;
                s_OpCodeImplementations[OpCodes.BORR] = (r, a, b, c) => r[c] = r[a] | r[b];
                s_OpCodeImplementations[OpCodes.BORI] = (r, a, b, c) => r[c] = r[a] | b;
                s_OpCodeImplementations[OpCodes.SETR] = (r, a, b, c) => r[c] = r[a];
                s_OpCodeImplementations[OpCodes.SETI] = (r, a, b, c) => r[c] = a;
                s_OpCodeImplementations[OpCodes.GTIR] = (r, a, b, c) => r[c] = a > r[b] ? 1 : 0;
                s_OpCodeImplementations[OpCodes.GTRI] = (r, a, b, c) => r[c] = r[a] > b ? 1 : 0;
                s_OpCodeImplementations[OpCodes.GTRR] = (r, a, b, c) => r[c] = r[a] > r[b] ? 1 : 0;
                s_OpCodeImplementations[OpCodes.EQIR] = (r, a, b, c) => r[c] = a == r[b] ? 1 : 0;
                s_OpCodeImplementations[OpCodes.EQRI] = (r, a, b, c) => r[c] = r[a] == b ? 1 : 0;
                s_OpCodeImplementations[OpCodes.EQRR] = (r, a, b, c) => r[c] = r[a] == r[b] ? 1 : 0;
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

        [Fact]
        void Problem1()
        {
            var lines = FileIterator.LoadLines<string>("Data/Day16-1.txt");
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

            var matchCounts = new int[observations.Count];
            for (var i = 0; i < observations.Count; i++)
            {
                var obs = observations[i];
                foreach (var opcode in Cpu.s_OpCodeImplementations.Values)
                {
                    var registers = (int[])obs.Before.Clone();
                    opcode(registers, obs.Action[1], obs.Action[2], obs.Action[3]);
                    if (ArraysMatch(registers, obs.After))
                        matchCounts[i]++;
                }
            }

            var multipleMatched = matchCounts.Where(c => c >= 3).Count();
            multipleMatched.Should().Be(509); // Problem 1 solution
        }
    }
}
