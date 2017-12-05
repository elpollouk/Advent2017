using Adevent2017.Utils;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Adevent2017
{
    public class Problem0501
    {
        public int Solve1(int[] prog)
        {
            var count = 0;
            var ip = 0;
            Func<bool> IsDone = () => ip < 0 || ip >= prog.Length;

            while (!IsDone())
            {
                ip = ip += prog[ip]++;
                count++;
            }

            return count;
        }

        [Fact]
        public void Example1()
        {
            var prog = new int[] { 0, 3, 0, 1, -3 };
            Solve1(prog).Should().Be(5);
        }

        [Fact]
        public void Solution1()
        {
            var prog = new List<int>();
            FileIterator.ForEachLine("Data/0501.txt", line =>
            {
                prog.Add(int.Parse(line));
            });

            Solve1(prog.ToArray()).Should().Be(373160);
        }
    }
}
