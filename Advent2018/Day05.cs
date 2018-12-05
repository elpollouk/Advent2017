using FluentAssertions;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Advent2018
{
    public class Day05
    {
        int UnitToValue(char unit)
        {
            if (('A' <= unit) && (unit <= 'Z'))
                return (unit - 'A') + 1; // We want the item to have a non-zero value
            return -((unit - 'a') + 1);
        }

        int Collapse(string input, params char[] removals)
        {
            var set = new HashSet<char>(removals);
            var list = new LinkedList<int>();
            foreach (var unit in input)
                if (!set.Contains(unit))
                    list.AddLast(UnitToValue(unit));

            var node = list.First;
            while (node != list.Last)
            {
                if (node.Value + node.Next.Value == 0)
                {
                    var prevNode = node.Previous;
                    list.Remove(node.Next);
                    list.Remove(node);
                    node = prevNode ?? list.First;
                }
                else
                {
                    node = node.Next;
                }
            }

            return list.Count;
        }

        [Theory]
        [InlineData("aA", 0)]
        [InlineData("Aa", 0)]
        [InlineData("abBA", 0)]
        [InlineData("aabAAB", 6)]
        [InlineData("dabAcCaCBAcCcaDA", 10)]
        public void Problem1_Test(string input, int expectedLength) => Collapse(input).Should().Be(expectedLength);

        [Fact]
        public void Problem1_Solution()
        {
            var input = File.ReadAllText("Data/Day05.txt");
            Collapse(input).Should().Be(9370);
        }

        [Theory]
        [InlineData("dabAcCaCBAcCcaDA", 4)]
        public void Problem2_Test(string input, int expectedLenth)
        {
            var shortest = input.Length;
            for (var i = 0; i < 26; i++)
            {
                var length = Collapse(input, (char)('a' + i), (char)('A' + i));
                if (length < shortest)
                    shortest = length;
            }

            shortest.Should().Be(expectedLenth);
        }

        [Fact]
        public void Problem2_Solution()
        {
            var input = File.ReadAllText("Data/Day05.txt");
            Problem2_Test(input, 6390);
        }
    }
}
