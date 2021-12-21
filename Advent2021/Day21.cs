﻿using FluentAssertions;
using System;
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

            private static readonly int[] distribution = new int[10];

            static Game()
            {
                // Generate the distribution for the outcomes of all 3-sided dice rolls
                for (var i = 1; i <= 3; i++)
                    for (var j = 1; j <= 3; j++)
                        for (var k = 1; k <= 3; k++)
                            distribution[i + j + k]++;
            }

            public Game(string filename)
            {
                var reader = FileIterator.CreateLineReader(filename);
                players[0] = int.Parse(reader().Groups(@"position: (\d)")[0]) - 1;
                players[1] = int.Parse(reader().Groups(@"position: (\d)")[0]) - 1;
            }

            int Roll() {
                NumRoles += 3;
                return dice() + dice() + dice() + 3; // Each "dice()" return 0-99, so add 3 to fix the roll to the correct range
            }

            static int Move(int pos, int roll) => (pos + roll) % 10;
            static int Score(int pos) => pos + 1;

            public void Play()
            {
                int currentPlayer = 0;
                while (true)
                {
                    var roll = Roll();
                    var pos = players[currentPlayer];
                    pos = Move(pos, roll);
                    players[currentPlayer] = pos;
                    scores[currentPlayer] += Score(pos);
                    if (scores[currentPlayer] >= 1000) break;

                    // XOR the player to flip-flop between player 0 and 1 each step
                    currentPlayer ^= 1;
                }

                LoserScore = scores[currentPlayer ^ 1];
            }

            void BruteExplore0(ref long winCount, long pathCount, int rollCount, int roll, int score0, int score1, int position0, int position1)
            {
                position0 = Move(position0, roll);
                score0 += Score(position0);
                if (score0 >= 21)
                {
                    winCount += pathCount;
                    return;
                }

                rollCount++;
                for (roll = 3; roll <= 9; roll++)
                {
                    BruteExplore1(ref winCount, pathCount * distribution[roll], rollCount, roll, score0, score1, position0, position1);
                }
            }

            void BruteExplore1(ref long winCount, long pathCount, int rollCount, int roll, int score0, int score1, int position0, int position1)
            {
                position1 = Move(position1, roll);
                score1 += Score(position1);
                if (score1 >= 21) return; // This path beat the player we're calculating wins for, so abort this search branch

                rollCount++;
                for (roll = 3; roll <= 9; roll++)
                {
                    BruteExplore0(ref winCount, pathCount * distribution[roll], rollCount, roll, score0, score1, position0, position1);
                }
            }

            public long BruteExplore(int player)
            {
                long winCount = 0;
                for (var roll = 3; roll <= 9; roll++)
                {
                    if (player == 0)
                    {
                        BruteExplore0(ref winCount, distribution[roll], 1, roll, 0, 0, players[0], players[1]);
                    }
                    else
                    {
                        // Player 0 always goes first, so if we're calculating for player 1, simulate the "other" player's move first
                        // while also reversing the starting positions.
                        BruteExplore1(ref winCount, distribution[roll], 1, roll, 0, 0, players[1], players[0]);
                    }
                }
                return winCount;
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
        [InlineData("Data/Day21_Test.txt", 444356092776315)]
        [InlineData("Data/Day21.txt", 214924284932572)]
        public void Part2(string filename, long expectedAnswer)
        {
            Game game = new(filename);

            var wins0 = game.BruteExplore(0);
            var wins1 = game.BruteExplore(1);

            var answer = Math.Max(wins0, wins1);

            answer.Should().Be(expectedAnswer);
        }
    }
}
