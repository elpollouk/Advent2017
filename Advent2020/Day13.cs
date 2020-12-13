using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Advent2020
{
    public class Day13
    {
        const int EXAMPLE_TIME = 939;
        const string EXAMPLE_BUSES = "7,13,x,x,59,x,31,19";
        const int PROBLEM_TIME = 1002576;
        const string PROBLEM_BUSES = "13,x,x,x,x,x,x,37,x,x,x,x,x,449,x,29,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,19,x,x,x,23,x,x,x,x,x,x,x,773,x,x,x,x,x,x,x,x,x,41,x,x,x,x,x,x,17";

        static IEnumerable<int> ParseBuses(string input)
        {
            foreach (var bus in input.Split(','))
                if (bus == "x") yield return -1;
                else yield return int.Parse(bus);
        }

        [Theory]
        [InlineData(EXAMPLE_TIME, EXAMPLE_BUSES, 295)]
        [InlineData(PROBLEM_TIME, PROBLEM_BUSES, 3865)]
        public void Problem1(int time, string buses, int expected)
        {
            var minWait = int.MaxValue;
            var minWaitBus = 0;
            foreach(var bus in ParseBuses(buses))
            {
                if (bus == -1) continue;
                var wait = bus - (time % bus);
                if (wait < minWait)
                {
                    minWait = wait;
                    minWaitBus = bus;
                }
            }

            (minWait * minWaitBus).Should().Be(expected);
        }

        [Theory]
        [InlineData("17,x,13,19", 3417)]
        [InlineData("67,7,59,61", 754018)]
        [InlineData("67,x,7,59,61", 779210)]
        [InlineData("67,7,x,59,61", 1261476)]
        [InlineData("1789,37,47,1889", 1202161486)]
        [InlineData(EXAMPLE_BUSES, 1068781)]
        [InlineData(PROBLEM_BUSES, 415579909629976)]
        public void Problem2(string input, long expected)
        {
            var offset = -1;
            var step = 0L;
            var time = 0L;
            foreach (var bus in ParseBuses(input))
            {
                offset++;
                if (bus == -1) continue;
                if (offset == 0)
                {
                    // We know "time % bus[0] == 0" will hold for the final answer, so start advancing time in step sizes of
                    // this bus number.
                    step = bus;
                    continue;
                }

                // The buses will now have an offset, so we need to calculate how many minutes into the bus cycle we expect to
                // be at the time of the final answer. This is the same calculation from part 1. E.g, if the second bus has the
                // number 15, its cycle offset would be 14 at the final time.
                // We can't just subtract the offset directly as it's possible for the offset to be larger than the bus number
                // requiring wrap around handling.
                var cycleOffset = bus - (offset % bus);

                // Advance time by the current step value until we find a time that has the same remainder as the expected cycle
                // offset for this bus.
                while ((time % bus) != cycleOffset)
                    time += step;

                // Multiply the step size by this bus number as we now need to advance time in steps that ensures the time value
                // constraints we've calculated so far continue to hold, i.e. each time value will have the same remainder for
                // all the busses checked so far.
                step *= bus;
            }

            time.Should().Be(expected);
        }
    }
}
