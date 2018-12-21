using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Advent2018
{
    public class Day21
    {
        [Fact]
        void Problem1_Raw()
        {
            var program = Day19.LoadProgram("Data/Day21.txt", out int ipr);
            var cpu = new Day19.Cpu
            {
                IPR = ipr
            };

            cpu.registers[0] = 7129803; // Part 1 solutions
            cpu.Execute(program);
        }

        [Fact]
        void Problem2_Compiled()
        {
            var seen = new HashSet<int>();
            int lastValue;

            var a = 0;
            do
            {
                seen.Add(a);
                lastValue = a;

                var b = a | 65536;
                a = 2024736;

                while (b != 0)
                {
                    a += b & 255;
                    a &= 16777215;
                    a *= 65899;
                    a &= 16777215;
                    b >>= 8;
                }
            }
            while (!seen.Contains(a));

            lastValue.Should().Be(12284643);
        }
    }
}
