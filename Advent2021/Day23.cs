using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Utils;
using Utils.Alogrithms;
using Xunit;

namespace Advent2021
{
    public class Day23
    {
        /*
            #############
            #01 2 3 4 56#
            ###7#9#B#D###
              #8#A#C#E#
              #########
         */

        class Burrow : Astar.IGraphAdapter<int>
        {
            public const int EMPTY = 0;
            public const int A = 1;
            public const int B = 10;
            public const int C = 100;
            public const int D = 1000;

            public readonly static int[][] Layout = new int[15][];
            public readonly static int[][] Distances = new int[15][];
            public readonly static int[] EncodedIndexMapper = new int[] { A, A, B, B, C, C, D, D };
            public readonly static Dictionary<(int from, int to), int[]> Paths = new();

            static Burrow()
            {
                Layout[0x0] = new int[] { 1 };
                Layout[0x1] = new int[] { 0, 2, 7 };
                Layout[0x2] = new int[] { 1, 3, 7, 9 };
                Layout[0x3] = new int[] { 2, 4, 9, 0xB };
                Layout[0x4] = new int[] { 3, 5, 0xB, 0xD };
                Layout[0x5] = new int[] { 4, 6, 0xD };
                Layout[0x6] = new int[] { 5 };
                Layout[0x7] = new int[] { 1, 2, 8 };
                Layout[0x8] = new int[] { 7 };
                Layout[0x9] = new int[] { 2, 3, 0xA };
                Layout[0xA] = new int[] { 9 };
                Layout[0xB] = new int[] { 3, 4, 0xC };
                Layout[0xC] = new int[] { 0xB };
                Layout[0xD] = new int[] { 4, 5, 0xE };
                Layout[0xE] = new int[] { 0xD };

                //                            0   1   2   3   4   5   6   7   8   9   A   B   C   D   E
                Distances[0x0] = new int[] {  0,  1,  3,  5,  7,  9, 10,  3,  4,  5,  6,  7,  8,  9, 10 };
                Distances[0x1] = new int[] {  1,  0,  2,  4,  6,  8,  9,  2,  3,  4,  5,  6,  7,  8,  9 };
                Distances[0x2] = new int[] {  3,  2,  0,  2,  4,  6,  7,  2,  3,  2,  3,  4,  5,  6,  7 };
                Distances[0x3] = new int[] {  5,  4,  2,  0,  2,  4,  5,  4,  5,  2,  3,  2,  3,  4,  5 };
                Distances[0x4] = new int[] {  7,  6,  4,  2,  0,  2,  3,  6,  7,  4,  5,  2,  3,  2,  3 };
                Distances[0x5] = new int[] {  9,  8,  6,  4,  2,  0,  1,  8,  9,  6,  7,  4,  5,  2,  3 };
                Distances[0x6] = new int[] { 10,  9,  7,  5,  3,  1,  0,  9, 10,  7,  8,  5,  6,  3,  4 };
                Distances[0x7] = new int[] {  3,  2,  2,  4,  6,  8,  9,  0,  1,  4,  5,  6,  7,  8,  9 };
                Distances[0x8] = new int[] {  4,  3,  3,  5,  7,  9, 10,  1,  0,  5,  6,  7,  8,  9, 10 };
                Distances[0x9] = new int[] {  5,  4,  2,  2,  4,  6,  7,  4,  5,  0,  1,  4,  5,  6,  7 };
                Distances[0xA] = new int[] {  6,  5,  3,  3,  5,  7,  8,  5,  6,  1,  0,  5,  6,  7,  8 };
                Distances[0xB] = new int[] {  7,  6,  4,  2,  2,  4,  5,  6,  7,  4,  5,  0,  1,  4,  5 };
                Distances[0xC] = new int[] {  8,  7,  5,  3,  3,  5,  6,  7,  8,  5,  6,  1,  0,  5,  6 };
                Distances[0xD] = new int[] {  9,  8,  6,  4,  2,  2,  3,  8,  9,  6,  7,  4,  5,  0,  1 };
                Distances[0xE] = new int[] { 10,  9,  7,  5,  3,  3,  4,  9, 10,  7,  8,  5,  6,  1,  0 };

                Burrow burrow = new();
                for (var from = 0; from < 15; from++)
                {
                    for (var to = 0; to < 15; to++)
                    {
                        if (from == to) continue;
                        Paths[(from, to)] = burrow.Path(from, to);
                    }
                }
            }

            public static (int, int) Goals(int type)
            {
                return type switch
                {
                    A => (0x7, 0x8),
                    B => (0x9, 0xA),
                    C => (0xB, 0xC),
                    D => (0xD, 0xE),
                    _ => throw new InvalidOperationException()
                };
            }

            public static bool IsRoom(int position) => position > 6;

