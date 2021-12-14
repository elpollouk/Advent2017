using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2021
{
    public class Day14
    {
        class Node
        {
            public char Id { get; init; }
            public Node Next;
        }

        static (Node template, Dictionary<(char, char), char> rules) LoadData(string filename)
        {
            Node template = null;
            Node last = null;
            Dictionary<(char, char), char> rules = new();

            var reader = FileIterator.CreateLineReader(filename);
            var line = reader();
            foreach (var c in line)
            {
                Node newNode = new() { Id = c };
                if (template == null)
                {
                    template = newNode;
                    last = newNode;
                }
                else
                {
                    last.Next = newNode;
                    last = newNode;
                }
            }
            reader();

            foreach (var mapping in FileIterator.Lines(reader))
            {
                var groups = mapping.Groups("(.)(.) -> (.)");
                rules.Add((groups[0][0], groups[1][0]), groups[2][0]);
            }

            return (template, rules);
        }

        static void Expand(Node poly, Dictionary<(char, char), char> rules)
        {
            while (poly.Next != null)
            {
                var c1 = poly.Id;
                var c2 = poly.Next.Id;
                var newId = rules[(c1, c2)];
                var newNode = new Node { Id = newId, Next = poly.Next };
                poly.Next = newNode;
                poly = newNode.Next;
            }
        }

        [Theory]
        [InlineData("Data/Day14_Test.txt", 10, 1588)]
        [InlineData("Data/Day14.txt", 10, 3095)]
        public void Part1(string filename, int iterations, long expectedAnswer)
        {
            (var p, var rules) = LoadData(filename);
            while (iterations --> 0)
            {
                Expand(p, rules);
            }

            var counts = new long[26];
            for (var node = p; node != null; node = node.Next)
            {
                counts[node.Id - 'A']++;
            }

            (var min, var max) = counts.Where(v => v != 0).MinAndMax();
            (max - min).Should().Be(expectedAnswer);
        }
        
    }
}
