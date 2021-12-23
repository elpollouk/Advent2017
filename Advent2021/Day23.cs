using FluentAssertions;
using System;
using System.Collections.Generic;
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
            const int EMPTY = 0;
            const int A = 1;
            const int B = 10;
            const int C = 100;
            const int D = 1000;

            public readonly static int[][] Layout = new int[15][];
            public readonly static int[][] Distances = new int[15][];
            public readonly static int[] EncodedIndexMapper = new int[] { A, A, B, B, C, C, D, D };

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
            }

            public static bool IsHallway(int position) => position <= 6;

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

            public static bool IsValidRoom(int[] state, int type, int position)
            {
                switch (type)
                {
                    case A:
                        if (position == 0x8) return true;
                        if (position == 0x7 && state[0x8] == EMPTY) return true;
                        return false;

                    case B:
                        if (position == 0xA) return true;
                        if (position == 0x9 && state[0xA] == EMPTY) return true;
                        return false;

                    case C:
                        if (position == 0xC) return true;
                        if (position == 0xB && state[0xC] == EMPTY) return true;
                        return false;

                    case D:
                        if (position == 0xE) return true;
                        if (position == 0xD && state[0xE] == EMPTY) return true;
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

            public int[] State;

            public Burrow(int[] state)
            {
                State = state;
            }

            public IEnumerable<int> GetLinked(int node)
            {
                for (int i = 0; i < Layout[node].Length; i++)
                {
                    var linked = Layout[node][i];
                    if (State[linked] != EMPTY) continue;
                    yield return linked;
                }
            }

            static int Distance(int from, int to)
            {
                return Distances[from][to];
            }

            public int GetMoveCost(int from, int to) => Distances[from][to];

            public int GetScore(int from, int to) => Distance(from, to);

            public bool NodesEqual(int a, int b) => a == b;

            public static bool HasPath(uint state, int from, int to)
            {
                return HasPath(DecodeState(state), from, to);
            }

            public static bool HasPath(int[] state, int from, int to)
            {
                Burrow burrow = new(state);
                var path = Astar.FindPath(burrow, from, to);
                return path != null;
            }
        }

        class Solver : Astar.IGraphAdapter<uint>
        {
            public const uint GOAL_STATE = 0xDEBC9A78;

            private static (int from, int to, int cost) StateDelta(uint stateFrom, uint stateTo)
            {
                for (int i = 0; i < 8; i++)
                {
                    var from = stateFrom & 0xF;
                    var to = stateTo & 0xF;
                    if (from != to) return ((int)from, (int)to, Burrow.EncodedIndexMapper[i]);

                    stateFrom >>= 4;
                    stateTo >>= 4;
                }
                throw new InvalidOperationException();
            }

            private static int ScoreToGoal(int type, int pos1, int pos2)
            {
                (var goal1, var goal2) = Burrow.Goals(type);
                var score1 = Burrow.Distances[pos1][goal1] + Burrow.Distances[pos2][goal2];
                var score2 = Burrow.Distances[pos2][goal1] + Burrow.Distances[pos1][goal2];

                return score1 < score2 ? score1 : score2;
            }

            public IEnumerable<uint> GetLinked(uint node)
            {
                var state = Burrow.DecodeState(node);
                var originalState = node;
                for (var i = 0; i < 8; i++)
                {
                    int pos = (int)(node & 0xF);
                    node >>= 4;
                    var links = Burrow.Layout[pos];
                    for (var j = 0; j < links.Length; j++)
                    {
                        if (Burrow.IsHallway(pos))
                        {
                            if (!Burrow.IsValidRoom(state, Burrow.EncodedIndexMapper[i], links[j]))
                            {
                                continue;
                            }
                        }
                        else if (!Burrow.IsHallway(links[j]))
                        {
                            continue;
                        }

                        if (!Burrow.HasPath(state, pos, links[j])) continue;

                        var newState = Burrow.WriteState(originalState, i, links[j]);
                        yield return newState;
                    }
                }
                throw new InvalidOperationException();
            }

            public int GetMoveCost(uint from, uint to)
            {
                (var f, var t, var cost) = StateDelta(from, to);
                return Burrow.Distances[f][t] * cost;
            }

            public int GetScore(uint from, uint to)
            {
                int score = 0;

                for (var i = 0; i < 4; i++)
                {
                    var a = (int)(from & 0xF);
                    from >>= 4;
                    var b = (int)(from & 0xF);
                    from >>= 4;
                    score += ScoreToGoal(Burrow.EncodedIndexMapper[i * 2], a, b);
                }

                return score;
            }

            public bool NodesEqual(uint a, uint b) => GetScore(a, b) == 0;
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
        [InlineData(0x56431280, 21)]
        [InlineData(0x87EDBCA9, 48)]
        [InlineData(0xDEBC9A78, 0)]
        [InlineData(0xEDCBA987, 0)]
        public void SolverGetScoreTest(uint state, int expectedScore)
        {
            var solver = new Solver();
            solver.GetScore(state, Solver.GOAL_STATE).Should().Be(expectedScore);
        }

        [Theory]
        [InlineData(0xDA9C7B8E, 12521)]
        public void Part1(uint state, int expectedAnswer)
        {
            Solver solver = new();
            var path = Astar.FindPath(solver, state, 0u);
            path.Count.Should().Be(expectedAnswer);
        }

        /*[Theory]
        [InlineData("Data/Day23_Test.txt", 0)]
        [InlineData("Data/Day23.txt", 0)]
        public void Part1(string filename, long expectedAnswer)
        {
            var burrow = new Burrow();
        }

        [Theory]
        [InlineData("Data/Day23_Test.txt", 0)]
        [InlineData("Data/Day23.txt", 0)]
        public void Part2(string filename, long expectedAnswer)
        {

        }*/
    }
}
