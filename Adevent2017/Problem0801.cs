using Adevent2017.Utils;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Adevent2017
{
    class RuntimeStats
    {
        public int FinalMaxRegisterValue = int.MinValue;
        public int LifetimeMaxGegisterValue = int.MinValue;
    }

    public class Problem0801
    {
        class Instruction
        {
            public string Register;
            public Func<int, int, int> Op;
            public int Value;
            public string CondRegister;
            public Func<int, int, bool> CondOp;
            public int CondValue;
        }

        Instruction Parse(string instruction)
        {
            var parts = instruction.Split(' ');
            Func<int, int, int> op;
            switch (parts[1])
            {
                case "inc":
                    op = (r, v) => r + v;
                    break;

                case "dec":
                    op = (r, v) => r - v;
                    break;

                default:
                    throw new Exception($"invalid instruction {parts[1]}");
            }

            Func<int, int, bool> condOp;
            switch (parts[5])
            {
                case ">":
                    condOp = (r, v) => r > v;
                    break;

                case "<":
                    condOp = (r, v) => r < v;
                    break;

                case ">=":
                    condOp = (r, v) => r >= v;
                    break;

                case "<=":
                    condOp = (r, v) => r <= v;
                    break;

                case "==":
                    condOp = (r, v) => r == v;
                    break;

                case "!=":
                    condOp = (r, v) => r != v;
                    break;

                default:
                    throw new Exception($"Invalid op {parts[5]}");
            }

            return new Instruction()
            {
                Register = parts[0],
                Op = op,
                Value = int.Parse(parts[2]),
                CondRegister = parts[4],
                CondOp = condOp,
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
