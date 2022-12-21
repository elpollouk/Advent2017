using FluentAssertions;
using System;
using System.Collections.Generic;
using Utils;
using Xunit;

namespace Advent2022
{
    public class Day21
    {
        class Monkey
        {
            public readonly string Name;
            public Monkey DepA;
            public Monkey DepB;
            public long Result = long.MinValue;
            public Func<Monkey, Monkey, long> Op;
            public Func<long, Monkey, long> SolveForA;
            public Func<long, Monkey, long> SolveForB;

            public Monkey(string name)
            {
                Name = name;
            }

            public long Value
            {
                get
                {
                    if (Result == long.MinValue)
                    {
                        Result = Op(DepA, DepB);
                    }
                    return Result;
                }
            }

            public long Solve(long target)
            {
                if (Name == "humn") return target;

                Monkey knownMonkey;
                Monkey unknownMonkey;
                Func<long, Monkey, long> solver;
                if (DepA.HasHuman())
                {
                    knownMonkey = DepB;
                    unknownMonkey = DepA;
                    solver = SolveForA;
                }
                else
                {
                    knownMonkey = DepA;
                    unknownMonkey = DepB;
                    solver = SolveForB;
                }

                target = solver(target, knownMonkey);
                return unknownMonkey.Solve(target);
            }

            public bool HasHuman()
            {
                if (Name == "humn") return true;
                if (DepA == null) return false;
                return DepA.HasHuman() || DepB.HasHuman();
            }

            public override string ToString()
            {
                return Name;
            }
        }

        static long Add(Monkey a, Monkey b) => a.Value + b.Value;
        static long AddSolve(long target, Monkey m) => target - m.Value;

        static long Sub(Monkey a, Monkey b) => a.Value - b.Value;
        static long SubSolveForA(long target, Monkey b) => target + b.Value;
        static long SubSolveForB(long target, Monkey a) => a.Value - target;

        static long Mul(Monkey a, Monkey b) => a.Value * b.Value;
        static long MulSolve(long target, Monkey m) => target / m.Value;

        static long Div(Monkey a, Monkey b) => a.Value / b.Value;
        static long DivSolveForA(long target, Monkey b) => target * b.Value;
        static long DivSolveForB(long target, Monkey a) => a.Value / target;

        Dictionary<string, Monkey> Parse(string filename)
        {
            Dictionary<string, Monkey> monkeys = new();

            foreach (var line in FileIterator.Lines(filename))
            {
                var parts = line.Split(": ");
                var monkey = monkeys.GetOrCreate(parts[0], () => new Monkey(parts[0]));

                var groups = parts[1].Groups(@"([a-z]+) (.) ([a-z]+)");
                if (groups.Length == 0)
                {
                    monkey.Result = long.Parse(parts[1]);
                }
                else
                {
                    monkey.DepA = monkeys.GetOrCreate(groups[0], () => new Monkey(groups[0]));
                    monkey.DepB = monkeys.GetOrCreate(groups[2], () => new Monkey(groups[2]));
                    switch (groups[1][0])
                    {
                        case '+':
                            monkey.Op = Add;
                            monkey.SolveForA = AddSolve;
                            monkey.SolveForB = AddSolve;
                            break;

                        case '-':
                            monkey.Op = Sub;
                            monkey.SolveForA = SubSolveForA;
                            monkey.SolveForB = SubSolveForB;
                            break;

                        case '*':
                            monkey.Op = Mul;
                            monkey.SolveForA = MulSolve;
                            monkey.SolveForB = MulSolve;
                            break;

                        case '/':
                            monkey.Op = Div;
                            monkey.SolveForA = DivSolveForA;
                            monkey.SolveForB = DivSolveForB;
                            break;

                        default:
                            throw new Exception();
                    };
                }
            }

            return monkeys;
        }

        [Theory]
        [InlineData("Data/Day21_Test.txt", 152)]
        [InlineData("Data/Day21.txt", 194501589693264)]
        public void Part1(string filename, long expectedAnswer)
        {
            var monkeys = Parse(filename);
            monkeys["root"].Value.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day21_Test.txt", 301)]
        [InlineData("Data/Day21.txt", 3887609741189)]
        public void Part2(string filename, long expectedAnswer)
        {
            var monkeys = Parse(filename);
            var root = monkeys["root"];
            long target;
            Monkey unknownMonkey;
            if (root.DepA.HasHuman())
            {
                target = root.DepB.Value;
                unknownMonkey = root.DepA;
            }
            else
            {
                target = root.DepA.Value;
                unknownMonkey = root.DepB;
            }

            unknownMonkey.Solve(target).Should().Be(expectedAnswer);
        }
    }
}
