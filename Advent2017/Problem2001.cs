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

            public long DistanceFrom0() => Math.Abs(x) + Math.Abs(y) + Math.Abs(z);
            public long DistanceFrom(Vector3 point) => Math.Abs(x - point.x) + Math.Abs(y - point.y) + Math.Abs(z - point.z);
            public double DistanceFrom0Sqrd() => (x * x) + (y * y) + (z * z); 
        }

        class Particle
        {
            public Vector3 Pos;
            public Vector3 Vel;
            public Vector3 Acc;

            public void Update()
            {
                Vel.Add(Acc);
                Pos.Add(Vel);
            }
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

        Particle ParseParticle(string info)
        {
            var vectors = info.Replace(", ", "|").Split('|');
            var p = ParseVector3(vectors[0]);
            var v = ParseVector3(vectors[1]);
            var a = ParseVector3(vectors[2]);
            return new Particle()
            {
                Pos = p,
                Vel = v,
                Acc = a
            };
        }

        int Solve1(string datafile)
        {
            var particles = new List<Particle>();
            FileIterator.ForEachLine<string>(datafile, line =>
            {
                particles.Add(ParseParticle(line));
            });
      

            var lowestAcc = particles[0].Acc.DistanceFrom0Sqrd();
            var lowestParticle = 0;
            for (var i = 1; i < particles.Count; i++)
            {
                var part = particles[i];
                if (part.Acc.DistanceFrom0Sqrd() < lowestAcc)
                {
                    lowestAcc = part.Acc.DistanceFrom0Sqrd();
                    lowestParticle = i;
                }
            }
            
            return lowestParticle;
        }

        [Theory]
        [InlineData("Data/2001-example.txt", 0)]
        [InlineData("Data/2001.txt", 170)]
        void Part1(string datafile, int answer) => Solve1(datafile).Should().Be(answer);
    }
}
