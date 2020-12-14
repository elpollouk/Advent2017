using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2020
{
    public class Day14
    {
        enum Operation {
            Mask,
            Mem,
        }

        enum BitOp
        {
            Set,
            Clear,
            Copy
        }

        record Instruction(Operation Op, long operandA, long operandB);

        class VmState
        {
            public Dictionary<long, long> Mem = new();
            public long AndMask = 0;
            public long OrMask = 0;

            public void Execute(Instruction[] prog)
            {
                foreach (var instuction in prog)
                {
                    switch (instuction.Op)
                    {
                        case Operation.Mem:
                            var value = instuction.operandB;
                            value &= AndMask;
                            value |= OrMask;
                            Mem[instuction.operandA] = value;

                            break;

                        case Operation.Mask:
                            AndMask = instuction.operandA;
                            OrMask = instuction.operandB;
                            break;

                        default:
                            Oh.Bugger();
                            break;
                    }
                }
            }
        }

        static Instruction[] LoadProgram(string input)
        {
            var prog = new List<Instruction>();

            foreach (var line in FileIterator.Lines(input))
            {
                Operation op;
                long operandA = 0;
                long operandB = 0;

                if (line.StartsWith("mask ="))
                {
                    op = Operation.Mask;
                    foreach (var c in line[7..])
                    {
                        switch (c)
                        {
                            case '1':
                                operandA = (operandA << 1) + 1;
                                operandB = (operandB << 1) + 1;
                                break;

                            case '0':
                                operandA <<= 1;
                                operandB <<= 1;
                                break;

                            case 'X':
                                operandA = (operandA << 1) + 1;
                                operandB <<= 1;
                                break;

                            default:
                                Oh.Bollocks();
                                break;
                        };
                    }
                }
                else
                {
                    op = Operation.Mem;
                    var groups = line.Groups("^mem\\[(\\d+)\\] = (\\d+)$");
                    operandA = long.Parse(groups[0]);
                    operandB = long.Parse(groups[1]);
                }

                prog.Add(new Instruction(op, operandA, operandB));
            }

            return prog.ToArray();
        }


        [Theory]
        [InlineData("Data/Day14_test.txt", 165)]
        [InlineData("Data/Day14.txt", 16003257187056)]
        public void Problem1(string input, long expected)
        {
            var prog = LoadProgram(input);
            var vm = new VmState();
            vm.Execute(prog);
            vm.Mem.Values.Sum().Should().Be(expected);
        }

        /*[Theory]
        public void Problem2(string input, long expected)
        {
        }*/
    }
}
