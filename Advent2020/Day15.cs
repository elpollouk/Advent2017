using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2020
{
    public class Day15
    {

        [Theory]
        [InlineData("0,3,6", 2020, 436)]
        [InlineData("0,3,6", 30000000, 175594)]
        [InlineData("0,5,4,1,10,14,7", 2020, 203)]
        [InlineData("0,5,4,1,10,14,7", 30000000, 9007186)]
        public void Problem(string input, int target, int expected)
        {
            var spokenNumbers = new Dictionary<int, (int, int)>();
            var turn = 1;
            var lastNumber = -1;
            foreach (var number in input.Split(",").Select(s => int.Parse(s)))
            {
                spokenNumbers[number] = (turn++, 0);
                lastNumber = number;
            }

            while (turn <= target)
            {
                var (previousTurn, previousPreviousTurn) = spokenNumbers.GetOrDefault(lastNumber, (0, 0));
                int numberToSay;
                if (previousPreviousTurn == 0)
                {
                    // Last turn was the first time the number was spoken
                    numberToSay = 0;
                }
                else
                {
                    numberToSay = previousTurn - previousPreviousTurn;
                }
                (previousTurn, _) = spokenNumbers.GetOrDefault(numberToSay, (0, 0));
                spokenNumbers[numberToSay] = (turn++, previousTurn);
                lastNumber = numberToSay;
            }

            lastNumber.Should().Be(expected);
        }
    }
}
