using FluentAssertions;
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
            Mem
        }

        record Instruction(Operation Op, long OperandA, long OperandB, string Mask);

        class VmState
        {
            public Dictionary<long, long> Mem = new();
            public long AndMask = 0;
            public long OrMask = 0;
            public string Mask = null;

            void SetMemRecursive(int maskIndex, long addressSoFar, long originalAddress, long value)
            {
                while (maskIndex < Mask.Length)
                {
                    var bitToCheck = ((Mask.Length - maskIndex)) - 1;
                    var singleBitMask = 1L << bitToCheck;
                    switch (Mask[maskIndex])
                    {
                        case '0':
                            addressSoFar |= originalAddress & singleBitMask;
                            break;

                        case '1':
                            addressSoFar |= singleBitMask;
                            break;

                        case 'X':
                            SetMemRecursive(maskIndex + 1, addressSoFar, originalAddress, value);
                            SetMemRecursive(maskIndex + 1, addressSoFar + singleBitMask, originalAddress, value);
                            return;
                    }

                    maskIndex++;
                }
                Mem[addressSoFar] = value;
            }

            public void Execute(Instruction[] prog, bool part2)
            {
                foreach (var instuction in prog)
                {
                    switch (instuction.Op)
                    {
                        case Operation.Mem:
                            if (part2)
                            {
                                SetMemRecursive(0, 0, instuction.OperandA, instuction.OperandB);
                            }
                            else
                            {
                                var value = instuction.OperandB;
                                value &= AndMask;
                                value |= OrMask;
                                Mem[instuction.OperandA] = value;
                            }
                            break;

                        case Operation.Mask:
                            AndMask = instuction.OperandA;
                            OrMask = instuction.OperandB;
                            Mask = instuction.Mask;
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
                string mask = null;

                if (line.StartsWith("mask ="))
                {
                    // For part 1, the masking logic can be implemented by first applying AND to the value to
                    // filter out the bits that need to be cleared and then applying OR to set the bits that
                    // must be set.
                    // To create a valid filter, all the 'X' bits must be set to 1 for the AND mask
                    // To prevent over-setting bits, all the 'X' bits must be set to 0 for the OR mask
                    op = Operation.Mask;
                    mask = line[7..];
                    foreach (var c in mask)
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

                prog.Add(new Instruction(op, operandA, operandB, mask));
            }

            return prog.ToArray();
        }

        [Theory]
        [InlineData("Data/Day14_test.txt", false, 165)]
        [InlineData("Data/Day14_test2.txt", true, 208)]
        [InlineData("Data/Day14.txt", false, 16003257187056)]
        [InlineData("Data/Day14.txt", true, 3219837697833)]
        public void Problem(string input, bool part2, long expected)
        {
            var prog = LoadProgram(input);
            var vm = new VmState();
            vm.Execute(prog, part2);
            vm.Mem.Values.Sum().Should().Be(expected);
        }
    }
}
