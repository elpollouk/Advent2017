using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Utils.VM;
using Xunit;

namespace Advent2019
{
    public class Day07
    {
        const int BLANK_ENTRY = int.MinValue;

        IEnumerable<int[]> Permutations(params int[] values)
        {
            var output = new int[values.Length];
            return Permutations(values, output, 0);
        }

        IEnumerable<int[]> Permutations(int[] values, int[] output, int offset)
        {
            for (var i = 0; i < values.Length; i++)
            {
                if (values[i] == BLANK_ENTRY) continue;
                output[offset] = values[i];
                values[i] = BLANK_ENTRY;

                if (offset == output.Length - 1)
                {
                    yield return (int[])output.Clone();
                }
                else
                {
                    foreach (var o in Permutations(values, output, offset + 1))
                        yield return o;
                }

                values[i] = output[offset];
            }
        }

        [Fact]
        void TestPermutations()
        {
            var perms = Permutations(1, 2, 3).ToArray();
            perms.Length.Should().Be(6);
            perms.Should().ContainSingle(p => p.SequenceEqual(new int[] { 1, 2, 3 }));
            perms.Should().ContainSingle(p => p.SequenceEqual(new int[] { 1, 3, 2 }));
            perms.Should().ContainSingle(p => p.SequenceEqual(new int[] { 2, 1, 3 }));
            perms.Should().ContainSingle(p => p.SequenceEqual(new int[] { 2, 3, 1 }));
            perms.Should().ContainSingle(p => p.SequenceEqual(new int[] { 3, 2, 1 }));
            perms.Should().ContainSingle(p => p.SequenceEqual(new int[] { 3, 1, 2 }));
        }

        [Theory]
        [InlineData("Data/Day07-example.txt", 43210)]
        [InlineData("Data/Day07-example2.txt", 54321)]
        [InlineData("Data/Day07-example3.txt", 65210)]
        [InlineData("Data/Day07.txt", 116680)]
        public void Part1(string input, int answer)
        {
            var max = 0;
            var prog = FileIterator.LoadCSV<int>(input);
            foreach (var perm in Permutations(0, 1, 2, 3, 4))
            {
                var o = 0;
                foreach (var p in perm)
                {
                    var vm = IntCode.CreateVM(prog);
                    vm.State.Input = Generators.Reader(p, o);
                    vm.State.Output = _o => o = _o;
                    vm.Execute();
                }

                if (max < o) max = o;
            }

            max.Should().Be(answer);
        }

        [Theory]
        [InlineData("Data/Day07-example4.txt", 139629729)]
        [InlineData("Data/Day07-example5.txt", 18216)]
        [InlineData("Data/Day07.txt", 89603079)]
        public void Part2(string input, int answer)
        {
            var max = 0;
            var prog = FileIterator.LoadCSV<int>(input);
            foreach (var perm in Permutations(5, 6, 7, 8, 9))
            {
                var _out = 0;

                var vms = new List<Executor<IntCode.VmState, int, (int, int, int)>>();
                foreach (var p in perm)
                {
                    var vm = IntCode.CreateVM(prog);
                    vm.State.InputQueue.Enqueue(p);

                    vms.Add(vm);
                }

                foreach (var vm in vms.Cycler())
                {
                    vm.State.OutputQueue.Count.Should().Be(0);
                    vm.State.InputQueue.Enqueue(_out);
                    vm.Execute(step =>
                    {
                        while (vm.State.OutputQueue.Count == 0)
                            step();
                    });

                    if (vm.State.OutputQueue.Count == 0)
                    {
                        if (max < _out)
                            max = _out;
                        break;
                    }
                    else
                    {
                        _out = vm.State.OutputQueue.Dequeue();
                    }
                }
            }

            max.Should().Be(answer);
        }
    }
}
