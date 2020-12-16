using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

using Fields = System.Collections.Generic.Dictionary<string, System.Collections.Generic.HashSet<int>>;

namespace Advent2020
{
    public class Day16
    {
        static IEnumerable<int> Range(int from, int to)
        {
            for (var i = from; i <= to; i++)
                yield return i;
        }

        static Fields LoadFields(string input)
        {
            Fields fields = new();

            foreach (var line in FileIterator.Lines(input))
            {
                if (line == "") break;
                var groups = line.Groups("^(.+): (.+)$");
                var set = fields.GetOrCreate(groups[0], () => new());
                groups = groups[1].Split(" or ");
                foreach (var range in groups)
                {
                    var fromTo = range.Split('-');
                    foreach (var i in Range(int.Parse(fromTo[0]), int.Parse(fromTo[1])))
                        set.Add(i);
                }
            }

            return fields;
        }

        static IEnumerable<int[]> LoadTickets(string input, bool yours)
        {
            var find = yours ? "your ticket:" : "nearby tickets:";
            var found = false;
            foreach (var line in FileIterator.Lines(input))
            {
                if (!found)
                {
                    if (line == find) found = true;
                    continue;
                }
                else if (line == "") break;

                yield return line.SplitAndConvert<int>(',');
            }
        }

        static HashSet<int> AllValidValues(Fields fields)
        {
            HashSet<int> allValidValues = new();
            foreach (var field in fields.Values)
                foreach (var value in field)
                    allValidValues.Add(value);

            return allValidValues;
        }

        static bool IsValidTicket(int[] ticket, HashSet<int> allValidFields)
        {
            foreach (var i in ticket)
                if (!allValidFields.Contains(i))
                    return false;

            return true;
        }

        static IEnumerable<int> GetInvalidFields(string input, Fields fields)
        {
            var allValidValues = AllValidValues(fields);

            foreach (var ticket in LoadTickets(input, false))
            {
                foreach (var i in ticket)
                    if (!allValidValues.Contains(i))
                        yield return i;
            }
        }

        static IEnumerable<string> Reduce(List<HashSet<string>> foundFields)
        {
            HashSet<string> solvedFields = new();

            // Keep looping over the the fields until we reach a point where we only have
            // a single candidate per field
            bool needsToReduce = true;
            while (needsToReduce)
            {
                needsToReduce = false;
                foreach (var fields in foundFields)
                {
                    if (fields.Count == 1)
                    {
                        // This is a solved field as we only have a single candidate, no further reduction of this
                        // field is possible
                        solvedFields.Add(fields.First());
                    }
                    else
                    {
                        // There are multiple candiates for this field, so reduce it by removing all the fields that have
                        // be solved so far. We will need another reduction iteration in order to flag this field as solved.
                        needsToReduce = true;
                        fields.ExceptWith(solvedFields);
                    }
                }
            }

            return foundFields.Select(f => f.First());
        }

        static IEnumerable<string> GetFieldOrder(string input, Fields fields)
        {
            // First filter out all the tickets that are invalid due to having fields that fail to
            // satisfy any constraints
            var allValidFields = AllValidValues(fields);
            var tickets = LoadTickets(input, false)
                .Where(t => IsValidTicket(t, allValidFields))
                .ToArray();

            // Then gather all the candidates for all the fields
            // Start by assuming all fields are valid and then removing them from the valid fields set
            // as we discover ticket field values that fail to satisfy that field's contraint
            List<HashSet<string>> foundFields = new();
            for (var field = 0; field < tickets[0].Length; field++)
            {
                HashSet<string> validFields = new(fields.Keys);

                for (var ticket = 0; ticket < tickets.Length; ticket++)
                    foreach (var fieldName in validFields)
                        if (!fields[fieldName].Contains(tickets[ticket][field]))
                            validFields.Remove(fieldName);

                foundFields.Add(validFields); 
            }

            // At this point, we have a set per field of candidate field names, we need to reduce this down to just
            // one candidate per field
            return Reduce(foundFields);
        }

        [Theory]
        [InlineData("Data/Day16_test.txt", 71)]
        [InlineData("Data/Day16.txt", 23044)]
        public void Problem1(string input, int expected)
        {
            var fields = LoadFields(input);
            GetInvalidFields(input, fields).Sum().Should().Be(expected);
        }

        [Theory]
        [InlineData("Data/Day16_test2.txt")]
        public void Problem2_Test(string input)
        {
            var fields = LoadFields(input);
            var fieldOrder = GetFieldOrder(input, fields);
            fieldOrder.Should().BeEquivalentTo(
                "class",
                "row",
                "seat"
            );
        }

        [Theory]
        [InlineData("Data/Day16.txt", 3765150732757)]
        public void Problem2(string input, long expected)
        {
            var fields = LoadFields(input);
            var fieldOrder = GetFieldOrder(input, fields).ToArray();
            var myTicket = LoadTickets(input, true).First();

            var product = 1L;
            for (var i = 0; i < myTicket.Length; i++)
                if (fieldOrder[i].StartsWith("departure"))
                    product *= myTicket[i];

            product.Should().Be(expected);
        }
    }
}
