using FluentAssertions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Utils;
using Xunit;

namespace Advent2020
{
    public class Day18
    {
        enum Op
        {
            Assign,
            Add,
            Mul,
        }

        private static long Apply(Op op, long v1, long v2)
        {
            // LISP would have been so much easier to implement
            return op switch
            {
                Op.Assign => v2,
                Op.Add => v1 + v2,
                Op.Mul => v1 * v2,
                _ => throw new NotImplementedException()
            };
        }

        private static long Evaluate(string input)
        {
            int index = 0;
            return Evaluate(input, ref index);
        }

        private static long Evaluate(string input, ref int index)
        {
            long acc = 0;
            Op op = Op.Assign;

            while (index < input.Length)
            {
                var c = input[index++];
                switch (c)
                {
                    case ' ':
                        break;

                    case '+':
                        op = Op.Add;
                        break;

                    case '*':
                        op = Op.Mul;
                        break;

                    case '(':
                        long result = Evaluate(input, ref index);
                        acc = Apply(op, acc, result);
                        break;

                    case ')':
                        return acc;

                    default:
                        acc = Apply(op, acc, c - '0');
                        break;
                }
            }
            return acc;
        }

        //---------------------------------------------------------------------------------------//
        // Including as a testimony to my utter madness
        // Real part 2 solution after the jump...
        //---------------------------------------------------------------------------------------//
        class Token
        {
            public char value;
            public Token prev;
            public Token next;
        }

        private static Token Parse(string input)
        {
            Token token = new()
            {
                value = input[0]
            };
            Token current = token;

            foreach (var c in input[1..])
            {
                if (c == ' ') continue;
                current.next = new();
                current.next.prev = current;
                current = current.next;
                current.value = c;
            }

            return token;
        }

        private static void BackTrackUpdate(Token token)
        {
            int braceLevel = 0;
            token = token.prev;
            while (token != null)
            {
                switch (token.value)
                {
                    case ')':
                        braceLevel++;
                        break;

                    case '(':
                        braceLevel--;
                        break;
                }

                if (braceLevel == 0)
                {
                    var newToken = new Token
                    {
                        value = '(',
                        prev = token.prev,
                        next = token
                    };
                    if (token.prev != null)
                    {
                        token.prev.next = newToken;
                        newToken.prev = token.prev;
                    }
                    token.prev = newToken;
                    return;
                }

                token = token.prev;
            }
        }

        private static void ForwardTrackUpdate(Token token)
        {
            int braceLevel = 0;
            token = token.next;
            while (token != null)
            {
                switch (token.value)
                {
                    case '(':
                        braceLevel++;
                        break;

                    case ')':
                        braceLevel--;
                        break;
                }

                if (braceLevel == 0)
                {
                    var newToken = new Token
                    {
                        value = ')',
                        prev = token,
                        next = token.next
                    };
                    if (token.next != null)
                    {
                        token.next.prev = newToken;
                        newToken.prev = token.prev;
                    }
                    token.next = newToken;
                    return;
                }

                token = token.next;
            }
        }

        private static void DebugDump(Token token)
        {
            while (token.prev != null)
                token = token.prev;
            var f = Flatten(token);
            Debug.WriteLine(f);
        }

        private static Token ApplyPrecedence(Token token)
        {
            var current = token.next;
            while (current != null)
            {
                if (current.value == '+')
                {
                    BackTrackUpdate(current);
                    ForwardTrackUpdate(current);
                    DebugDump(token);
                }
                current = current.next;
            }

            while (token.prev != null)
                token = token.prev;

            return token;
        }

        private static string Flatten(Token token)
        {
            var builder = new StringBuilder();
            while (token != null)
            {
                if (token.value == (char)0)
                    throw new Exception();
                builder.Append(token.value);
                token = token.next;
            }
            return builder.ToString();
        }

        private static long Evaluate2(string input)
        {
            var expression = Parse(input);
            expression = ApplyPrecedence(expression);
            var flattened = Flatten(expression);
            long result = Evaluate(flattened);
            return result;
        }

