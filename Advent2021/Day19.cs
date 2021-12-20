using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Utils;
using Xunit;


namespace Advent2021
{
    public class Day19
    {
        class OverlapData
        {
            public Scanner scanner;
            public int rotation;
            public (int x, int y, int z) offset;
        }

        class Scanner
        {
            public int id;
            public (int x, int y, int z)[][] clouds;
            public List<OverlapData> overlaps = new();
            public bool added = false;
        }

        static (int x, int y, int z) Point(int x, int y, int z) => (x, y, z);
        static (int x, int y, int z) Reverse((int x, int y, int z) p) => (-p.x, -p.y, -p.z);
        static (int x, int y, int z) RotateAroundZ((int x, int y, int z) point) => (-point.y, point.x, point.z);
        static (int x, int y, int z) RotateAroundY((int x, int y, int z) point) => (-point.z, point.y, point.x);
        static (int x, int y, int z) RotateAroundX((int x, int y, int z) point) => (point.x, point.z, -point.y);

        static void RotateAroundZ((int x, int y, int z)[] cloud, (int x, int y, int z)[] result)
        {
            for (var i = 0; i < cloud.Length; i++)
                result[i] = RotateAroundZ(cloud[i]);
        }

        static void RotateAroundY((int x, int y, int z)[] cloud, (int x, int y, int z)[] result)
        {
            for (var i = 0; i < cloud.Length; i++)
                result[i] = RotateAroundY(cloud[i]);
        }

        static void RotateAroundX((int x, int y, int z)[] cloud, (int x, int y, int z)[] result)
        {
            for (var i = 0; i < cloud.Length; i++)
                result[i] = RotateAroundX(cloud[i]);
        }

        static IEnumerable<((int x, int y, int z)[] cloud, int rotationIndex)> RotationsFast((int x, int y, int z)[] cloud)
        {
            var result = new (int x, int y, int z)[cloud.Length];
            
            yield return (cloud, 0);
            RotateAroundY(cloud, result);
            yield return (result, 1);
            RotateAroundY(result, result);
            yield return (result, 2);
            RotateAroundY(result, result);
            yield return (result, 3);

            RotateAroundZ(cloud, result);
            yield return (result, 4);
            RotateAroundY(result, result);
            yield return (result, 5);
            RotateAroundY(result, result);
            yield return (result, 6);
            RotateAroundY(result, result);
            yield return (result, 7);

            RotateAroundY(result, result);
            RotateAroundZ(result, result);
            yield return (result, 8);
            RotateAroundY(result, result);
            yield return (result, 9);
            RotateAroundY(result, result);
            yield return (result, 10);
            RotateAroundY(result, result);
            yield return (result, 11);

            RotateAroundY(result, result);
            RotateAroundZ(result, result);
            yield return (result, 12);
            RotateAroundY(result, result);
            yield return (result, 13);
            RotateAroundY(result, result);
            yield return (result, 14);
            RotateAroundY(result, result);
            yield return (result, 15);

            RotateAroundX(cloud, result);
            yield return (result, 16);
            RotateAroundY(result, result);
            yield return (result, 17);
            RotateAroundY(result, result);
            yield return (result, 18);
            RotateAroundY(result, result);
            yield return (result, 19);

            RotateAroundX(cloud, result);
            RotateAroundX(result, result);
            RotateAroundX(result, result);
            yield return (result, 20);
            RotateAroundY(result, result);
            yield return (result, 21);
            RotateAroundY(result, result);
            yield return (result, 22);
            RotateAroundY(result, result);
            yield return (result, 23);
        }

        static IEnumerable<((int x, int y, int z)[] cloud, int rotationIndex)> Rotations((int x, int y, int z)[] cloud)
        {
            return RotationsFast(cloud).Select(r => (((int x, int y, int z)[])r.cloud.Clone(), r.rotationIndex));
        }

        static (int x, int y, int z) CalcTranslation((int x, int y, int z) a, (int x, int y, int z) b) => (a.x-b.x, a.y-b.y, a.z-b.z);

        static (int x, int y, int z)[] Cloud(params (int x, int y, int z)[] points) => points;

        static HashSet<(int x, int y, int z)> CloudToSet((int x, int y, int z)[] cloud, (int x, int y, int z) offset)
        {
            HashSet<(int x, int y, int z)> set = new();

            for (var i = 0; i < cloud.Length; i++)
                set.Add((cloud[i].x + offset.x, cloud[i].y + offset.y, cloud[i].z + offset.z));

            return set;
        }