            public static bool IsValidRoom(int[] state, int type, int position)
            {
                switch (type)
                {
                    case A:
                        if (position == 0x8) return true;
                        if (position == 0x7 && state[0x8] == A) return true;
                        return false;

                    case B:
                        if (position == 0xA) return true;
                        if (position == 0x9 && state[0xA] == B) return true;
                        return false;

                    case C:
                        if (position == 0xC) return true;
                        if (position == 0xB && state[0xC] == C) return true;
                        return false;

                    case D:
                        if (position == 0xE) return true;
                        if (position == 0xD && state[0xE] == D) return true;
                        return false;
                }
                throw new InvalidOperationException();
            }

            public static int ReadState(uint state, int index)
            {
                state >>= (index * 4);
                return (int)(state & 0xF);
            }

            public static uint WriteState(uint state, int index, int value)
            {
                if (value < 0 || 14 < value) throw new InvalidOperationException();
                uint mask = ~(uint)(0xF << (index * 4));
                value <<= (index * 4);
                
                return (state & mask) | (uint)value;
            }

            public static int[] DecodeState(uint state)
            {
                var s = new int[15];
                s[ReadState(state, 0)] = A;
                s[ReadState(state, 1)] = A;
                s[ReadState(state, 2)] = B;
                s[ReadState(state, 3)] = B;
                s[ReadState(state, 4)] = C;
                s[ReadState(state, 5)] = C;
                s[ReadState(state, 6)] = D;
                s[ReadState(state, 7)] = D;
                return s;
            }

            public static uint EncodeState(int[] state)
            {
                bool setA = false, setB = false, setC = false, setD = false;
                uint encoded = 0xFFFFFFFF;

                for (int i = 0; i < 15; i++)
                {
                    switch (state[i])
                    {
                        case A:
                            encoded = WriteState(encoded, setA ? 1 : 0, i);
                            setA = true;
                            break;

                        case B:
                            encoded = WriteState(encoded, setB ? 3 : 2, i);
                            setB = true;
                            break;

                        case C:
                            encoded = WriteState(encoded, setC ? 5 : 4, i);
                            setC = true;
                            break;

                        case D:
                            encoded = WriteState(encoded, setD ? 7 : 6, i);
                            setD = true;
                            break;
                    }
                }

                return encoded;
            }

            public IEnumerable<int> GetLinked(int node)
            {
                for (int i = 0; i < Layout[node].Length; i++)
                    yield return Layout[node][i];
            }

            static int Distance(int from, int to) => Distances[from][to];

            public int GetMoveCost(int from, int to) => Distances[from][to];

            public int GetScore(int from, int to) => Distance(from, to);

            public bool NodesEqual(int a, int b) => a == b;

            public int[] Path(int from, int to)
            {
                var path = Astar.FindPath(this, from, to);
                return path.ToArray();
            }

            public static bool HasPath(uint state, int from, int to)
            {
                return HasPath(DecodeState(state), from, to);
            }

            public static bool HasPath(int[] state, int from, int to)
            {
                var path = Paths[(from, to)];
                for (var i = 1; i < path.Length; i++)
                {
                    if (state[path[i]] != EMPTY) return false;
                }
                return true;
            }
        }

        class Solver
        {
            public const uint GOAL_STATE = 0xEDCBA987;

            readonly static int[][] Routes = new int[15][];

            readonly static HashSet<uint> DebugStates;

            static Solver()
            {
                Routes[0x0] = new int[] { 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE };
                Routes[0x1] = new int[] { 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE };
                Routes[0x2] = new int[] { 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE };
                Routes[0x3] = new int[] { 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE };
                Routes[0x4] = new int[] { 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE };
                Routes[0x5] = new int[] { 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE };
                Routes[0x6] = new int[] { 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE };
                Routes[0x7] = new int[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6 };
                Routes[0x8] = new int[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6 };
                Routes[0x9] = new int[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6 };
                Routes[0xA] = new int[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6 };
                Routes[0xB] = new int[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6 };
                Routes[0xC] = new int[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6 };
                Routes[0xD] = new int[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6 };
                Routes[0xE] = new int[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6 };
            }

            readonly int[] State;
            readonly HashSet<uint> Visited = new();
            readonly Dictionary<uint, long> CostToState = new(); 
            public long MinScore = long.MaxValue;


            public Solver(uint state)
            {
                State = Burrow.DecodeState(state);
            }

