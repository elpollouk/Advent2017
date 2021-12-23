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
            //readonly static Func<(byte, byte, byte, byte, byte, byte, byte, byte), int>[] StateByIndexGetters = new Func<(byte, byte, byte, byte, byte, byte, byte, byte), int>[8];

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

                /*StateByIndexGetters[0] = t => t.Item1;
                StateByIndexGetters[1] = t => t.Item2;
                StateByIndexGetters[2] = t => t.Item3;
                StateByIndexGetters[3] = t => t.Item4;
                StateByIndexGetters[4] = t => t.Item5;
                StateByIndexGetters[5] = t => t.Item6;
                StateByIndexGetters[6] = t => t.Item7;
                StateByIndexGetters[7] = t => t.Item7;*/
            }

            public uint State;

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

            public Burrow(uint state)
            {
                State = state;
            }

            static (byte, byte, byte, byte, byte, byte, byte, byte) EncodeState(int[] state)
            {
                byte a1 = 255, a2 = 255, b1 = 255, b2 = 255, c1 = 255, c2 = 255, d1 = 255, d2 = 255;

                //for (int i = 0; i < )

                return (a1, a2, b1, b2, c1, c2, d1, d2);
            }

            public IEnumerable<int> GetLinked(int node)
            {
                for (int i = 0; i < Layout[node].Length; i++)
                {
                    yield return Layout[node][i];
                }
            }

            static int Distance(int from, int to)
            {
                return Distances[from][to];
            }

            public int GetMoveCost(int from, int to) => 1;

            public int GetScore(int from, int to)
            {
                return Distance(from, to);
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
        [InlineData(0x12345678, 7, 1)]
        public void ReadStateTest(uint state, int index, int expectedValue)
        {
            Burrow.ReadState(state, index).Should().Be(expectedValue);
        }

        [Theory]
        [InlineData(0x12345670, 0, 8, 0x12345678)]
        [InlineData(0x12345608, 1, 7, 0x12345678)]
        [InlineData(0x12345078, 2, 6, 0x12345678)]
        [InlineData(0x12340678, 3, 5, 0x12345678)]
        [InlineData(0x12305678, 4, 4, 0x12345678)]
        [InlineData(0x12045678, 5, 3, 0x12345678)]
        [InlineData(0x10345678, 6, 2, 0x12345678)]
        [InlineData(0x02345678, 7, 14, 0xE2345678)]
        public void WriteStateTest(uint state, int index, int value, uint expectedState)
        {
            Burrow.WriteState(state, index, value).Should().Be(expectedState);
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