        static bool CloudEquals((int x, int y, int z)[] cloud, params (int x, int y, int z)[] points)
        {
            if (cloud.Length != points.Length) return false;

            for (int i = 0; i < points.Length; i++)
                if (cloud[i] != points[i])
                    return false;

            return true;
        }

        static bool IsInRange((int x, int y, int z) p1, (int x, int y, int z) p2, int range)
        {
            return Math.Abs(p1.x - p2.x) <= range
                && Math.Abs(p1.y - p2.y) <= range
                && Math.Abs(p1.z - p2.z) <= range;
        }

        static int SuccessfulOverlaps((int x, int y, int z)[] sourceCloud, (int x, int y, int z)[] targetCloud, (int x, int y, int z) offset, int scannerRange)
        {
            var cloud2set = CloudToSet(targetCloud, offset);
            int matchCount = 0;

            for (int k = 0; k < sourceCloud.Length; k++)
            {
                // Don't check points that aren't in range of the scanner from the target cloud
                if (!IsInRange(sourceCloud[k], offset, scannerRange)) continue;
                if (!cloud2set.Contains(sourceCloud[k])) return -1;
                matchCount++;
            }

            return matchCount;
        }

        static bool ResolveOverlap((int x, int y, int z)[] cloud1, (int x, int y, int z)[] cloud2, int scannerRange, int requiredOverlap, out (int x, int y, int z) offset)
        {
            for (int i = 0; i < cloud1.Length; i++)
            {
                for (int j = 0; j < cloud2.Length; j++)
                {
                    var translation = CalcTranslation(cloud1[i], cloud2[j]);

                    int matchCount1 = SuccessfulOverlaps(cloud1, cloud2, translation, scannerRange);
                    if (matchCount1 < requiredOverlap) continue;
                    if (matchCount1 > requiredOverlap) Debug.WriteLine($"Found overlaps with {matchCount1} points!");

                    int matchCount2 = SuccessfulOverlaps(cloud2, cloud1, Reverse(translation), scannerRange);
                    if (matchCount2 != matchCount1)
                    {
                        Debug.WriteLine("Found non-symertic overlap!");
                        continue;
                    }

                    offset = translation;
                    return true;
                }
            }
            offset = (0, 0, 0);
            return false;
        }

        [Theory]
        [InlineData(0, 0, 0, 3, -3, 0, 3, true)]
        [InlineData(0, 0, 0, 5, 0, 0, 3, false)]
        [InlineData(500, 0, -500, -500, 1000, -1500, 1000, true)]
        [InlineData(500, 0, -500, 1501, 0, -500, 1000, false)]
        public void IsInRange_Test(int x1, int y1, int z1, int x2, int y2, int z2, int range, bool expectedAnswer)
        {
            IsInRange((x1, y1, z1), (x2, y2, z2), range).Should().Be(expectedAnswer);
        }

        [Fact]
        public void ResolveOverlap_Test()
        {
            var cloud1 = Cloud(
                Point(-2, -2, 0),
                Point(2, 0, 0),
                Point(3, -3, 0)
            );
            var cloud2 = Cloud(
                Point(-2, 1, 0),
                Point(-1, -2, 0),
                Point(1, 1, 0)
            );

            ResolveOverlap(cloud1, cloud2, 3, 2, out var offset).Should().BeTrue();
            offset.Should().Be((4, -1, 0));
        }

        [Fact]
        public void ResolveOverlap_Example1()
        {
            var cloud1 = Cloud(
                Point(0, 2, 0),
                Point(4, 1, 0),
                Point(3, 3, 0)
            );
            var cloud2 = Cloud(
                Point(-1, -1, 0),
                Point(-5, 0, 0),
                Point(-2, 1, 0)
            );

            ResolveOverlap(cloud1, cloud2, 1000, 3, out var offset).Should().BeTrue();
            offset.Should().Be((5, 2, 0));

            var resolvedCloud = CloudToSet(cloud2, offset);
            resolvedCloud.Contains(cloud1[0]).Should().BeTrue();
            resolvedCloud.Contains(cloud1[1]).Should().BeTrue();
            resolvedCloud.Contains(cloud1[2]).Should().BeTrue();
        }

