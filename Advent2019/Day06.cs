using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Utils.DataStructures;
using Xunit;

namespace Advent2019
{
    public class Day06
    {
        class Orbit
        {
            public readonly string Name;
            public int Depth = 0;

            public Orbit(string name)
            {
                Name = name;
            }

            public override string ToString() => $"\"{Name}\"={Depth}";
        }

        (Graph<Orbit>, Dictionary<string, Orbit>) BuildOrbits(Dictionary<string, string> orbitsDef)
        {
            var orbitMap = new Dictionary<string, Orbit>();
            var orbits = new Graph<Orbit>();
            var com = new Orbit("COM");
            orbitMap["COM"] = com;
            orbits.AddNode(com);
            foreach (var orbit in orbitsDef.Keys)
            {
                orbitMap[orbit] = new Orbit(orbit);
                orbits.AddNode(orbitMap[orbit]);
            }

            foreach (var orbit in orbitsDef)
                orbits.AddParentChildLink(orbitMap[orbit.Value], orbitMap[orbit.Key]);

            orbits.Root = com;

            return (orbits, orbitMap);
        }

        int CalculateNodeDepth(Graph<Orbit> orbits)
        {
            orbits.DepthFirstWalk(orbit =>
            {
                if (orbit.Name != "COM")
                    orbit.Depth = orbits.GetParent(orbit).Depth + 1;
            });

            var total = 0;
            orbits.DepthFirstWalk(orbit => total += orbit.Depth);
            return total;
        }

        int PathLength(Graph<Orbit> orbits, Dictionary<string, Orbit> orbitMap)
        {
            var youChain = new LinkedList<Orbit>();
            var sanChain = new LinkedList<Orbit>();

            var orbit = orbitMap["YOU"];
            while (orbit.Name != "COM")
            {
                orbit = orbits.GetParent(orbit);
                youChain.AddFirst(orbit);
            }

            orbit = orbitMap["SAN"];
            while (orbit.Name != "COM")
            {
                orbit = orbits.GetParent(orbit);
                sanChain.AddFirst(orbit);
            }

            while (youChain.First.Value == sanChain.First.Value)
            {
                youChain.RemoveFirst();
                sanChain.RemoveFirst();
            }

            return youChain.Count + sanChain.Count;
        }

        [Theory]
        [InlineData("Data/Day06-example.txt", 54, 4)] // Adjusted to include YOU/SAN example
        [InlineData("Data/Day06.txt", 194721, 316)]
        public void Problem(string input, int answer1, int answer2)
        {
            var orbitsDef = new Dictionary<string, string>();
            FileIterator.ForEachLine<string>(input, line =>
            {
                var pair = line.Split(')');
                orbitsDef[pair[1]] = pair[0];
            });

            var (orbits, orbitMap) = BuildOrbits(orbitsDef);

            CalculateNodeDepth(orbits).Should().Be(answer1);
            PathLength(orbits, orbitMap).Should().Be(answer2);
        }
    }
}