        //---------------------------------------------------------------------------------------//
        // End of insanity
        // This is why you never role your own if real work is on the line
        //---------------------------------------------------------------------------------------//

        private static string FortranExpand(string input)
        {
            // Adapted from:
            // https://en.wikipedia.org/wiki/Operator-precedence_parser#Alternative_methods
            StringBuilder b = new();

            b.Append("(((");
            for (int i = 0; i != input.Length; i++)
            {
                switch (input[i])
                {
                    case '(': b.Append("((("); continue;
                    case ')': b.Append(")))"); continue;
                    case '*': b.Append("))*(("); continue;
                    case '+': b.Append(")+("); continue;
                    case ' ': continue;
                }
                b.Append(input[i]);
            }
            b.Append(")))");

            return b.ToString();
        }

        [Theory]
        [InlineData("2 * 3 + (4 * 5)", 26)]
        [InlineData("5 + (8 * 3 + 9 + 3 * 4 * 3)", 437)]
        [InlineData("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", 12240)]
        [InlineData("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", 13632)]
        public void TestEvaluate(string input, long expected) => Evaluate(input).Should().Be(expected);

        [Fact]
        public void Problem1() => FileIterator.Lines("Data/Day18.txt")
            .Select(Evaluate)
            .Sum()
            .Should().Be(800602729153);


        [Theory]
        [InlineData("1 + 2", 3)]
        [InlineData("(1 + 2) * 3", 9)]
        [InlineData("3 * 2 + 1", 9)]
        [InlineData("1 + (2 * 3)", 7)]
        [InlineData("1 + (2 * 3) + (4 * (5 + 6))", 51)]
        [InlineData("2 * 3 + (4 * 5)", 46)]
        [InlineData("5 + (8 * 3 + 9 + 3 * 4 * 3)", 1445)]
        [InlineData("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", 669060)]
        [InlineData("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", 23340)]
        [InlineData(
            "(6 + 5 * 3 * 9 + 6 + (5 + 8 * 2 + 7))",
            ((6 + 5) * 3 * ((9 + 6) + ((5 + 8) * (2 + 7))))
        )]
        [InlineData(
            "((9 * 6) + 8 * 6) + (9 + 9 * 6 + 9) + (8 * 6 + 5 * 4 + 3) + 5 * 4 * (6 + 5 * 3 * 9 + 6 + (5 + 8 * 2 + 7))",
            ((((((9 * 6) + 8) * 6) + ((9 + 9) * (6 + 9))) + (8 * (6 + 5) * (4 + 3))) + 5) * 4 * ((6 + 5) * 3 * ((9 + 6) + ((5 + 8) * (2 + 7))))
        )]
        [InlineData(
            "((6 * 9 + 2 + 8 + 4) + 6 * 7 + 8 + (2 + 5 + 7 + 5 + 3 * 8) * 5) * 6 + (2 + 4) * 7 + (3 + 2 * (4 * 8 * 5) * 2 + 7 * 9)",
            (
                ((
                    6L * (((9 + 2) + 8) + 4)
                ) + 6) * ((7 + 8) + (
                    ((((2 + 5) + 7) + 5) + 3) * 8
                )) * 5
            ) * (6 + (
                (2 + 4)
            )) * (7 + (
                (3 + 2) * (
                    4 * 8 * 5
                )) * (2 + 7) * 9
            )
        )]
        public void TestEvaluate2(string input, long expected)
        {
            var expanded = FortranExpand(input);
            Evaluate(expanded).Should().Be(expected);
        }

        // 2342647541014 - Too low
        // 92173009047076 - Just right?
        // 92173009470705 - Too high
        [Fact]
        public void Problem2() => FileIterator.Lines("Data/Day18.txt")
            .Select(FortranExpand)
            .Select(Evaluate2)
            .Sum()
            .Should().Be(92173009047076);
    }
}
