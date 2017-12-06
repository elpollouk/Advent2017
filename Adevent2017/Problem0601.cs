using Adevent2017.Utils;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Adevent2017
{
    public class Problem0601
    {
        int MaxIndex(int[] input)
        {
            var max = input[0];
            var maxIndex = 0;

            for (var i = 1; i < input.Length; i++)
            {
                if (input[i] > max)
                {
                    max = input[i];
                    maxIndex = i;
                }
            }
            return maxIndex;
        }

        string Redistribute(int[] input)
        {
            var m = MaxIndex(input);
            var i = m + 1;
            var r = input[m];
            var len = input.Length;
            input[m] = 0;

            var overageCount = r % len;
            var normalRedistVal = r / len;

            for (var j = 0; j < len; j++)
            {
                var v = normalRedistVal;
                if (overageCount != 0)
                {
                    overageCount--;
                    v++;
                }
                input[i++ % len] += v;
            }

            var builder = new StringBuilder();
            foreach (var v in input)
            {
                builder.Append(v);
                builder.Append(',');
            }
            return builder.ToString();
        }

        public int Solve(Func<int, int, int> resultor, params int[] input)
        {
            var history = new Dictionary<string, int>();
            var count = 1;

            while (true)
            {
                var signature = Redistribute(input);
                if (history.ContainsKey(signature)) return resultor(count, history[signature]);
                history[signature] = count;
                count++;
            }
        }

        [Fact]
        public void Example1() =>Solve((c, h) => c, 0, 2, 7, 0).Should().Be(5);

        [Fact]
        public void Solution1()
        {
            var data = FileIterator.LoadLines<int>("Data/0601.txt");
            Solve((c, h) => c, data).Should().Be(14029);
        }

        [Fact]
        public void Example2() => Solve((c, h) => c - h, 0, 2, 7, 0).Should().Be(4);

        [Fact]
        public void Solution2()
        {
            var data = FileIterator.LoadLines<int>("Data/0601.txt");
            Solve((c, h) => c - h, data).Should().Be(2765);
        }
    }
}
