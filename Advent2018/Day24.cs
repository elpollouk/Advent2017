using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2018
{
    public class Day24
    {
        enum UnitType
        {
            Immune,
            Infection
        }

        class Unit
        {
            public readonly UnitType Type;
            public int NumUnits;
            public readonly int HitPointsPerUnit;
            public readonly int Damage;
            public readonly string DamageType;
            public readonly int Initiative;
            public readonly ICollection<string> WeakTo;
            public readonly ICollection<string> ImmuneTo;
            public int Power => NumUnits * Damage;
            public bool IsAlive => NumUnits > 0;
            public Unit Target = null;

            public Unit(string description, UnitType type, int boost)
            {
                Type = type;
                var matches = description.Match(@"(\d+) units each with (\d+) hit points (.*) ?with an attack that does (\d+) (.+) damage at initiative (\d+)");
                NumUnits = int.Parse(matches.Groups[1].Value);
                HitPointsPerUnit = int.Parse(matches.Groups[2].Value);
                Damage = int.Parse(matches.Groups[4].Value) + (type == UnitType.Immune ? boost : 0);
                DamageType = matches.Groups[5].Value;
                Initiative = int.Parse(matches.Groups[6].Value);

                WeakTo = new HashSet<string>();
                ImmuneTo = new HashSet<string>();
                var weaknesses = matches.Groups[3].Value;
                if (weaknesses != "")
                {
                    weaknesses = weaknesses.Substring(1, weaknesses.Length - 3);
                    var weaknessLists = weaknesses.Split(';');
                    foreach (var list in weaknessLists)
                    {
                        ICollection<string> target;
                        if (list.Trim().StartsWith("weak to "))
                            target = WeakTo;
                        else
                            target = ImmuneTo;

                        var split = list.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        for (var i = 2; i < split.Length; i++)
                            target.Add(split[i].Trim());
                    }
                }
            }

            public int CalculateDamageFrom(Unit attacker)
            {
                if (ImmuneTo.Contains(attacker.DamageType))
                    return 0;

                var power = attacker.Power;
                if (WeakTo.Contains(attacker.DamageType))
                    power *= 2;

                return power;
            }

            public void AttackTarget()
            {
                if (Target == null) return;

                var damage = Target.CalculateDamageFrom(this);
                var unitsKilled = damage / Target.HitPointsPerUnit;
                Target.NumUnits -= unitsKilled;
            }
        }

        bool HasWinner(IEnumerable<Unit> units)
        {
            units = units.Where(u => u.IsAlive);
            var winningTeam = units.First().Type;

            foreach (var unit in units)
                if (unit.Type != winningTeam)
                    return false;

            return true;
        }

        [Theory]
        [InlineData(5216, "Data/Day24-Text.txt", 0)]
        [InlineData(51, "Data/Day24-Text.txt", 1570)] // Part 1 solution
        [InlineData(16086, "Data/Day24.txt", 0)]
        [InlineData(3957, "Data/Day24.txt", 34)] // Part 2 solution
        void Problem(int expectedAnswer, string inputFile, int boost)
        {
            var allUnits = new List<Unit>();
            var currentType = UnitType.Immune;

            foreach (var line in FileIterator.Lines(inputFile))
            {
                if (line == "Immune System:")
                    currentType = UnitType.Immune;
                else if (line == "Infection:")
                    currentType = UnitType.Infection;
                else if (line != "")
                    allUnits.Add(new Unit(line, currentType, boost));
            }

            while (!HasWinner(allUnits))
            {
                var targetted = new HashSet<Unit>();
                foreach (var unit in allUnits.Where(u => u.IsAlive).OrderByDescending(u => u.Power).ThenByDescending(u => u.Initiative))
                {
                    unit.Target = allUnits.Where(u => u != unit && u.IsAlive && !targetted.Contains(u) && u.Type != unit.Type && u.CalculateDamageFrom(unit) != 0)
                                          .OrderByDescending(u => u.CalculateDamageFrom(unit))
                                          .ThenByDescending(u => u.Power)
                                          .ThenByDescending(u => u.Initiative)
                                          .FirstOrDefault();

                    if (unit.Target != null)
                        targetted.Add(unit.Target);
                }

                foreach (var unit in allUnits.OrderByDescending(u => u.Initiative))
                {
                    if (!unit.IsAlive) continue;
                    unit.AttackTarget();
                }
            }

            var sum = allUnits.Where(u => u.IsAlive).Select(u => u.NumUnits).Sum();
            sum.Should().Be(expectedAnswer);
        }
    }
}
