using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        public const int A = 1;
        public const int B = 10;
        public const int C = 100;
        public const int D = 1000;
        public const int EMPTY = 0;

        static int CharToType(char c) => c switch
        {
            'A' => A,
            'B' => B,
            'C' => C,
            'D' => D,
            _ => throw new InvalidOperationException()
        };

        static char TypeToChar(int type) => type switch
        {
            A => 'A',
            B => 'B',
            C => 'C',
            D => 'D',
            EMPTY => '.',
            _ => throw new InvalidOperationException()
        };

    class Room
        {
            private readonly Stack<int> _stack = new();

            public int Id { get; init; }
            private readonly int _maxCapacity;

            public bool IsReady { get; private set; }
            public bool IsSolved => IsReady && _stack.Count == _maxCapacity;
            public bool CanEnter => IsReady && _stack.Count < _maxCapacity;
            public bool CanExit => !IsReady && _stack.Count != 0;


            public Room(int id, string initialState)
            {
                Id = id;
                _maxCapacity = initialState.Length;
                foreach (var c in initialState)
                    _stack.Push(CharToType(c));
            }

            public int Enter(int type)
            {
                if (type != Id) throw new InvalidOperationException("Wrong type");
                if (!CanEnter) throw new InvalidOperationException("Unenterable");
                var cost = _maxCapacity - _stack.Count;
                _stack.Push(type);
                return cost;
            }

            public (int type, int cost, bool wasReady) Exit()
            {
                if (!CanExit) throw new InvalidOperationException("Unexitable");
                var type = _stack.Pop();
                var wasReady = IsReady;
                IsReady = _stack.Count == 0 || _stack.All(t => t == Id);
                return (type, _maxCapacity - _stack.Count, wasReady);
            }

            public void UndoExit(int type)
            {
                _stack.Push(type);
                IsReady = false;
            }

            public void UndoEnter()
            {
                _stack.Pop();
            }

            public void GetState(StringBuilder sb)
            {
                foreach (var v in _stack)
                    sb.Append(TypeToChar(v));
            }
        }

        class Burrow
        {
            static readonly int[] PathsFromRooms = new int[] { 0, 1, 3, 5, 7, 9, 10 };
            static readonly int[] PathsToRooms = new int[] { 2, 4, 6, 8 };

            public int MinCost { get; private set; } = int.MaxValue;

            readonly int[] _burrow = new int[11];
            readonly Room[] _rooms = new Room[4];
            readonly HashSet<string> visited = new();
            readonly Dictionary<string, int> minStateCosts = new();

            public Burrow(string room1, string room2, string room3, string room4)
            {
                _rooms[0] = new Room(A, room1);
                _rooms[1] = new Room(B, room2);
                _rooms[2] = new Room(C, room3);
                _rooms[3] = new Room(D, room4);
            }

            Room GetRoom(int burrowIndex)
            {
                return burrowIndex switch
                {
                    2 => _rooms[0],
                    4 => _rooms[1],
                    6 => _rooms[2],
                    8 => _rooms[3],
                    _ => null
                };
            }

            bool CanTravelTo(int from, int to)
            {
                int step = to < from ? -1 : 1;
                while (from != to)
                {
                    from += step;
                    if (_burrow[from] != EMPTY)
                        return false;
                }

                return true;
            }

            bool IsSolved()
            {
                for (var i = 0; i < _rooms.Length; i++)
                    if (!_rooms[i].IsSolved)
                        return false;

                return true;
            }

            public void Explore(int costSoFar)
            {
                if (IsSolved())
                {
                    if (costSoFar < MinCost) MinCost = costSoFar;
                    return;
                }

                var state = ToString();
                if (minStateCosts.TryGetValue(state, out var prevCost))
                {
                    if (prevCost <= costSoFar)
                        return;
                }
                minStateCosts[state] = costSoFar;

                if (visited.Contains(state)) return;
                visited.Add(state);

                for (var from = 0; from < _burrow.Length; from++)
                {
                    var type = _burrow[from];
                    if (type == EMPTY)
                    {
                        var room = GetRoom(from);
                        if (room == null) continue;
                        if (!room.CanExit) continue;

                        // Explore out of a room into the hallway
                        for (int i = 0; i < PathsFromRooms.Length; i++)
                        {
                            var to = PathsFromRooms[i];
                            if (!CanTravelTo(from, to)) continue;

                            var result = room.Exit();
                            var cost = Math.Abs(from - to);
                            cost += result.cost;
                            cost *= result.type;
                            _burrow[to] = result.type;
                            Explore(cost + costSoFar);
                            _burrow[to] = EMPTY;
                            room.UndoExit(result.type);
                        }
                    }

                    // Explore into a room
                    for (int i = 0; i < PathsToRooms.Length; i++)
                    {
                        var to = PathsToRooms[i];

                        var room = GetRoom(to);
                        if (room.Id != type) continue;
                        if (!room.CanEnter) break;
                        if (!CanTravelTo(from, to)) break;

                        var cost = room.Enter(type);
                        cost += Math.Abs(from - to);
                        cost *= type;
                        _burrow[from] = EMPTY;
                        Explore(costSoFar + cost);
                        _burrow[from] = type;
                        room.UndoEnter();
                    }
                }

                visited.Remove(state);
            }

            public override string ToString()
            {
                StringBuilder sb = new();
                for (int i = 0; i < _burrow.Length; i++)
                    sb.Append(TypeToChar(_burrow[i]));

                for (int i = 0; i < _rooms.Length; i++) {
                    sb.Append(':');
                    _rooms[i].GetState(sb);
                }

                return sb.ToString();
            }
        }
  

        [Theory]
        [InlineData("AB", "DC", "CB", "AD", 12521)] // Example 1
        [InlineData("CA", "CD", "DA", "BB", 13495)] // Part 1
        [InlineData("ADDB", "DBCC", "CABB", "ACAD", 44169)] // Example 2
        [InlineData("CDDA", "CBCD", "DABA", "BCAB", 53767)] // Part 2
        public void Part1(string room1, string room2, string room3, string room4, int expectedAnswer)
        {
            Burrow burrow = new(room1, room2, room3, room4);
            burrow.Explore(0);
            burrow.MinCost.Should().Be(expectedAnswer);
        }
    }
}
