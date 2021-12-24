using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2021
{
    public class Day24
    {
        class VmState
        {
            public readonly long[] wxyz = new long[4];
            public readonly Queue<long> Input = new();

            public void Write(long value)
            {
                Input.Enqueue(value);
            }

            public void Execute(IEnumerable<string> lines)
            {
                foreach (var line in lines)
                    if (!exec(line))
                        return;
            }

            long FetchValue(string value)
            {
                char c = value[0];
                if ('w' <= c && c <= 'z') return wxyz[c - 'w'];
                return long.Parse(value);
            }

            void Store(string location, long value)
            {
                char c = location[0];
                wxyz[c - 'w'] = value;
            }

            bool exec(string line)
            {
                var op = line.Groups(@"([a-z]+) ([w-z]+) ?([-w-z0-9]+)?");
                switch (op[0])
                {
                    case "inp":
                        if (Input.Count == 0) return false;
                        Store(op[1], Input.Dequeue());
                        break;
                    case "add":
                        Store(op[1], FetchValue(op[1]) + FetchValue(op[2]));
                        break;
                    case "mul":
                        Store(op[1], FetchValue(op[1]) * FetchValue(op[2]));
                        break;
                    case "div":
                        Store(op[1], FetchValue(op[1]) / FetchValue(op[2]));
                        break;
                    case "mod":
                        Store(op[1], FetchValue(op[1]) % FetchValue(op[2]));
                        break;
                    case "eql":
                        Store(op[1], FetchValue(op[1]) == FetchValue(op[2]) ? 1 : 0);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
                return true;
            }
        }

        long ExtractConstant(string line)
        {
            var op = line.Groups(@"([a-z]+) ([w-z]+) ?([-w-z0-9]+)?");
            return long.Parse(op[2]);
        }

        (long, long, long) ExtractConstants(string[] prog, int offset)
        {
            var c5 = ExtractConstant(prog[offset + 4]);
            var c6 = ExtractConstant(prog[offset + 5]);
            var c16 = ExtractConstant(prog[offset + 15]);

            return (c5, c6, c16);
        }

        [Theory]
        [InlineData(0, 0, 0, 0, 0)]
        [InlineData(1, 0, 0, 0, 1)]
        [InlineData(2, 0, 0, 1, 0)]
        [InlineData(3, 0, 0, 1, 1)]
        [InlineData(4, 0, 1, 0, 0)]
        [InlineData(5, 0, 1, 0, 1)]
        [InlineData(6, 0, 1, 1, 0)]
        [InlineData(7, 0, 1, 1, 1)]
        [InlineData(8, 1, 0, 0, 0)]
        [InlineData(9, 1, 0, 0, 1)]
        public void TestProgram(long input, long w, long x, long y, long z)
        {
            var prog = FileIterator.Lines("Data/Day24_Test.txt");
            var vm = new VmState();
            vm.Write(input);
            vm.Execute(prog);
            vm.wxyz[0].Should().Be(w);
            vm.wxyz[1].Should().Be(x);
            vm.wxyz[2].Should().Be(y);
            vm.wxyz[3].Should().Be(z);
        }

        void Step(VmState vm, long w, (long c5, long c6, long c16) constants)
        {
            long z = vm.wxyz[3];
            long x = z % 26;            // Peek
            z /= constants.c5;          // 1 or 26 (pop if 26)

            if (x + constants.c6 != w)
            {
                // Push
                z *= 26;
                z += w + constants.c16;
            }

            vm.wxyz[3] = z;
        }

        [Fact]
        public void Part1_Native()
        {
            var prog = FileIterator.Lines("Data/Day24.txt").ToArray();
            var vm = new VmState();
            var inputs = new long[] { 9, 9, 5, 9, 8, 9, 6, 3, 9, 9, 9, 9, 7, 1 };

            for (var i = 0; i <  14; i++)
            {
                var constants = ExtractConstants(prog, i * 18);
                Step(vm, inputs[i], constants);
            }

            vm.wxyz[3].Should().Be(0);
        }

        [Fact]
        public void Part1_Script()
        {
            var prog = FileIterator.Lines("Data/Day24.txt");
            var vm = new VmState();

            foreach (var v in new long[] { 9, 9, 5, 9, 8, 9, 6, 3, 9, 9, 9, 9, 7, 1 })
                vm.Write(v);

            vm.Execute(prog);
            vm.wxyz[3].Should().Be(0);
        }

        [Fact]
        public void Part2_Script()
        {
            var prog = FileIterator.Lines("Data/Day24.txt");
            var vm = new VmState();
            foreach (var v in new long[] { 9, 3, 1, 5, 1, 4, 1, 1, 7, 1, 1, 2, 1, 1 })
                vm.Write(v);

            vm.Execute(prog);
            vm.wxyz[3].Should().Be(0);
        }
    }
}