            public void Explore(long scoreSoFar)
            {
                var encoded = Burrow.EncodeState(State);

                if (encoded == GOAL_STATE)
                {
                    if (scoreSoFar < MinScore) MinScore = scoreSoFar;
                    return;
                }

                if (CostToState.TryGetValue(encoded, out var prevScore))
                {
                    if (prevScore < scoreSoFar)
                    {
                        return;
                    }
                }
                CostToState[encoded] = scoreSoFar;

                if (Visited.Contains(encoded)) return;
                Visited.Add(encoded);

                for (var from = 0; from < 15; from++)
                {
                    if (State[from] == Burrow.EMPTY) continue;
                    if (Burrow.IsValidRoom(State, State[from], from)) continue;

                    var routes = Routes[from];

                    for (var i = 0; i < routes.Length; i++)
                    {
                        var to = routes[i];

                        if (State[to] != Burrow.EMPTY) continue;
                        if (Burrow.IsRoom(to))
                        {
                            if (!Burrow.IsValidRoom(State, State[from], to))
                            {
                                continue;
                            }
                        }

                        if (!Burrow.HasPath(State, from, to)) continue;

                        var moveScore = State[from] * Burrow.Distances[from][to];
                        State[to] = State[from];
                        State[from] = Burrow.EMPTY;
                        Explore(scoreSoFar + moveScore);
                        State[from] = State[to];
                        State[to] = Burrow.EMPTY;
                    }
                }

                Visited.Remove(encoded);
            }

            char StateChar(int index)
            {
                return State[index] switch
                {
                    Burrow.A => 'A',
                    Burrow.B => 'B',
                    Burrow.C => 'C',
                    Burrow.D => 'D',
                    Burrow.EMPTY => '.',
                    _ => '?'
                };
            }

            public void Render()
            {
                Debug.WriteLine("#############");
                Debug.WriteLine($"#{StateChar(0)}{StateChar(1)}.{StateChar(2)}.{StateChar(3)}.{StateChar(4)}.{StateChar(5)}{StateChar(6)}#");
                Debug.WriteLine($"###{StateChar(7)}#{StateChar(9)}#{StateChar(0xB)}#{StateChar(0xD)}###");
                Debug.WriteLine($"  #{StateChar(8)}#{StateChar(0xA)}#{StateChar(0xC)}#{StateChar(0xE)}#");
                Debug.WriteLine($"  #########");
            }
        }

        [Fact]
        public void VerifyDistances()
        {
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    Burrow.Distances[i][j].Should().Be(Burrow.Distances[j][i]);
                }
            }
        }

        [Theory]
        [InlineData(0x12345678, 0, 8)]
        [InlineData(0x12345678, 1, 7)]
        [InlineData(0x12345678, 2, 6)]
        [InlineData(0x12345678, 3, 5)]
        [InlineData(0x12345678, 4, 4)]
        [InlineData(0x12345678, 5, 3)]
        [InlineData(0x12345678, 6, 2)]
        [InlineData(0xE2345678, 7, 0xE)]
        public void ReadStateTest(uint state, int index, int expectedValue)
        {
            Burrow.ReadState(state, index).Should().Be(expectedValue);
        }

        [Theory]
        [InlineData(0x12345670, 0, 8, 0x12345678)]
        [InlineData(0x12345608, 1, 7, 0x12345678)]
        [InlineData(0x12345078, 2, 6, 0x12345678)]
        [InlineData(0x12340678, 3, 5, 0x12345678)]
        [InlineData(0x12335678, 4, 4, 0x12345678)]
        [InlineData(0x12545678, 5, 3, 0x12345678)]
        [InlineData(0x10345678, 6, 2, 0x12345678)]
        [InlineData(0x02345678, 7, 14, 0xE2345678)]
        public void WriteStateTest(uint state, int index, int value, uint expectedState)
        {
            Burrow.WriteState(state, index, value).Should().Be(expectedState);
        }

        [Theory]
        [InlineData(0x87654321, 0x87654321)]
        [InlineData(0x78563412, 0x87654321)]
        [InlineData(Solver.GOAL_STATE, Solver.GOAL_STATE)]
        public void DecodeEncodeStateTest(uint encoded, uint expectedEncoded)
        {
            var state = Burrow.DecodeState(encoded);
            Burrow.EncodeState(state).Should().Be(expectedEncoded);
        }

        [Theory]
        [InlineData(0x00000000, 0x8, 0xE, true)]
        [InlineData(0x00000000, 0x8, 0x6, true)]
        [InlineData(0x00000000, 0x8, 0x0, false)]
        [InlineData(0x00000000, 0xA, 0xC, true)]
        [InlineData(0x30000000, 0xA, 0xC, false)]
        [InlineData(0x03000000, 0xA, 0xC, false)]
        [InlineData(0x00300000, 0xA, 0xC, false)]
        [InlineData(0x00030000, 0xA, 0xC, false)]
        [InlineData(0x00003000, 0xA, 0xC, false)]
        [InlineData(0x00000300, 0xA, 0xC, false)]
        [InlineData(0x00000030, 0xA, 0xC, false)]
        [InlineData(0x00000003, 0xA, 0xC, false)]
        [InlineData(0x42EC217A, 0xA, 0xB, true)]
        public void HasPathTest(uint state, int from, int to, bool expectedResult)
        {
            Burrow.HasPath(state, from, to).Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(0xDA9C7B8E, 12521)]
        [InlineData(0xC98AEDB7, 13495)]
        public void Part1(uint state, int expectedAnswer)
        {
            Solver solver = new(state);
            solver.Explore(0);
            solver.MinScore.Should().Be(expectedAnswer);
        }
    }
}
