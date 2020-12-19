using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2020
{
    public class Day19
    {
        class Rule
        {
            public readonly int id;
            public readonly char match;
            public readonly List<int[]> subRulesSets;
            //public readonly HashSet<string> fullMatches = new();

            public Rule(string rule)
            {
                var split = rule.Split(": ");
                id = int.Parse(split[0]);
                if (split[1][0] == '"')
                {
                    match = split[1][1];
                    subRulesSets = null;
                    
                }
                else
                {
                    match = '\0';
                    subRulesSets = new();
                    split = split[1].Split(" | ");
                    foreach (var ruleSet in split)
                        subRulesSets.Add(ruleSet.SplitAndConvert<int>(' '));
                }
            }
        }

        private const int NOT_MATCHED = -1;

        private static int ValidateMessage(Dictionary<int, Rule> rules, string message, Stack<(int, int, int, int)> stack)
        {
            var (index, ruleId, subRuleSetIndex, subRuleIndex) = stack.Pop();
            var rule = rules[ruleId];

            if (message.Length == index)
                return NOT_MATCHED;

            if (rule.match != '\0')
                return message[index] == rule.match ? index + 1 : NOT_MATCHED;

            var nextValidIndex = index;
            while (subRuleSetIndex < rule.subRulesSets.Count)
            //foreach (var subRuleSet in rule.subRulesSets)
            {
                var subRuleSet = rule.subRulesSets[subRuleSetIndex];
                while (subRuleIndex < subRuleSet.Length)
                //foreach (var subRuleId in subRuleSet)
                //for (var i = 0; i < subRuleSet.Length; i++)
                {
                    var subRuleId = subRuleSet[subRuleIndex];
                    stack.Push((nextValidIndex, subRuleId, 0, 0));
                    nextValidIndex = ValidateMessage(rules, message, stack);
                    if (nextValidIndex == NOT_MATCHED)
                    {
                        // Roll back to the index for the next rules set in the list, we weren't able to match any of these sub rules
                        nextValidIndex = index;
                        break;
                    }
                    subRuleIndex++;
                }

                // We advanced the message index, so we much have found a matching sub rules set
                if (index < nextValidIndex) return nextValidIndex;
                subRuleSetIndex++;
                subRuleIndex = 0;
            }

            // If we dropped out of the bottom of the loop, no sub rules sets must have matched so this rule isn't a match
            return NOT_MATCHED;
        }

        

        private static bool ValidateMessage(Dictionary<int, Rule> rules, string message)
        {
            Stack<(int, int, int, int)> stack = new();
            stack.Push((0, 0, 0, 0));
            var index = ValidateMessage(rules, message, stack);
            return index == message.Length;
        }

        private static Dictionary<int, Rule> LoadRules(string input)
        {
            Dictionary<int, Rule> rules = new();

            foreach (var line in FileIterator.Lines(input))
            {
                if (line == "") break;
                var rule = new Rule(line);
                rules[rule.id] = rule;
            }

            return rules;
        }

        private static IEnumerable<string> LoadMessages(string input)
        {
            var foundRules = false;
            foreach (var line in FileIterator.Lines(input))
            {
                if (foundRules)
                {
                    if (line[0] != '#') yield return line;
                }
                else if (line == "")
                {
                    foundRules = true;
                }
            }
        }

        [Fact]
        public void TestParseStringRule()
        {
            var rule = new Rule("128: \"b\"");
            rule.id.Should().Be(128);
            rule.match.Should().Be('b');
            rule.subRulesSets.Should().BeNull();
        }

        [Fact]
        public void TestParseSubRules()
        {
            var rule = new Rule("1: 90 112 | 128 103");
            rule.id.Should().Be(1);
            rule.match.Should().Be('\0');
            rule.subRulesSets.Should().BeEquivalentTo(new int[][] {
                new[]{ 90, 112 },
                new[]{ 128, 103 }
            });
        }

        [Theory]
        [InlineData("Data/Day19_test.txt", 2)]
        [InlineData("Data/Day19_test2.txt", 3)]
        [InlineData("Data/Day19.txt", 102)]
        public void Problem1(string input, int expected)
        {
            var rules = LoadRules(input);
            LoadMessages(input)
                .Where(m => ValidateMessage(rules, m))
                .Count()
                .Should().Be(expected);
        }

        // 145 - Too low
        [Theory]
        [InlineData("Data/Day19_test2.txt", 12)]
        //[InlineData("Data/Day19.txt", 0)]
        public void Problem2(string input, int expected)
        {
            var rules = LoadRules(input);
            rules[8] = new Rule("8: 42 | 42 8");
            rules[11] = new Rule("11: 42 31 | 42 11 31");
            var messages = LoadMessages(input);
            LoadMessages(input)
                .Where(m => ValidateMessage(rules, m))
                .Count()
                .Should().Be(expected);
        }
    }
}
