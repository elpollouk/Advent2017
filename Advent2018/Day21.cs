using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        void Problem1_Compiled()
        {
            var r = new int[6];

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

                    r[2] = 0;
                    while (true)
                    {
                        r[3] = r[2] + 1;
                        r[3] *= 256;
                        if (r[3] > r[1])
                            break;

                        r[2]++;
                    }
                    r[1] = r[2];
                }
            }
            while (r[4] != r[0]);
            // 15656032 Too high

            /*            r[4] = 0;
                    L1:
                        r[1] = r[4] | 65536;
                        r[4] = 2024736;
                    L2:
                        r[2] = r[1] & 255;
                        r[4] += r[2];
                        r[4] &= 16777215;
                        r[4] *= 65899;
                        r[4] &= 16777215;
                        r[2] = 256 > r[1] ? 1 : 0;
                        if (r[2] != 0) goto IF1;
                        goto EL1;

                    IF1:
                        goto L5;

                    EL1:
                        r[2] = 0;

                    L3:
                        r[3] = r[2] + 1;
                        r[3] *= 256;
                        r[3] = r[3] > r[1] ? 1 : 0;
                        if (r[3] != 0) goto IF2;
                        goto EL2;

                    IF2:
                        goto L4;

                    EL2:
                        r[2]++;
                        goto L3;
                    L4:
                        r[1] = r[2];
                        goto L2;

                    L5:
                        r[2] = r[4] == r[0] ? 1 : 0;
                        if (r[2] != 0) goto END;
                        goto L1;

                    END:
                        return;
            */
        }
    }
}
