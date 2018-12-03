using Utils;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Adevent2017
{
    using OpFunc = Func<int, int, int>;
    using CondFunc = Func<int, int, bool>;

    public class Problem0801
    {
        class RuntimeStats
        {
            public int FinalMaxRegisterValue = int.MinValue;
            public int LifetimeMaxGegisterValue = int.MinValue;
        }

        class Instruction
        {
            public string Register;
            public OpFunc Op;
            public int Value;
            public string CondRegister;
            public CondFunc CondOp;
            public int CondValue;
        }

        OpFunc ParseOp(string op)
        {
            switch (op)
            {
                case "inc":
                    return (r, v) => r + v;

                case "dec":
                    return (r, v) => r - v;

                default:
                    throw new Exception($"Invalid instruction {op}");
            }
        }

        CondFunc ParseCondOp(string condOp)
        {
            switch (condOp)
            {
                case ">":
                    return (r, v) => r > v;

                case "<":
                    return (r, v) => r < v;

                case ">=":
                    return (r, v) => r >= v;

                case "<=":
                    return (r, v) => r <= v;

                case "==":
                    return (r, v) => r == v;

                case "!=":
                    return (r, v) => r != v;

                default:
                    throw new Exception($"Invalid op {condOp}");
            }
        }

        Instruction Parse(string instruction)
        {
            var parts = instruction.Split(' ');

            return new Instruction()
            {
                Register = parts[0],
                Op = ParseOp(parts[1]),
                Value = int.Parse(parts[2]),
                CondRegister = parts[4],
                CondOp = ParseCondOp(parts[5]),
                CondValue = int.Parse(parts[6])
            };
        }

        RuntimeStats Exec(string[] input)
        {
            var stats = new RuntimeStats();
            var registers = new Dictionary<string, int>();
            
            foreach (var line in input)
            {
                var instruction = Parse(line);
                var regV = registers.GetOrDefault(instruction.CondRegister);
                var opV = instruction.CondValue;
                if (!instruction.CondOp(regV, opV)) continue;

                regV = registers.GetOrDefault(instruction.Register);
                opV = instruction.Value;
                var result = instruction.Op(regV, opV);
                registers[instruction.Register] = result;

                if (stats.LifetimeMaxGegisterValue < result)
                    stats.LifetimeMaxGegisterValue = result;
            }

             foreach (var v in registers.Values)
                if (stats.FinalMaxRegisterValue < v)
                    stats.FinalMaxRegisterValue = v;

            return stats;
        }

        [Theory]
        [InlineData("Data/0801-example.txt", 1)]
        [InlineData("Data/0801.txt", 3089)]
        public void Part1(string input, int answer)
        {
            var lines = FileIterator.LoadLines<string>(input);
            Exec(lines).FinalMaxRegisterValue.Should().Be(answer);
        }

        [Theory]
        [InlineData("Data/0801-example.txt", 10)]
        [InlineData("Data/0801.txt", 5391)]
        public void Part2(string input, int answer)
        {
            var lines = FileIterator.LoadLines<string>(input);
            Exec(lines).LifetimeMaxGegisterValue.Should().Be(answer);
        }
    }
}
