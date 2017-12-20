using Adevent2017.Utils;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Adevent2017
{
    public class Problem2001
    {
        struct Vector3
        {
            public long x;
            public long y;
            public long z;

            public void Add(Vector3 other)
            {
                x += other.x;
                y += other.y;
                z += other.z;
            }

            public double DistanceFrom0Sqrd() => (x * x) + (y * y) + (z * z); 
        }

        class Particle
        {
            public int Id;
            public Vector3 Pos;
            public Vector3 Vel;
            public Vector3 Acc;

            public void Update()
            {
                Vel.Add(Acc);
                Pos.Add(Vel);
            }

            public string PositionId => $"{Pos.x},{Pos.y},{Pos.z}";
        }

        Vector3 ParseVector3(string info)
        {
            var values = info.Split(new char[] { '<', ',', '>', 'p', 'a', 'v', '=' }, StringSplitOptions.RemoveEmptyEntries);
            return new Vector3
            {
                x = long.Parse(values[0]),
                y = long.Parse(values[1]),
                z = long.Parse(values[2])
            };
        }

        Particle ParseParticle(int id, string info)
        {
            var vectors = info.Replace(", ", "|").Split('|');
            var p = ParseVector3(vectors[0]);
            var v = ParseVector3(vectors[1]);
            var a = ParseVector3(vectors[2]);
            return new Particle()
            {
                Id = id,
                Pos = p,
                Vel = v,
                Acc = a
            };
        }

        int Solve1(string datafile)
        {
            var particles = new List<Particle>();
            int id = 0;
            FileIterator.ForEachLine<string>(datafile, line =>
            {
                particles.Add(ParseParticle(id++, line));
            });
      
            var lowestAcc = particles[0].Acc.DistanceFrom0Sqrd();
            var lowestParticle = particles[0];
            for (var i = 1; i < particles.Count; i++)
            {
                var part = particles[i];
                if (part.Acc.DistanceFrom0Sqrd() < lowestAcc)
                {
                    lowestAcc = part.Acc.DistanceFrom0Sqrd();
                    lowestParticle = part;
                }
            }
            
            return lowestParticle.Id;
        }

        Dictionary<string, List<int>> CreateCollisionCache() => new Dictionary<string, List<int>>();

        static List<int> CreateParticleList() => new List<int>();

        int Solve2(string datafile)
        {
            var particles = new Dictionary<int, Particle>();
            int id = 0;
            FileIterator.ForEachLine<string>(datafile, line =>
            {
                particles[id] = ParseParticle(id, line);
                id++;
            });

            for (var i = 0; i < 39; i++)
            {
                var ccache = CreateCollisionCache();
                foreach (var part in particles.Values)
                {
                    part.Update();
                    ccache.GetOrCreate(part.PositionId, CreateParticleList).Add(part.Id);
                }

                foreach (var collisions in ccache.Values)
                    if (collisions.Count >= 2)
                        foreach (var partId in collisions)
                            particles.Remove(partId);
            }

            return particles.Count;
        }

        [Theory]
        [InlineData("Data/2001-example.txt", 0)]
        [InlineData("Data/2001.txt", 170)]
        void Part1(string datafile, int answer) => Solve1(datafile).Should().Be(answer);

        [Theory]
        [InlineData("Data/2001-example.txt", 2)]
        [InlineData("Data/2001.txt", 571)]
        void Part2(string datafile, int answer) => Solve2(datafile).Should().Be(answer);
    }
}
