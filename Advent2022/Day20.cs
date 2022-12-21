using FluentAssertions;
using System;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2022
{
    public class Day20
    {
        class Entry
        {
            public long Value;
            public Entry Left;
            public Entry Right;

            public Entry(long value)
            {
                Value = value;
            }

            public void MoveLeft()
            {
                var oldLeft = Left;
                Left = Left.Left;
                Left.Right = this;
                oldLeft.Right = Right;
                Right.Left = oldLeft;
                Right = oldLeft;
                oldLeft.Left = this;
            }

            public void MoveRight()
            {
                var oldRight = Right;
                Right = Right.Right;
                Right.Left = this;
                oldRight.Left = Left;
                Left.Right = oldRight;
                Left = oldRight;
                oldRight.Right = this;
            }

            public override string ToString()
            {
                return $"{Value}";
            }
        }

        void Link(Entry[] entries)
        {
            for (int i = 0; i < entries.Length; i++)
            {
                var left = entries.GetAtMod(i - 1);
                var right = entries.GetAtMod(i + 1);
                var entry = entries[i];
                entry.Left = left;
                left.Right = entry;
                entry.Right = right;
                right.Left = entry;
            }
        }

        [Fact]
        public void MoveLeft()
        {
            var entries = new Entry[] {
                new(1),
                new(2),
                new(3),
                new(4),
            };
            Link(entries);

            entries[0].MoveLeft();

            entries[0].Left.Should().Be(entries[2]);
            entries[0].Right.Should().Be(entries[3]);
            entries[1].Left.Should().Be(entries[3]);
            entries[1].Right.Should().Be(entries[2]);
            entries[2].Left.Should().Be(entries[1]);
            entries[2].Right.Should().Be(entries[0]);
            entries[3].Left.Should().Be(entries[0]);
            entries[3].Right.Should().Be(entries[1]);
        }

        [Fact]
        public void MoveRight()
        {
            var entries = new Entry[] {
                new(1),
                new(2),
                new(3),
                new(4),
            };
            Link(entries);

            entries[0].MoveRight();

            entries[0].Left.Should().Be(entries[1]);
            entries[0].Right.Should().Be(entries[2]);
            entries[1].Left.Should().Be(entries[3]);
            entries[1].Right.Should().Be(entries[0]);
            entries[2].Left.Should().Be(entries[0]);
            entries[2].Right.Should().Be(entries[3]);
            entries[3].Left.Should().Be(entries[2]);
            entries[3].Right.Should().Be(entries[1]);
        }

        [Theory]
        [InlineData("Data/Day20_Test.txt",1, 1, 3)]
        [InlineData("Data/Day20.txt", 1, 1, 11073)]
        [InlineData("Data/Day20_Test.txt", 10, 811589153, 1623178306)]
        [InlineData("Data/Day20.txt", 10, 811589153, 11102539613040)]
        public void Solution(string filename, int rounds, long multiplier, long expectedAnswer)
        {
            var entries = FileIterator.Lines(filename)
                .Select(line => new Entry(int.Parse(line)))
                .ToArray();
            Link(entries);

            foreach (var entry in entries)
            {
                entry.Value *= multiplier;
            }

            Action move;
            while (rounds --> 0)
            {
                foreach (var entry in entries)
                {
                    var count = entry.Value;
                    if (count < 0)
                    {
                        count = -entry.Value;
                        move = entry.MoveLeft;
                    }
                    else
                    {
                        move = entry.MoveRight;
                    }

                    count %= entries.Length - 1;
                    while (count --> 0)
                    {
                        move();
                    }
                }
            }

            {
                long sum = 0;
                var entry = entries[0];
                while (entry.Value != 0)
                {
                    entry = entry.Right;
                }

                for (int i = 0; i < 3000; i++)
                {
                    entry = entry.Right;
                    if (i == 999) sum += entry.Value;
                    else if (i == 1999) sum += entry.Value;
                    else if (i == 2999) sum += entry.Value;
                }

                sum.Should().Be(expectedAnswer);
            }
        }
    }
}
