using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Advent2018
{
    public class Day21
    {
        //[Fact]
        void Promblem1_Raw()
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
            var r = new int[6];
            var seen = new HashSet<int>();
            int lastValue = -1;

            r[4] = 0;
            do
            {
                r[1] = r[4] | 65536;
                r[4] = 2024736;

                while (true)
                {
                    r[2] = r[1] & 255;
                    r[4] += r[2];
                    r[4] &= 16777215;
                    r[4] *= 65899;
                    r[4] &= 16777215;

                    if (256 > r[1])
                        break;

                    r[1] >>= 8;
                }

                if (seen.Contains(r[4]))
                    break;
                lastValue = r[4];
                seen.Add(lastValue);
            }
            while (r[4] != r[0]);

            lastValue.Should().Be(12284643);
        }
    }
}
