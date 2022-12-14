using FluentAssertions;
using System.Collections.Generic;
using Utils;
using Xunit;

namespace Advent2022
{
    public class Day13
    {
        class ValueOrList
        {
            public readonly bool IsValue;
            public readonly int Value;
            public readonly List<ValueOrList> List;

            public ValueOrList(int value)
            {
                IsValue = true;
                Value = value;
                List = null;
            }

            public ValueOrList(List<ValueOrList> list)
            {
                IsValue = false;
                Value = int.MinValue;
                List = list;
            }

            public int Count => List.Count;

            public ValueOrList this[int index] => List[index];

            public ValueOrList AsList()
            {
                if (IsValue)
                {
                    var list = new List<ValueOrList>();
                    list.Add(this);
                    return new(list);
                }
                return this;
            }

            public int Compare(ValueOrList right)
            {
                var left = this;

                if (left.IsValue && right.IsValue)
                {
                    return left.Value - right.Value;
                }

                left = left.AsList();
                right = right.AsList();

                for (int i = 0; i < left.Count; i++)
                {
                    if (i == right.Count) return 1;
                    var v = left[i].Compare(right[i]);
                    if (v != 0) return v;
                }

                if (left.Count < right.Count) return -1;
                return 0;
            }

            public override string ToString()
            {
                if (IsValue)
                {
                    return Value.ToString();
                }
                else
                {
                    var r = "[";
                    foreach (var v in List)
                    {
                        if (r.Length != 1)
                        {
                            r += ",";
                        }
                        r += v.ToString();
                    }
                    return r + "]";
                }
            }
        }

        ValueOrList ParseNumber(string line, ref int index)
        {
            int value = 0;
            while (true)
            {
                var c = line[index];
                switch (c)
                {
                    case ',':
                    case ']':
                        return new(value);

                    default:
                        value *= 10;
                        value += c - '0';
                        index++;
                        break;
                }
            }
        }

        ValueOrList ParseList(string line, ref int index)
        {
            List<ValueOrList> list = new();
            while (true)
            {
                var c = line[index++];
                switch (c)
                {
                    case ',':
                    {
                        break;
                    }

                    case '[':
                    {
                        var result = ParseList(line, ref index);
                        list.Add(result);
                        break;
                    }

                    case ']':
                    {
                        return new(list);
                    }

                    default:
                    {
                        index--;
                        var result = ParseNumber(line, ref index);
                        list.Add(result);
                        break;
                    }
                }
            }
        }

        [Theory]
        [InlineData("Data/Day13_Test.txt", 13)]
        [InlineData("Data/Day13.txt", 5529)]
        public void Part1(string filename, int expectedAnswer)
        {
            var reader = FileIterator.CreateLineReader(filename);
            int pair = 1;
            int sum = 0;

            while (true)
            {
                var line = reader();
                int index = 1;
                var left = ParseList(line, ref index);

                line = reader();
                index = 1;
                var right = ParseList(line, ref index);

                if (left.Compare(right) < 0)
                {
                    sum += pair;
                }
                pair++;

                line = reader();
                if (line == null) break;
            }

            sum.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day13_Test.txt", 140)]
        [InlineData("Data/Day13.txt", 27690)]
        public void Part2(string filename, int expectedAnswer)
        {
            var reader = FileIterator.CreateLineReader(filename);
            List<ValueOrList> packets = new();
            var addToList = (string s) =>
            {
                int index = 1;
                packets.Add(ParseList(s, ref index));
            };

            addToList("[[2]]");
            addToList("[[6]]");

            while (true)
            {
                var line = reader();
                addToList(line);

                line = reader();
                addToList(line);

                line = reader();
                if (line == null) break;
            }

            packets.Sort((left, right) => left.Compare(right));
            int a = -1;
            int b = -1;
            for (int i = 0; i < packets.Count; i++)
            {
                var packet = packets[i];
                if (packet.ToString() == "[[2]]") a = i + 1;
                else if (packet.ToString() == "[[6]]") b = i + 1;
            }

            (a * b).Should().Be(expectedAnswer);
        }
    }
}
