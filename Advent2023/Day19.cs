using FluentAssertions;
using System;
using System.Collections.Generic;
using Utils;
using Xunit;

namespace Advent2023
{
    public class Day19
    {
        record Widget(long x, long m, long a, long s);

        enum Result
        {
            Continue,
            Accept,
            Reject,
        };

        delegate Result Process(Widget w);

        Func<Widget, bool> ParseCondition(string condition)
        {
            var match = condition.Match(@"^(.)(.)(\d+)$");
            long value = long.Parse(match.Groups[3].Value);
            if (match.Groups[2].Value == "<")
            {
                return match.Groups[1].Value switch
                {
                    "x" => w => w.x < value,
                    "m" => w => w.m < value,
                    "a" => w => w.a < value,
                    "s" => w => w.s < value,
                    _ => throw new Exception()
                };
            }

            return match.Groups[1].Value switch
            {
                "x" => w => w.x > value,
                "m" => w => w.m > value,
                "a" => w => w.a > value,
                "s" => w => w.s > value,
                _ => throw new Exception()
            };
        }

        Process ParseProcess(Func<string> reader)
        {
            Dictionary<string, Process> functions = [];
            var line = reader();
            while (!string.IsNullOrEmpty(line))
            {
                var match = line.Match(@"^(\w+){(.+)}$");
                var name = match.Groups[1].Value;
                var instructions = match.Groups[2].Value.Split(',');
                var steps = new Process[instructions.Length];

                Result Accept(Widget w) => Result.Accept;
                Result Reject(Widget w) => Result.Reject;

                for (int i = 0; i< instructions.Length; i++)
                {
                    var parts = instructions[i].Split(':');
                    string target = parts.Length == 2 ? parts[1] : parts[0];

                    Process next = target switch
                    {
                        "A" => Accept,
                        "R" => Reject,
                        _ => w => functions[target](w)
                    };

                    if (parts.Length == 2)
                    {
                        var condition = ParseCondition(parts[0]);
                        steps[i] = w =>
                        {
                            if (condition(w))
                                return next(w);
                            return Result.Continue;
                        };
                    }
                    else
                    {
                        steps[i] = next;
                    }
                }

                functions[name] = w =>
                {
                    foreach (var step in steps)
                    {
                        var result = step(w);
                        switch (step(w))
                        {
                            case Result.Accept:
                            case Result.Reject:
                                return result;
                        }
                    }
                    throw new Exception();
                };

                line = reader();
            }

            return functions["in"];
        }

        List<Widget> ParseWidgets(Func<string> reader)
        {
            List<Widget> widgets = [];

            var line = reader();
            while (line != null)
            {
                var match = line.Match(@"^{x=(\d+),m=(\d+),a=(\d+),s=(\d+)}$");
                widgets.Add(new(
                    long.Parse(match.Groups[1].Value),
                    long.Parse(match.Groups[2].Value),
                    long.Parse(match.Groups[3].Value),
                    long.Parse(match.Groups[4].Value)
                ));
                line = reader();
            }

            return widgets;
        }

        long Sum(Widget widget)
        {
            return widget.x + widget.m + widget.a + widget.s;
        }

        [Theory]
        [InlineData("Data/Day19_Test.txt", 19114)]
        [InlineData("Data/Day19.txt", 492702)]
        public void Part1(string filename, long expectedAnswer)
        {
            var reader = FileIterator.CreateLineReader(filename);
            var process = ParseProcess(reader);
            var widgets = ParseWidgets(reader);

            long total = 0;

            foreach (var widget in widgets)
            {
                var result = process(widget);
                if (result == Result.Accept)
                {
                    total += Sum(widget);
                }
            }

            total.Should().Be(expectedAnswer);
        }


        //---------------------------------------------------------------------------------------//
        // Part 2
        //---------------------------------------------------------------------------------------//

        record Range(int min, int max);

        record Check(char field, int value, char op, string target);

        Dictionary<char, Range> ForkLower(Dictionary<char, Range> widget, char field, int value)
        {
            Dictionary<char, Range> newWidget = new(widget);
            Range lower = new(widget[field].min, value);
            Range upper = new(value, widget[field].max);

            newWidget[field] = lower;
            widget[field] = upper;

            return newWidget;
        }

        long ValidCount(Dictionary<char, Range> widget)
        {
            long total = 1;
            foreach (var range in widget.Values)
            {
                total *= range.max - range.min;
            }
            return total;
        }

        Dictionary<string, List<Check>> ParseChecks(Func<string> reader)
        {
            Dictionary<string, List<Check>> functions = [];

            var line = reader();
            while (!string.IsNullOrEmpty(line))
            {
                var match = line.Match(@"^(\w+){(.+)}$");
                var name = match.Groups[1].Value;
                var steps = match.Groups[2].Value.Split(',');
                List<Check> checks = [];

                foreach (var step in steps)
                {
                    var parts = step.Split(":");
                    if (parts.Length == 1)
                    {
                        checks.Add(new('x', int.MaxValue, '<', parts[0]));
                    }
                    else
                    {
                        match = parts[0].Match(@"^(.)(.)(\d+)$");
                        var field = match.Groups[1].Value[0];
                        var op = match.Groups[2].Value[0];
                        var value = int.Parse(match.Groups[3].Value);
                        checks.Add(new(field, value, op, parts[1]));
                    }
                }

                functions[name] = checks;
                line = reader();
            }

            return functions;
        }

        long Solve(Dictionary<string, List<Check>> functions, string function, Dictionary<char, Range> widget)
        {
            if (function == "A")
            {
                return ValidCount(widget);
            }
            else if (function == "R")
            {
                return 0;
            }

            long total = 0;
            var checks = functions[function];
            foreach (var check in checks)
            {
                var range = widget[check.field];
                if (check.op == '<')
                {
                    if (check.value <= range.min)
                    {
                        continue;
                    }

                    if (range.max <= check.value)
                    {
                        return total + Solve(functions, check.target, widget);
                    }

                    var lower = ForkLower(widget, check.field, check.value);
                    total += Solve(functions, check.target, lower);
                }
                else // (check.op == '>')
                {
                    if (range.max < check.value + 1)
                    {
                        continue;
                    }

                    if (range.min > check.value)
                    {
                        return total + Solve(functions, check.target, widget);
                    }

                    var lower = ForkLower(widget, check.field, check.value + 1);
                    total += Solve(functions, check.target, widget);
                    widget = lower;
                }
            }

            return total;
        }

        [Theory]
        [InlineData("Data/Day19_Test.txt", 167409079868000)]
        [InlineData("Data/Day19.txt", 138616621185978)]
        public void Part2(string filename, long expectedAnswer)
        {
            var reader = FileIterator.CreateLineReader(filename);
            var functions = ParseChecks(reader);

            Dictionary<char, Range> widget = new() {
                ['x'] = new(1, 4001),
                ['m'] = new(1, 4001),
                ['a'] = new(1, 4001),
                ['s'] = new(1, 4001)
            };

            var total = Solve(functions, "in", widget);
            total.Should().Be(expectedAnswer);
        }
    }
}
