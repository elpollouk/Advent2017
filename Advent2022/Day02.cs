using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2022
{
    public class Day02
    {
        const int LOSE = 0;
        const int DRAW = 3;
        const int WIN = 6;

        enum Play
        {
            ROCK,
            PAPER,
            SCISSORS
        }

        Play ParsePlay(string c) => c switch
        {
            "A" => Play.ROCK,
            "B" => Play.PAPER,
            "C" => Play.SCISSORS,
            "X" => Play.ROCK,
            "Y" => Play.PAPER,
            "Z" => Play.SCISSORS,
            _ => throw new ArgumentException(),
        };

        int ParseRequiredResult(string c) => c switch
        {
            "X" => LOSE,
            "Y" => DRAW,
            "Z" => WIN,
            _ => throw new ArgumentException()
        };

        int ScorePlay(Play p) => p switch
        {
            Play.ROCK => 1,
            Play.PAPER => 2,
            Play.SCISSORS => 3,
            _ => throw new ArgumentException()
        };

        int ScoreRound(Play theirs, Play yours) => theirs switch
        {
            Play.ROCK => yours switch
            {
                Play.ROCK => DRAW,
                Play.PAPER => WIN,
                Play.SCISSORS => LOSE,
                _ => throw new ArgumentException()
            },
            Play.PAPER => yours switch
            {
                Play.ROCK => LOSE,
                Play.PAPER => DRAW,
                Play.SCISSORS => WIN,
                _ => throw new ArgumentException()
            },
            Play.SCISSORS => yours switch
            {
                Play.ROCK => WIN,
                Play.PAPER => LOSE,
                Play.SCISSORS => DRAW,
                _ => throw new ArgumentException()
            },
            _ => throw new ArgumentException()
        };

        Play Think(Play theirPlay, int requiredResult) => theirPlay switch
        {
            Play.ROCK => requiredResult switch
            {
                WIN => Play.PAPER,
                DRAW => Play.ROCK,
                LOSE => Play.SCISSORS,
                _ => throw new ArgumentException()
            },
            Play.PAPER => requiredResult switch
            {
                WIN => Play.SCISSORS,
                DRAW => Play.PAPER,
                LOSE => Play.ROCK,
                _ => throw new ArgumentException()
            },
            Play.SCISSORS => requiredResult switch
            {
                WIN => Play.ROCK,
                DRAW => Play.SCISSORS,
                LOSE => Play.PAPER,
                _ => throw new ArgumentException()
            },
            _ => throw new ArgumentException()
        };


        [Theory]
        [InlineData("Data/Day02_Test.txt", 15)]
        [InlineData("Data/Day02.txt", 17189)]
        public void Part1(string filename, long expectedAnswer)
        {
            long sum = 0;

            foreach (var line in FileIterator.Lines(filename))
            {
                var groups = line.Groups("(.) (.)");
                var theirPlay = ParsePlay(groups[0]);
                var yourPlay = ParsePlay(groups[1]);
                int score = ScoreRound(theirPlay, yourPlay) + ScorePlay(yourPlay);
                sum += score;
            }

            sum.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day02_Test.txt", 12)]
        [InlineData("Data/Day02.txt", 13490)]
        public void Part2(string filename, long expectedAnswer)
        {
            long sum = 0;

            foreach (var line in FileIterator.Lines(filename))
            {
                var groups = line.Groups("(.) (.)");
                var theirPlay = ParsePlay(groups[0]);
                var requiredResult = ParseRequiredResult(groups[1]);
                var yourPlay = Think(theirPlay, requiredResult);
                int score = ScoreRound(theirPlay, yourPlay) + ScorePlay(yourPlay);
                sum += score;
            }

            sum.Should().Be(expectedAnswer);
        }
    }
}
