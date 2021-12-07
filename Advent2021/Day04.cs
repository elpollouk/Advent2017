using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2021
{
    public class Day04
    {
        class Card
        {
            public int[,] board = new int[5, 5];
            public int[] columHits = new int[5];
            public int[] rowHits = new int[5];
            public Dictionary<int, (int, int)> numberToPosition = new();

            public void Set(int x, int y, int number)
            {
                board[x, y] = number;
                numberToPosition[number] = (x, y);
            }

            public bool Mark(int number)
            {
                (int x, int y) pos;
                if (!numberToPosition.TryGetValue(number, out pos)) return false;

                board[pos.x, pos.y] = 0;
                if (++columHits[pos.x] == 5) return true;
                if (++rowHits[pos.y] == 5) return true;

                return false;
            }

            public int Sum()
            {
                int sum = 0;
                foreach (int v in board)
                {
                    sum += v;
                }
                return sum;
            }
        }

        Dictionary<int, List<Card>> cardsByNumber = new();

        int[] ParseNumbers(Func<string> reader) => reader().Split(",").Select(v => int.Parse(v)).ToArray();

        void ParseCard(Func<string> reader, Dictionary<int, List<Card>> output)
        {
            Card card = new();

            for (var y = 0; y < 5; y++)
            {
                int[] row = reader().Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(v => int.Parse(v)).ToArray();
                for (var x = 0; x < 5; x++)
                {
                    var number = row[x];
                    card.Set(x, y, number);
                    var cards = output.GetOrCreate(number, () => new());
                    cards.Add(card);
                }
            }

        }

        [Theory]
        [InlineData("Data/Day04_Test.txt", 4512)]
        [InlineData("Data/Day04.txt", 11536)]
        public void Part1(string inputFile, int expectedAnswer)
        {
            Dictionary<int, List<Card>> cards = new();
            var reader = FileIterator.CreateLineReader(inputFile);
            var numbers = ParseNumbers(reader);
            while (reader() != null)
            {
                ParseCard(reader, cards);
            }

            int winValue = 0;

            foreach (var number in numbers)
            {
                if (!cards.TryGetValue(number, out var validCards)) continue;

                foreach (var card in validCards)
                {
                    if (card.Mark(number))
                    {
                        winValue = card.Sum() * number;
                        goto done;
                    }
                }
            }

        done:
            winValue.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day04_Test.txt", 1924)]
        [InlineData("Data/Day04.txt", 1284)]
        public void Part2(string inputFile, int expectedAnswer)
        {
            HashSet<Card> winninCards = new();

            Dictionary<int, List<Card>> cards = new();
            var reader = FileIterator.CreateLineReader(inputFile);
            var numbers = ParseNumbers(reader);
            while (reader() != null)
            {
                ParseCard(reader, cards);
            }

            int winValue = 0;

            foreach (var number in numbers)
            {
                if (!cards.TryGetValue(number, out var validCards)) continue;

                foreach (var card in validCards)
                {
                    if (winninCards.Contains(card)) continue;

                    if (card.Mark(number))
                    {
                        winValue = card.Sum() * number;
                        winninCards.Add(card);
                    }
                }
            }

            winValue.Should().Be(expectedAnswer);
        }
    }
}
