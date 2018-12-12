using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Xunit;

namespace Advent2018
{
    public class Day12
    {
        class Pot
        {
            public readonly int Value;
            public bool Active;

            public Pot(int value, bool active)
            {
                Value = value;
                Active = active;
            }

            public override string ToString() => Active ? "#" : ".";
        }

        void PadState(LinkedList<Pot> state)
        {
            var pot = state.First;
            var padValue = pot.Value.Value;
            while (padValue > -6) // I'm only padding to -6 so that things line up when debug printing
                state.AddFirst(new Pot(--padValue, false));

            var padCount = 4;
            pot = state.Last;
            padValue = pot.Value.Value;
            while (pot.Value.Active == false && padCount != 0)
            {
                padCount--;
                pot = pot.Previous;
            }

            for (var i = 0; i < padCount; i++)
                state.AddLast(new Pot(++padValue, false));
        }

        LinkedList<Pot> CreateInitialState(string initialState)
        {
            var state = new LinkedList<Pot>();

            var potValue = 0;
            foreach (var c in initialState)
            {
                var pot = new Pot(potValue++, c == '#');
                state.AddLast(pot);
            }

            PadState(state);

            return state;
        }

        string PotStateToString(LinkedListNode<Pot> node)
        {
            var result = "";
            node = node.Previous.Previous;
            for (var i = 0; i < 5; i++)
            {
                result += node.Value.Active ? "#" : ".";
                node = node.Next;
            }
            return result;
        }

        LinkedList<Pot> NextState(HashSet<string> rules, LinkedList<Pot> state)
        {
            var newState = new LinkedList<Pot>();
            var node = state.First.Next.Next;

            while (node.Next.Next != null)
            {
                var rule = PotStateToString(node);
                newState.AddLast(new Pot(node.Value.Value, rules.Contains(rule)));
                node = node.Next;
            }

            PadState(newState);

            return newState;
        }

        int SumPots(LinkedList<Pot> states)
        {
            var sum = 0;
            var pot = states.First;

            while (pot != null)
            {
                if (pot.Value.Active)
                    sum += pot.Value.Value;
                pot = pot.Next;
            }

            return sum;
        }

        void PrintPots(LinkedList<Pot> states)
        {
            var pot = states.First;
            Debug.Write($"{pot.Value.Value}: ");
            while (pot != null)
            {
                Debug.Write(pot.Value.Active ? "#" : ".");
                pot = pot.Next;
            }
            Debug.WriteLine("");
        }

        HashSet<string> LoadRules(string inputFile)
        {
            var rules = new HashSet<string>();

            foreach (var line in FileIterator.Lines(inputFile))
            {
                var parsed = line.Match(@"(.+) => (.)");
                if (parsed.Groups[2].Value == "#")
                    rules.Add(parsed.Groups[1].Value);
            }

            return rules;
        }

        [Theory]
        [InlineData("Data/Day12-Test.txt", "#..#.#..##......###...###", 325)]
        [InlineData("Data/Day12.txt", "##..##....#.#.####........##.#.#####.##..#.#..#.#...##.#####.###.##...#....##....#..###.#...#.#.#.#", 3248)]
        void Test(string inputfile, string initialState, int expectedAnswer)
        {
            var rules = LoadRules(inputfile);
            var state = CreateInitialState(initialState);
            PrintPots(state);
            for (var i = 0; i < 20; i++)
            {
                state = NextState(rules, state);
                PrintPots(state);
            }

            SumPots(state).Should().Be(expectedAnswer);
        }
    }
}
