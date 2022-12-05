using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2022
{
    public class Day05
    {
        void EnsureStack<T>(List<T> stacks, int stackIndex) where T : IEnumerable<char>, new()
        {
            while (stacks.Count <= stackIndex)
            {
                stacks.Add(new());
            }
        }

        List<Stack<char>> Convert(List<List<char>> stacks)
        {
            List<Stack<char>> r = new();
            for (int i = 0; i < stacks.Count; i++)
            {
                Stack<char> s = new(stacks[i]);
                r.Add(s);
            }
            return r;
        }

        void AddToStacks(List<List<char>> stacks, string line)
        {
            for (int i = 1; i < line.Length; i += 4)
            {
                var c = line[i];
                if (c < 'A' || c > 'Z') continue;
                
                var stackIndex = (i - 1) / 4;
                EnsureStack(stacks, stackIndex);
                var stack = stacks[stackIndex];
                stack.Insert(0, c);
            }
        }

        List<Stack<char>> ParseStacks(Func<string> reader)
        {
            List<List<char>> stacks = new();

            while (true)
            {
                var line = reader();
                if (line == "") break;
                AddToStacks(stacks, line);
            }

            return Convert(stacks);
        }

        string TopLine(List<Stack<char>> stacks)
        {
            string result = "";
            foreach (var stack in stacks)
                result += stack.Pop();
            return result;
        }

        void Operate1(List<Stack<char>> stacks, string line)
        {
            var groups = line.Groups("move (\\d+) from (\\d+) to (\\d+)");
            var count = int.Parse(groups[0]);
            var fromIndex = int.Parse(groups[1]) - 1;
            var toIndex = int.Parse(groups[2]) - 1;

            EnsureStack(stacks, toIndex);
            while (count --> 0)
            {
                var c = stacks[fromIndex].Pop();
                stacks[toIndex].Push(c);
            }
        }

        void Operate2(List<Stack<char>> stacks, string line)
        {
            var groups = line.Groups("move (\\d+) from (\\d+) to (\\d+)");
            var count = int.Parse(groups[0]);
            var fromIndex = int.Parse(groups[1]) - 1;
            var toIndex = int.Parse(groups[2]) - 1;

            EnsureStack(stacks, toIndex);
            var buffer = new Stack<char>();
            while (count-- > 0)
            {
                var c = stacks[fromIndex].Pop();
                buffer.Push(c);
            }

            while (buffer.Count != 0)
            {
                stacks[toIndex].Push(buffer.Pop());
            }
        }

        string Solve(string filename, Action<List<Stack<char>>, string> op)
        {
            var reader = FileIterator.CreateLineReader(filename);
            var stacks = ParseStacks(reader);

            while (true)
            {
                var line = reader();
                if (line == null) break;
                op(stacks, line);
            }

            return TopLine(stacks);
        }

        [Theory]
        [InlineData("Data/Day05_Test.txt", "CMZ")]
        [InlineData("Data/Day05.txt", "BZLVHBWQF")]
        public void Part1(string filename, string expectedAnswer) => Solve(filename, Operate1).Should().Be(expectedAnswer);

        [Theory]
        [InlineData("Data/Day05_Test.txt", "MCD")]
        [InlineData("Data/Day05.txt", "TDGJQTZSL")]
        public void Part2(string filename, string expectedAnswer) => Solve(filename, Operate2).Should().Be(expectedAnswer);
    }
}
