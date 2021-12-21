using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2021
{
    public class Day21
    {
        class Game
        {
            readonly int[] players = new int[2];
            readonly int[] scores = new int[2];
            readonly Func<int> dice = Generators.Cycler(100);

            public long LoserScore;
            public long NumRoles;

            public Game(string filename)
            {
                var reader = FileIterator.CreateLineReader(filename);
                players[0] = int.Parse(reader().Groups(@"position: (\d)")[0]) - 1;
                players[1] = int.Parse(reader().Groups(@"position: (\d)")[0]) - 1;
            }

            int Roll() {
                NumRoles += 3;
                return dice() + dice() + dice() + 3;
            }

            public void Play()
            {
                int currentPlayer = 0;
                while (true)
                {
                    var move = Roll();
                    var pos = players[currentPlayer];
                    pos = (pos + move) % 10;
                    players[currentPlayer] = pos;
                    scores[currentPlayer] += (pos + 1);
                    if (scores[currentPlayer] >= 1000) break;

                    currentPlayer ^= 1;
                }

                LoserScore = scores[currentPlayer ^ 1];
            }
        }

        [Theory]
        [InlineData("Data/Day21_Test.txt", 739785)]
        [InlineData("Data/Day21.txt", 412344)]
        public void Part1(string filename, long expectedAnswer)
        {
            Game game = new(filename);
            game.Play();
            (game.LoserScore * game.NumRoles).Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day21_Test.txt", 0)]
        [InlineData("Data/Day21.txt", 0)]
        public void Part2(string filename, long expectedAnswer)
        {

        }
    }
}
