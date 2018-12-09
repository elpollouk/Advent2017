using FluentAssertions;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2018
{
    public class Day09
    {
        class Marble
        {
            public readonly ulong Value;
            public Marble Clockwise;
            public Marble AntiClockwise;

            public Marble(ulong value)
            {
                Value = value;
                if (value == 0)
                {
                    Clockwise = this;
                    AntiClockwise = this;
                }
            }

            public void Insert(Marble marble)
            {
                var clock1 = Clockwise;
                var clock2 = clock1.Clockwise;

                marble.AntiClockwise = clock1;
                marble.Clockwise = clock2;
                clock1.Clockwise = marble;
                clock2.AntiClockwise = marble;
            }

            public Marble RemoveAntiClockwise(uint offset)
            {
                var marble = this;
                while (offset-- != 0)
                    marble = marble.AntiClockwise;

                marble.AntiClockwise.Clockwise = marble.Clockwise;
                marble.Clockwise.AntiClockwise = marble.AntiClockwise;

                return marble;
            }

            public override string ToString() => $"{Value}";
        }

        [Theory]
        [InlineData(9, 25, 32)]
        [InlineData(10, 1618, 8317)]
        [InlineData(13, 7999, 146373)]
        [InlineData(17, 1104, 2764)]
        [InlineData(21, 6111, 54718)]
        [InlineData(30, 5807, 37305)]
        [InlineData(465, 71498, 383475)] // Solution 1
        [InlineData(465, 7149800, 3148209772)] // Solution 2
        public void Solve(int numPlayers, int numMarbles, ulong answer)
        {
            var players = new ulong[numPlayers];
            var getCurrentPlayer = Generators.Cycler(numPlayers);
            var currentMarble = new Marble(0);

            for (var i = 1u; i <= numMarbles; i++)
            {
                var currentPlayer = getCurrentPlayer();
                if (i % 23 != 0)
                {
                    var marble = new Marble(i);
                    currentMarble.Insert(marble);
                    currentMarble = marble;
                }
                else
                {
                    var marble = currentMarble.RemoveAntiClockwise(7);
                    players[currentPlayer] += (i + marble.Value);
                    currentMarble = marble.Clockwise;
                }
            }

            players.Max().Should().Be(answer);
        }
    }
}
