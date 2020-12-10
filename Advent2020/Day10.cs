using FluentAssertions;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2020
{
    public class Day10
    {
        int CalcJolts(string input)
        {
            var prevJolt = 0;
            var jump1 = 0;
            var jump3 = 1; // Include the final 3 jolt jump up front
            foreach (var jolt in FileIterator.Lines<int>(input).OrderBy((v) => v))
            {
                switch (jolt - prevJolt)
                {
                    case 1:
                        jump1++;
                        break;

                    case 2:
                        break;

                    case 3:
                        jump3++;
                        break;

                    default:
                        Oh.Bugger();
                        break;
                }
                prevJolt = jolt;
            }

            return jump1 * jump3;
        }

        long CountRoutes(string input)
        {
            var adapters = FileIterator.Lines<int>(input);
            var pathTotals = new long[adapters.Max() + 1];

            // All path start at 0 which has an implicit single path.
            pathTotals[0] = 1;

            // Loop through the adapters in order, cascading up the total number of paths to reach each adapter.
            // An adapter's total path value is the sum of the path values of all the adapters that link to the current adapter.
            // We traverse the adapters in order and then back track to find the incoming totals.
            foreach (var adapter in adapters.OrderBy((v) => v))
            {
                var total = 0L;
                for (var i = 1; i < 4; i++)
                {
                    if (adapter - i < 0) break;
                    // As we're not using a sparse array and the array is zero-initialised, it's safe to sum up all three previous elements.
                    // Because we traverse in order, we are guaranteed to have calculated the previous values if they have non-zero total path count.
                    total += pathTotals[adapter - i];
                }
                pathTotals[adapter] = total;
            }

            // The answer should have cascaded up into the last element of the graph array.
            return pathTotals.Last();
        }

        [Theory]
        [InlineData("Data/Day10_test1.txt", 35)]
        [InlineData("Data/Day10_test2.txt", 220)]
        [InlineData("Data/Day10.txt", 2046)]
        public void Problem1(string input, int expected)
        {
            CalcJolts(input).Should().Be(expected);
        }

        [Theory]
        [InlineData("Data/Day10_test1.txt", 8)]
        [InlineData("Data/Day10_test2.txt", 19208)]
        [InlineData("Data/Day10.txt", 1157018619904)]
        public void Problem2(string input, long expected)
        {
            CountRoutes(input).Should().Be(expected);
        }
    }
}
