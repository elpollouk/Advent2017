using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2020
{
    public class Day13
    {
        const int EXAMPLE_TIME = 939;
        const string EXAMPLE_BUSSES = "7,13,x,x,59,x,31,19";
        const int PROBLEM_TIME = 1002576;
        const string PROBLEM_BUSSES = "13,x,x,x,x,x,x,37,x,x,x,x,x,449,x,29,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,19,x,x,x,23,x,x,x,x,x,x,x,773,x,x,x,x,x,x,x,x,x,41,x,x,x,x,x,x,17";

        static IEnumerable<int> ParseBuses(string input)
        {
            foreach (var bus in input.Split(','))
                if (bus != "x")
                    yield return int.Parse(bus);
        }

        static (int bus, int offset)[] ParseBusesReverse(string input)
        {
            var list = new List<(int, int)>();
            var busses = input.Split(',');
            int offset = busses.Length;
            foreach (var bus in busses)
            {
                offset--;
                if (bus != "x")
                    list.Add((int.Parse(bus), offset));
            }

            return list.ToArray();
        }

        [Theory]
        [InlineData(EXAMPLE_TIME, EXAMPLE_BUSSES, 295)]
        [InlineData(PROBLEM_TIME, PROBLEM_BUSSES, 3865)]
        public void Problem1(int time, string busses, int expected)
        {
            var minWait = int.MaxValue;
            var minWaitBus = 0;
            foreach(var bus in ParseBuses(busses))
            {
                var wait = bus - (time % bus);
                if (wait < minWait)
                {
                    minWait = wait;
                    minWaitBus = bus;
                }
            }

            (minWait * minWaitBus).Should().Be(expected);
        }

        static bool IsTimeMatch(long time, (int bus, int offset)[] busses)
        {
            foreach (var (bus, offset) in busses)
            {
                if ((time - offset) % bus != 0) return false;
            }
            return true;
        }

        /*[Theory]
        [InlineData(EXAMPLE_BUSSES, 1000000, 1068781)]
        //[InlineData(PROBLEM_BUSSES, 100000000000000, 0)]
        public void Problem2(string input, long startTime, long expected)
        {
            var busses = ParseBusesReverse(input);
            var maxBus = busses.MaxItem(bus => bus.bus);
            var correctionOffset = maxBus.offset;
            long time = startTime + (busses[0].bus - (startTime % busses[0].bus));
            while (true)
            {
                if (IsTimeMatch(time, busses))
                {
                    (time - correctionOffset).Should().Be(expected);
                    break;
                }
                time += maxBus.bus;
            }
        }*/
    }
}