        [Fact]
        public void Rotations_Test()
        {
            var cloud = Cloud(
                Point(-1, -1, 1),
                Point(1, -1, 1)
            );

            var rotations = Rotations(cloud).ToArray();
            rotations.Length.Should().Be(24);

            rotations[0].rotationIndex.Should().Be(0);
            CloudEquals(rotations[0].cloud, Point(-1, -1, 1), Point(1, -1, 1)).Should().BeTrue();
            rotations[1].rotationIndex.Should().Be(1);
            CloudEquals(rotations[1].cloud, Point(-1, -1, -1), Point(-1, -1, 1)).Should().BeTrue();
            rotations[2].rotationIndex.Should().Be(2);
            CloudEquals(rotations[2].cloud, Point(1, -1, -1), Point(-1, -1, -1)).Should().BeTrue();
            rotations[3].rotationIndex.Should().Be(3);
            CloudEquals(rotations[3].cloud, Point(1, -1, 1), Point(1, -1, -1)).Should().BeTrue();

            rotations[4].rotationIndex.Should().Be(4);
            CloudEquals(rotations[4].cloud, Point(1, -1, 1), Point(1, 1, 1)).Should().BeTrue();
            rotations[5].rotationIndex.Should().Be(5);
            CloudEquals(rotations[5].cloud, Point(-1, -1, 1), Point(-1, 1, 1)).Should().BeTrue();
            rotations[6].rotationIndex.Should().Be(6);
            CloudEquals(rotations[6].cloud, Point(-1, -1, -1), Point(-1, 1, -1)).Should().BeTrue();
            rotations[7].rotationIndex.Should().Be(7);
            CloudEquals(rotations[7].cloud, Point(1, -1, -1), Point(1, 1, -1)).Should().BeTrue();

            rotations[8].rotationIndex.Should().Be(8);
            CloudEquals(rotations[8].cloud, Point(1, 1, 1), Point(-1, 1, 1)).Should().BeTrue();
            rotations[9].rotationIndex.Should().Be(9);
            CloudEquals(rotations[9].cloud, Point(-1, 1, 1), Point(-1, 1, -1)).Should().BeTrue();
            rotations[10].rotationIndex.Should().Be(10);
            CloudEquals(rotations[10].cloud, Point(-1, 1, -1), Point(1, 1, -1)).Should().BeTrue();
            rotations[11].rotationIndex.Should().Be(11);
            CloudEquals(rotations[11].cloud, Point(1, 1, -1), Point(1, 1, 1)).Should().BeTrue();

            rotations[12].rotationIndex.Should().Be(12);
            CloudEquals(rotations[12].cloud, Point(-1, 1, 1), Point(-1, -1, 1)).Should().BeTrue();
            rotations[13].rotationIndex.Should().Be(13);
            CloudEquals(rotations[13].cloud, Point(-1, 1, -1), Point(-1, -1, -1)).Should().BeTrue();
            rotations[14].rotationIndex.Should().Be(14);
            CloudEquals(rotations[14].cloud, Point(1, 1, -1), Point(1, -1, -1)).Should().BeTrue();
            rotations[15].rotationIndex.Should().Be(15);
            CloudEquals(rotations[15].cloud, Point(1, 1, 1), Point(1, -1, 1)).Should().BeTrue();

            rotations[16].rotationIndex.Should().Be(16);
            CloudEquals(rotations[16].cloud, Point(-1, 1, 1), Point(1, 1, 1)).Should().BeTrue();
            rotations[17].rotationIndex.Should().Be(17);
            CloudEquals(rotations[17].cloud, Point(-1, 1, -1), Point(-1, 1, 1)).Should().BeTrue();
            rotations[18].rotationIndex.Should().Be(18);
            CloudEquals(rotations[18].cloud, Point(1, 1, -1), Point(-1, 1, -1)).Should().BeTrue();
            rotations[19].rotationIndex.Should().Be(19);
            CloudEquals(rotations[19].cloud, Point(1, 1, 1), Point(1, 1, -1)).Should().BeTrue();

            rotations[20].rotationIndex.Should().Be(20);
            CloudEquals(rotations[20].cloud, Point(-1, -1, -1), Point(1, -1, -1)).Should().BeTrue();
            rotations[21].rotationIndex.Should().Be(21);
            CloudEquals(rotations[21].cloud, Point(1, -1, -1), Point(1, -1, 1)).Should().BeTrue();
            rotations[22].rotationIndex.Should().Be(22);
            CloudEquals(rotations[22].cloud, Point(1, -1, 1), Point(-1, -1, 1)).Should().BeTrue();
            rotations[23].rotationIndex.Should().Be(23);
            CloudEquals(rotations[23].cloud, Point(-1, -1, 1), Point(-1, -1, -1)).Should().BeTrue();
        }

