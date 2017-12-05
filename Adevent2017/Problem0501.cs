using Adevent2017.Utils;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Adevent2017
{
    public class Problem0501
    {
        int Problem1(int offset) => offset + 1;

        int Problem2(int offset) => offset >= 3 ? offset - 1 : offset + 1;

        public int Exec(int[] prog, Func<int, int> updateCell)
        {
            var count = 0;
            var ip = 0;
            Func<bool> IsDone = () => ip < 0 || ip >= prog.Length;

            while (!IsDone())
            {
                var offset = prog[ip];
                prog[ip] = updateCell(offset);
                ip += offset;
                count++;
            }

            return count;
        }

        [Fact]
        public void Example1()
        {
            var prog = new int[] { 0, 3, 0, 1, -3 };
            Exec(prog, Problem1).Should().Be(5);
        }

        [Fact]
        public void Solution1()
        {
            var prog = new List<int>();
            FileIterator.ForEachInt("Data/0501.txt", value =>
            {
                prog.Add(value);
            });

            Exec(prog.ToArray(), Problem1).Should().Be(373160);
        }

        [Fact]
        public void Example2()
        {
            var prog = new int[] { 0, 3, 0, 1, -3 };
            Exec(prog, Problem2).Should().Be(10);
        }

        [Fact]
        public void Solution2()
        {
            var prog = new List<int>();
            FileIterator.ForEachInt("Data/0501.txt", value =>
            {
                prog.Add(value);
            });

            Exec(prog.ToArray(), Problem2).Should().Be(26395586);
        }
    }
}
