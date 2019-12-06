using FluentAssertions;
using System.Collections.Generic;
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

        IndexedGraph<string, Orbit> BuildOrbits(Dictionary<string, string> orbitsDef)
        {
            var orbits = new IndexedGraph<string, Orbit>();
            orbits["COM"] = new Orbit("COM");
            foreach (var orbit in orbitsDef.Keys)
                orbits[orbit] = new Orbit(orbit);

            foreach (var orbit in orbitsDef)
                orbits.AddParentChildLink(orbit.Value, orbit.Key);

            orbits.Root = orbits["COM"];

            return orbits;
        }

        int CalculateNodeDepth(IndexedGraph<string, Orbit> orbits)
        {
            foreach (var orbit in orbits.DepthFirstWalk())
                if (orbit.Name != "COM")
                    orbit.Depth = orbits.GetParent(orbit).Depth + 1;

            var total = 0;
            foreach (var orbit in orbits.DepthFirstWalk())
                total += orbit.Depth;

            return total;
        }

        int PathLength(IndexedGraph<string, Orbit> orbits)
        {
            var youChain = new LinkedList<Orbit>();
            var sanChain = new LinkedList<Orbit>();

            var orbit = orbits["YOU"];
            while (orbit.Name != "COM")
            {
                orbit = orbits.GetParent(orbit);
                youChain.AddFirst(orbit);
            }

            orbit = orbits["SAN"];
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

            var orbits = BuildOrbits(orbitsDef);

            CalculateNodeDepth(orbits).Should().Be(answer1);
            PathLength(orbits).Should().Be(answer2);
        }
    }
}
