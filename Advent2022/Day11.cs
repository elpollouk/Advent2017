using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2022
{
    public class Day11
    {
        static Func<long, long> OpMult(long value) => old => old * value;
        static Func<long, long> OpAdd(long value) => old => old + value;
        static Func<long, long> OpSquare() => old => old * old;

        static Func<long, long> GetOperation(string[] groups) => groups[0] switch
        {
            "+" => OpAdd(long.Parse(groups[1])),
            "*" => groups[1] == "old" ? OpSquare() : OpMult(long.Parse(groups[1])),
            _ => throw new Exception()
        };

        class Monkey
        {
            public static long Modulator { get; set; }

            public Queue<long> Items { get; } = new();
            public Func<long, long> Operation { get; init; }
            public long DivisibleCheck { get; init; }
            public int TargetTrue { get; init; }
            public int TargetFalse { get; init; }
            public long Inspections { get; set; }
            public List<Monkey> Monkies { get; init; }

            public void Process()
            {
                while (Items.Count > 0)
                {
                    var item = Items.Dequeue();
                    item = Operation(item);
                    if (Modulator != 0)
                    {
                        item %= Modulator;
                    }
                    else
                    {
                        item /= 3;
                    }

                    if (item % DivisibleCheck == 0)
                    {
                        Monkies[TargetTrue].Items.Enqueue(item);
                    }
                    else
                    {
                        Monkies[TargetFalse].Items.Enqueue(item);
                    }
                    Inspections++;
                }
            }
        }

        bool Parse(List<Monkey> monkies, Func<string> reader)
        {
            var initialLine = reader();
            if (initialLine == null) return false;
            if (initialLine.Length == 0) reader();

            var items = reader().Groups(@"Starting items: (.+)")[0].Split(',');
            var operation = GetOperation(reader().Groups(@"Operation: new = old (.) (.+)"));
            var divisibleCheck = long.Parse(reader().Groups(@"Test: divisible by (\d+)")[0]);
            var ifTrueMonkey = int.Parse(reader().Groups(@"If true: throw to monkey (\d+)")[0]);
            var ifFalseMonkey = int.Parse(reader().Groups(@"If false: throw to monkey (\d+)")[0]);

            var monkey = new Monkey()
            {
                Operation = operation,
                Monkies = monkies,
                DivisibleCheck = divisibleCheck,
                TargetTrue = ifTrueMonkey,
                TargetFalse = ifFalseMonkey
            };
            monkies.Add(monkey);

            foreach (var item in items)
                monkey.Items.Enqueue(long.Parse(item));

            return true;
        }

        [Theory]
        [InlineData("Data/Day11_Test.txt", 10605)]
        [InlineData("Data/Day11.txt", 113232)]
        public void Part1(string filename, long expectedAnswer)
        {
            List<Monkey> monkies = new();
            var reader = FileIterator.CreateLineReader(filename);
            while (Parse(monkies, reader)) { }

            Monkey.Modulator = 0;

            int rounds = 20;
            while (rounds --> 0)
            {
                foreach (var monkey in monkies)
                {
                    monkey.Process();
                }
            }

            monkies.Select(m => m.Inspections)
                .OrderDescending()
                .Take(2)
                .Product()
                .Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day11_Test.txt", 2713310158)]
        [InlineData("Data/Day11.txt", 29703395016)]
        public void Part2(string filename, long expectedAnswer)
        {
            List<Monkey> monkies = new();
            var reader = FileIterator.CreateLineReader(filename);
            while (Parse(monkies, reader)) { }

            Monkey.Modulator = monkies.Select(m => m.DivisibleCheck).Product();

            int rounds = 10000;
            while (rounds --> 0)
            {
                foreach (var monkey in monkies)
                {
                    monkey.Process();
                }
            }

            monkies.Select(m => m.Inspections)
                .OrderDescending()
                .Take(2)
                .Product()
                .Should().Be(expectedAnswer);
        }
    }
}
