using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Utils.DataStructures;
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
        [InlineData("Data/Day07.txt", 0)]
        public void Problem(string input, int answer)
        {
            throw new NotImplementedException();
        }
    }
}
