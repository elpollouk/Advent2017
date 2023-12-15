using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Utils;
using Xunit;

namespace Advent2023
{
    public class Day15
    {
        class LensRef(string label, int focalLength)
        {
            public readonly string Label = label;
            public int FocalLength = focalLength;
            public readonly int Box = Hash(label);
            public LensRef Previous;
            public LensRef Next;

            public void Unlink()
            {
                if (Previous != null)
                {
                    Previous.Next = Next;
                }
                if (Next != null)
                {
                    Next.Previous = Previous;
                }
                Previous = null;
                Next = null;
            }
        }

        static int Hash(string text)
        {
            int hash = 0;

            foreach (var c in text)
            {
                hash += c;
                hash *= 17;
                hash %= 256;
            }

            return hash;
        }

        [Theory]
        [InlineData("HASH", 52)]
        [InlineData("rn", 0)]
        [InlineData("qp", 1)]
        public void HashTest(string input, int expectedValue) => Hash(input).Should().Be(expectedValue);

        [Theory]
        [InlineData("Data/Day15_Test.txt", 1320)]
        [InlineData("Data/Day15.txt", 506891)]
        public void Part1(string filename, long expectedAnswer)
        {
            var line = FileIterator.CreateLineReader(filename)();
            var parts = line.Split(',');

            long total = 0;
            foreach (var part in parts)
            {
                total += Hash(part);
            }
            total.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day15_Test.txt", 145)]
        [InlineData("Data/Day15.txt", 230462)]
        public void Part2(string filename, long expectedAnswer)
        {
            var boxesHead = new LensRef[256];
            var boxesTail = new LensRef[256];
            Dictionary<string, LensRef> lenses = [];

            void Remove(LensRef lens)
            {
                if (lens != null)
                {
                    if (lens.Previous == null)
                    {
                        boxesHead[lens.Box] = lens.Next;
                    }
                    if (lens.Next == null)
                    {
                        boxesTail[lens.Box] = lens.Previous;
                    }
                    lens.Unlink();
                    lenses.Remove(lens.Label);
                }
            }

            void Add(string label, int focalLength)
            {
                LensRef lens = new(label, focalLength);
                lens.Previous = boxesTail[lens.Box];
                if (lens.Previous != null)
                {
                    lens.Previous.Next = lens;
                }
                else
                {
                    boxesHead[lens.Box] = lens;
                }
                boxesTail[lens.Box] = lens;
                lenses[lens.Label] = lens;
            }

            var line = FileIterator.CreateLineReader(filename)();
            var parts = line.Split(',');
            foreach (var part in parts)
            {
                var match = part.Match(@"(\w+)([-=])(\d+)?");
                var label = match.Groups[1].Value;
                var operation = match.Groups[2].Value;

                var lens = lenses.GetOrDefault(label);
                
                if (operation == "-")
                {
                    Remove(lens);
                }
                else if (lens != null)
                {
                    lens.FocalLength = int.Parse(match.Groups[3].Value);
                }
                else
                {
                    Add(label, int.Parse(match.Groups[3].Value));
                }
            }

            long total = 0;

            for (int i = 0; i < boxesHead.Length; i++)
            {
                int lensIndex = 1;
                var lens = boxesHead[i];
                while (lens != null)
                {
                    total += (i + 1) * lensIndex * lens.FocalLength;
                    lensIndex++;
                    lens = lens.Next;
                }
            }

            total.Should().Be(expectedAnswer);
        }
    }
}
