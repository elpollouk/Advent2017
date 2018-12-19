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
    public class Day19
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
            public int IPR = 0;
            public int[] registers = new int[6];

            public void Execute((OpCodes opcode, int a, int b, int c)[] program)
            {
                var ip = 0;
                while (ip < program.Length)
                {
                    registers[IPR] = ip;
                    var (opcode, a, b, c) = program[ip];
                    OpCodeImplementations[opcode](registers, a, b, c);
                    ip = registers[IPR];
                    ip++;
                }
            }

            public static Dictionary<OpCodes, Action<int[], int, int, int>> OpCodeImplementations;

            static Cpu()
            {
                OpCodeImplementations = new Dictionary<OpCodes, Action<int[], int, int, int>>
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

        (OpCodes opcode, int a, int b, int c)[] LoadProgram(string filename, out int ipr)
        {
            var lines = FileIterator.LoadLines<string>(filename);
            var program = new (OpCodes opcode, int a, int b, int c)[lines.Length - 1];

            ipr = int.Parse(lines[0].Match(@"#ip (\d)").Groups[1].Value);

            for (var i = 1; i < lines.Length; i++)
            {
                var match = lines[i].Match(@"(.+) (\d+) (\d+) (\d+)");
                program[i - 1].opcode = (OpCodes)Enum.Parse(typeof(OpCodes), match.Groups[1].Value.ToUpper());
                program[i - 1].a = int.Parse(match.Groups[2].Value);
                program[i - 1].b = int.Parse(match.Groups[3].Value);
                program[i - 1].c = int.Parse(match.Groups[4].Value);
            }

            return program;
        }

        [Theory]
        [InlineData(6, 0, "Data/Day19-Test.txt")]
        [InlineData(3224, 0, "Data/Day19.txt")] // Solution
        void Problem1(int expectedValue, int register, string inputFile)
        {
            var program = LoadProgram(inputFile, out int ipr);
            var cpu = new Cpu
            {
                IPR = ipr
            };
            cpu.Execute(program);

            cpu.registers[register].Should().Be(expectedValue);
        }

        //[Fact]
        void Problem2()
        {
            var program = LoadProgram("Data/Day19.txt", out int ipr);
            var cpu = new Cpu
            {
                IPR = ipr
            };
            cpu.registers[0] = 1;
            cpu.Execute(program);

            cpu.registers[0].Should().Be(0);
        }

        [Theory]
        [InlineData(false, 3224)]
        void ProblemCompiled(bool hardMode, int expectedAnswer)
        {
            var total = 0;
            var target = hardMode ? 10551408 : 1008;

            for (var i = 1; i <= target; i++)
                for (var j = 1; j <= target; j++)
                    if (i * j == target)
                        total += i;

            total.Should().Be(expectedAnswer);
        }
    }
}