        static Scanner LoadScanner(Func<string> reader)
        {
            string line = reader();
            if (line == null) return null;

            var id = int.Parse(line.Groups(@"(\d+)")[0]);
            List<(int x, int y, int z)> points = new();

            while (true)
            {
                line = reader();
                if (string.IsNullOrEmpty(line)) break;

                var groups = line.Groups(@"([-\d]+),([-\d]+),([-\d]+)");
                points.Add((
                    int.Parse(groups[0]),
                    int.Parse(groups[1]),
                    int.Parse(groups[2])
                ));
            }

            return new Scanner
            {
                id = id,
                clouds = Rotations(points.ToArray()).Select(r => r.cloud).ToArray()
            };
        }

        static void AddToCloud(HashSet<(int x, int y, int z)> cloud, (int x, int y, int z)[] other, (int x, int y, int z) offset)
        {
            for (var i = 0; i < other.Length; i++)
            {
                (var x, var y, var z) = other[i];
                cloud.Add((x + offset.x, y + offset.y, z + offset.z));
            }
        }

        static void GetOverlapRotation((int x, int y, int z)[]cloud, (int x, int y, int z)[][]candidates, out int rotation, out (int x, int y, int z) offset)
        {
            for (var i = 0; i < candidates.Length; i++)
            {
                if (ResolveOverlap(cloud, candidates[i], 1000, 12, out offset))
                {
                    rotation = i;
                    return;
                }
            }
            throw new InvalidOperationException("No oeverlap found");
        }

        static void ProcessScanner(HashSet<(int x, int y, int z)> cloud, Scanner scanner, int scannerRotation, (int x, int y, int z) offset)
        {
            scanner.added = true;
            translatedScanners.Add(offset);

            Debug.WriteLine($"Processing scanner {scanner.id}...");
            foreach (var data in scanner.overlaps)
            {
                if (data.scanner.added) continue;
                GetOverlapRotation(scanner.clouds[scannerRotation], data.scanner.clouds, out var childRotation, out var childOffset);
                childOffset = (childOffset.x + offset.x, childOffset.y + offset.y, childOffset.z + offset.z);
                AddToCloud(cloud, data.scanner.clouds[childRotation], childOffset);
                ProcessScanner(cloud, data.scanner, childRotation, childOffset);
            }
        }

        static Scanner[] LoadScanners(string filename)
        {
            var reader = FileIterator.CreateLineReader(filename);
            List<Scanner> scanners = new();
            while (true)
            {
                var scanner = LoadScanner(reader);
                if (scanner == null) break;
                scanners.Add(scanner);
            }
            return scanners.ToArray();
        }

        static int Distance((int x, int y, int z) a, (int x, int y, int z) b) => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z);

        static readonly List<(int x, int y, int z)> translatedScanners = new();

        [Theory]
        [InlineData("Data/Day19_Test.txt", 79, 3621)]
        [InlineData("Data/Day19.txt", 353, 10832)]
        public void Part1(string filename, int expectedAnswer1, int expectedAnswer2)
        {
            translatedScanners.Clear();
            var scanners = LoadScanners(filename);

            foreach (var scanner in scanners)
            {
                foreach (var other in scanners)
                {
                    if (other == scanner) continue;
                    for (var i = 0; i < other.clouds.Length; i++)
                    {
                        if (ResolveOverlap(scanner.clouds[0], other.clouds[i], 1000, 12, out var offset))
                        {
                            Debug.WriteLine($"Scaner {scanner.id} overlaps with {other.id} via rotation {i}");
                            scanner.overlaps.Add(new() {
                                rotation = i,
                                scanner = other,
                                offset = offset
                            });
                            break;
                        }
                    }
                }
            }

            foreach (var scanner in scanners)
            {
                Debug.WriteLine($"Scanner {scanner.id}");
                foreach (var data in scanner.overlaps)
                {
                    Debug.WriteLine($"   {data.scanner.id}");
                }
            }

            HashSet<(int x, int y, int z)> cloud = new(scanners[0].clouds[0]);
            ProcessScanner(cloud, scanners[0], 0, (0, 0, 0));

            Debug.WriteLine("Done");

            cloud.Count.Should().Be(expectedAnswer1);

            var max = int.MinValue;
            foreach (var a in translatedScanners)
            {
                foreach (var b in translatedScanners)
                {
                    var d = Distance(a, b);
                    if (max < d) max = d; 
                }
            }

            max.Should().Be(expectedAnswer2);
        }
    }
}
