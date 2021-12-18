using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
using Xunit;

namespace Advent2021
{
    public class Day18
    {
        class Node
        {
            public Node Left;
            public Node Right;
            public long Value;

            public bool IsValue => Left == null;

            public bool IsExplodable => !IsValue && Left.IsValue && Right.IsValue;

            public long Magnitude
            {
                get
                {
                    if (IsValue) return Value;
                    return Left.Magnitude * 3 + Right.Magnitude * 2;
                }
            }

            public Node(string input)
            {
                int index = 1;
                Parse(input, ref index);
            }
            private Node(string input, ref int index)
            {
                Parse(input, ref index);
            }

            private Node(long value)
            {
                Value = value;
            }

            public Node(Node left, Node right)
            {
                Left = left;
                Right = right;
                Reduce();
            }

            public void Reduce()
            {
                long left = 0;
                long right = 0;
                while (true)
                {
                    if (Explode(0, ref left, ref right)) continue;
                    if (Split()) continue;
                    break;
                }
            }

            private bool Explode(int depth, ref long left, ref long right)
            {
                if (depth < 4)
                {
                    if (!Left.IsValue && Left.Explode(depth + 1, ref left, ref right))
                    {
                        if (right != 0)
                        {
                            Right.PassRight(right);
                            right = 0;
                        }
                        return true;
                    }
                    if (!Right.IsValue && Right.Explode(depth + 1, ref left, ref right))
                    {
                        if (left != 0)
                        {
                            Left.PassLeft(left);
                            left = 0;
                        }
                        return true;
                    }
                    return false;
                }

                if (!IsExplodable) throw new InvalidOperationException($"Node is not explodable: {ToString()}");
                left = Left.Value;
                right = Right.Value;
                Left = null;
                Right = null;
                Value = 0;

                return true;
            }

            private void PassRight(long value)
            {
                if (IsValue) Value += value;
                else Left.PassRight(value);
            }

            private void PassLeft(long value)
            {
                if (IsValue) Value += value;
                else Right.PassLeft(value);
            }

            private bool Split()
            {
                if (!IsValue)
                {
                    if (Left.Split()) return true;
                    return Right.Split();
                }

                if (Value <= 9) return false;

                Left = new Node(Value / 2);
                Right = new Node(Value - Left.Value);
                Value = 0;

                return true;
            }

            private void Parse(string input, ref int index)
            {
                Left = ParseNode(input, ref index);
                if (input[index++] != ',') throw new InvalidOperationException("Expected ','");
                Right = ParseNode(input, ref index);
                if (input[index++] != ']') throw new InvalidOperationException("Expected ']'");
            }

            private void WriteString(StringBuilder builder)
            {
                if (IsValue)
                {
                    builder.Append(Value);
                    return;
                }
                builder.Append('[');
                Left.WriteString(builder);
                builder.Append(',');
                Right.WriteString(builder);
                builder.Append(']');
            }

            public override string ToString()
            {
                var builder = new StringBuilder();
                WriteString(builder);
                return builder.ToString();
            }

            private static Node ParseNode(string input, ref int index)
            {
                var c = input[index++];
                return c switch
                {
                    '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9' => new Node(c - '0'),
                    '[' => new Node(input, ref index),
                    _ => throw new InvalidOperationException($"Unexpected input character: {c}"),
                };
            }
        }

        [Theory]
        [InlineData("[9,1]", 29)]
        [InlineData("[1,9]", 21)]
        [InlineData("[[9,1],[1,9]]", 129)]
        [InlineData("[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]", 3488)]
        public void Magnitude(string input, long expectedAnswer)
        {
            var node = new Node(input);
            node.Magnitude.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("[2,1]", "[2,1]")]
        [InlineData("[[3,2],1]", "[[3,2],1]")]
        [InlineData("[[[4,3],2],1]", "[[[4,3],2],1]")]
        [InlineData("[[[[5,4],3],2],1]", "[[[[5,4],3],2],1]")]
        [InlineData("[[[[[9,8],1],2],3],4]", "[[[[0,9],2],3],4]")]
        [InlineData("[7,[6,[5,[4,[3,2]]]]]", "[7,[6,[5,[7,0]]]]")]
        [InlineData("[[6,[5,[4,[3,2]]]],1]", "[[6,[5,[7,0]]],3]")]
        [InlineData("[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]", "[[3,[2,[8,0]]],[9,[5,[7,0]]]]")]
        [InlineData("[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]", "[[3,[2,[8,0]]],[9,[5,[7,0]]]]")]
        public void Reduce(string input, string expectedResult)
        {
            var node = new Node(input);
            node.Reduce();
            node.ToString().Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("[[[[4,3],4],4],[7,[[8,4],9]]]", "[1,1]", "[[[[0,7],4],[[7,8],[6,0]]],[8,1]]")]
        [InlineData("[1,1]", "[2,2]", "[3,3]", "[4,4]", "[[[[1,1],[2,2]],[3,3]],[4,4]]")]
        [InlineData("[1,1]", "[2,2]", "[3,3]", "[4,4]", "[5,5]", "[[[[3,0],[5,3]],[4,4]],[5,5]]")]
        public void Add(params string[] input)
        {
            var n = new Node(input[0]);
            for (var i = 1; i < input.Length - 1; i++)
            {
                n = new Node(n, new Node(input[i]));
            }
            n.ToString().Should().Be(input[^1]);
        }

        static Node AddNodes(string filename)
        {
            Node node = null;
            foreach (var line in FileIterator.Lines(filename))
            {
                if (node == null)
                {
                    node = new Node(line);
                }
                else
                {
                    node = new Node(node, new Node(line));
                }
            }
            return node;
        }

        [Theory]
        [InlineData("Data/Day18_Test1.txt", "[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]")]
        [InlineData("Data/Day18_Test2.txt", "[[[[6,6],[7,6]],[[7,7],[7,0]]],[[[7,7],[7,7]],[[7,8],[9,9]]]]")]
        public void Examples(string filename, string expectedAnswer)
        {
            Node node = AddNodes(filename);
            node.ToString().Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day18_Test2.txt", 4140)]
        [InlineData("Data/Day18.txt", 4207)]
        public void Part1(string filename, long expectedAnswer)
        {
            Node node = AddNodes(filename);
            node.Magnitude.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day18_Test.txt", 0)]
        [InlineData("Data/Day18.txt", 0)]
        public void Part2(string filename, long expectedAnswer)
        {

        }
    }
}
