using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2023
{
    public class Day07
    {
        enum HandRank
        {
            FIVE_OF_A_KIND = 6,
            FOUR_OF_A_KIND = 5,
            FULL_HOUSE = 4,
            THREE_OF_A_KIND = 3,
            TWO_PAIR = 2,
            ONE_PAIR = 1,
            HIGH_CARD = 0,
        }

        class Hand
        {
            public readonly string Raw;
            public readonly HandRank PrimaryRank;
            public readonly long SecondaryRank;

            public Hand(string hand, Func<Dictionary<char, int>, HandRank> getHandRank, Func<char, int> getCardRank)
            {
                var countedCards = new Dictionary<char, int>();
                long srank = 0;
                foreach (var card in hand)
                {
                    srank *= 100;
                    srank += getCardRank(card);
                    countedCards.Increment(card);
                }
                Raw = hand;
                PrimaryRank = getHandRank(countedCards);
                SecondaryRank = srank;
            }

            public static int Compare(Hand h1, Hand h2)
            {
                if (h1.PrimaryRank < h2.PrimaryRank) return -1;
                if (h1.PrimaryRank == h2.PrimaryRank)
                {
                    return h1.SecondaryRank < h2.SecondaryRank ? -1 : 1;
                }
                return 1;
            }

            public override string ToString()
            {
                return $"{Raw}: {PrimaryRank}";
            }
        }

        record Game(Hand Hand, long Bid);

        static HandRank GetHandRank(Dictionary<char, int> hand)
        {
            HandRank rank = HandRank.HIGH_CARD;

            foreach (var count in hand.Values)
            {
                if (count == 5)
                {
                    return HandRank.FIVE_OF_A_KIND;
                }
                if (count == 4)
                {
                    return HandRank.FOUR_OF_A_KIND;
                }
                if (count == 3)
                {
                    if (rank == HandRank.ONE_PAIR)
                    {
                        return HandRank.FULL_HOUSE;
                    }
                    rank = HandRank.THREE_OF_A_KIND;
                }
                else if (count == 2)
                {
                    if (rank == HandRank.THREE_OF_A_KIND)
                    {
                        return HandRank.FULL_HOUSE;
                    }
                    if (rank == HandRank.ONE_PAIR)
                    {
                        return HandRank.TWO_PAIR;
                    }
                    rank = HandRank.ONE_PAIR;
                }
            }

            return rank;
        }

        static HandRank GetHandRankJoker(Dictionary<char, int> hand)
        {
            int jokers = hand.GetOrDefault('J');
            if (jokers == 5)
            {
                return HandRank.FIVE_OF_A_KIND;
            }
            HandRank rank = HandRank.HIGH_CARD;

            // Filer out the jokers and then order by card count so that the jokers can contribute to the most common card
            foreach (var trueCount in hand.Where(kv => kv.Key != 'J').Select(kv => kv.Value).OrderByDescending(v => v))
            {
                var count = trueCount + jokers;
                jokers = 0; // Always consume jokers

                if (count == 5)
                {
                    return HandRank.FIVE_OF_A_KIND;
                }
                if (count == 4)
                {
                    return HandRank.FOUR_OF_A_KIND;
                }
                if (count == 3)
                {
                    rank = HandRank.THREE_OF_A_KIND;
                }
                else if (count == 2)
                {
                    if (rank == HandRank.THREE_OF_A_KIND)
                    {
                        return HandRank.FULL_HOUSE;
                    }
                    if (rank == HandRank.ONE_PAIR)
                    {
                        return HandRank.TWO_PAIR;
                    }
                    rank = HandRank.ONE_PAIR;
                }
            }

            return rank;
        }

        static int GetCardRank(char c)
        {
            return c switch
            {
                '2' => 2,
                '3' => 3,
                '4' => 4,
                '5' => 5,
                '6' => 6,
                '7' => 7,
                '8' => 8,
                '9' => 9,
                'T' => 10,
                'J' => 11,
                'Q' => 12,
                'K' => 13,
                'A' => 14,
                _ => throw new InvalidOperationException($"Invalid card: {c}")
            };
        }

        static int GetCardRankJoker(char c)
        {
            return c switch
            {
                '2' => 2,
                '3' => 3,
                '4' => 4,
                '5' => 5,
                '6' => 6,
                '7' => 7,
                '8' => 8,
                '9' => 9,
                'T' => 10,
                'J' => 1,
                'Q' => 12,
                'K' => 13,
                'A' => 14,
                _ => throw new InvalidOperationException($"Invalid card: {c}")
            };
        }

        static int Compare(Game g1, Game g2)
        {
            return Hand.Compare(g1.Hand, g2.Hand);
        }

        long ScoreHands(string filename, Func<Dictionary<char, int>, HandRank> getHandRank, Func<char, int> getCardRank)
        {
            List<Game> games = [];
            foreach (var line in FileIterator.Lines(filename))
            {
                var parts = line.Split(' ');
                var hand = new Hand(parts[0], getHandRank, getCardRank);
                var game = new Game(hand, long.Parse(parts[1]));
                games.Add(game);
            }

            games.Sort(Compare);


            long winnings = 0;
            for (int i = 0; i < games.Count; i++)
            {
                winnings += games[i].Bid * (i + 1);
            }

            return winnings;
        }

        [Theory]
        [InlineData("Data/Day07_Test.txt", 6440)]
        [InlineData("Data/Day07.txt", 250058342)]
        public void Part1(string filename, long expectedAnswer)
        {
            ScoreHands(filename, GetHandRank, GetCardRank)
                .Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day07_Test.txt", 5905)]
        [InlineData("Data/Day07.txt", 250506580)]
        public void Part2(string filename, long expectedAnswer)
        {
            ScoreHands(filename, GetHandRankJoker, GetCardRankJoker)
                .Should().Be(expectedAnswer);
        }
    }
}
