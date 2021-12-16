using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2021
{
    static class Scott
    {
        public static IEnumerable<Day16.Packet> Packets(this string data)
        {
            var bits = new Day16.BitReader(data);
            var packet = new Day16.Packet(bits);
            return packet.Packets();
        }

        public static IEnumerable<Day16.Packet> Packets(this Day16.Packet packet)
        {
            foreach (var p in packet.SubPackets)
                foreach (var p2 in p.Packets())
                    yield return p2;


            yield return packet;
        }
    }

    public class Day16
    {
        private const int TYPE_SUM = 0;
        private const int TYPE_PRODUCT = 1;
        private const int TYPE_MIN = 2;
        private const int TYPE_MAX = 3;
        private const int TYPE_LITERAL = 4;
        private const int TYPE_GT = 5;
        private const int TYPE_LT = 6;
        private const int TYPE_EQ = 7;

        private const int LENGTH_IN_BITS = 0;

        public class BitReader
        {
            private readonly string _data;
            private int _nextByte = 0;
            private Queue<int> _currentBits;
            public int ReadSoFar { get; private set; }

            public BitReader(string data)
            {
                _data = data;
                FillBits();
            }

            private void FillBits()
            {
                if (_data.Length <= _nextByte) throw new InvalidOperationException("No more data");
                _currentBits = _data[_nextByte++] switch
                {
                    '0' => new Queue<int>(new[] { 0, 0, 0, 0 }),
                    '1' => new Queue<int>(new[] { 0, 0, 0, 1 }),
                    '2' => new Queue<int>(new[] { 0, 0, 1, 0 }),
                    '3' => new Queue<int>(new[] { 0, 0, 1, 1 }),
                    '4' => new Queue<int>(new[] { 0, 1, 0, 0 }),
                    '5' => new Queue<int>(new[] { 0, 1, 0, 1 }),
                    '6' => new Queue<int>(new[] { 0, 1, 1, 0 }),
                    '7' => new Queue<int>(new[] { 0, 1, 1, 1 }),
                    '8' => new Queue<int>(new[] { 1, 0, 0, 0 }),
                    '9' => new Queue<int>(new[] { 1, 0, 0, 1 }),
                    'A' => new Queue<int>(new[] { 1, 0, 1, 0 }),
                    'B' => new Queue<int>(new[] { 1, 0, 1, 1 }),
                    'C' => new Queue<int>(new[] { 1, 1, 0, 0 }),
                    'D' => new Queue<int>(new[] { 1, 1, 0, 1 }),
                    'E' => new Queue<int>(new[] { 1, 1, 1, 0 }),
                    'F' => new Queue<int>(new[] { 1, 1, 1, 1 }),
                    _ => throw new InvalidOperationException("Unexpected character in data")
                };
            }

            public int ReadBits(int count)
            {
                if (count > 32) throw new InvalidOperationException("Attempt to read more that 32 bits");
                int result = 0;
                for (var i = 0; i <  count; i++)
                {
                    if (_currentBits.Count == 0) FillBits();
                    result <<= 1;
                    result |= _currentBits.Dequeue();
                }

                ReadSoFar += count;
                return result;
            }
        }

        public class Packet
        {
            public int Version { get; private init; }
            public int TypeId { get; private init; }
            public long Value { get; private init; }
            public List<Packet> SubPackets { get; private init; }

            public Packet(BitReader bits)
            {
                Version = bits.ReadBits(3);
                TypeId = bits.ReadBits(3);

                if (TypeId == TYPE_LITERAL)
                {
                    Value = ReadLiteral(bits);
                    SubPackets = new();
                }
                else
                {
                    Value = 0;
                    SubPackets = ReadSubPackets(bits);
                }
            }

            public long Eval()
            {
                return TypeId switch
                {
                    TYPE_SUM => SubPackets.Select(p => p.Eval()).Sum(),
                    TYPE_PRODUCT => SubPackets.Select(p => p.Eval()).Aggregate((a, b) => a * b),
                    TYPE_MIN => SubPackets.Select(p => p.Eval()).Min(),
                    TYPE_MAX => SubPackets.Select(p => p.Eval()).Max(),
                    TYPE_LITERAL => Value,
                    TYPE_GT => SubPackets[0].Eval() > SubPackets[1].Eval() ? 1 : 0,
                    TYPE_LT => SubPackets[0].Eval() < SubPackets[1].Eval() ? 1 : 0,
                    TYPE_EQ => SubPackets[0].Eval() == SubPackets[1].Eval() ? 1 : 0,
                    _ => throw new InvalidOperationException("Unsupported type id"),
                };
            }

            private static long ReadLiteral(BitReader bits)
            {
                long value = 0;
                bool lastPacket;
                do
                {
                    lastPacket = bits.ReadBits(1) == 0;
                    value <<= 4;
                    value |= (long)bits.ReadBits(4);
                }
                while (!lastPacket);

                return value;
            }

            private static List<Packet> ReadSubPackets(BitReader bits)
            {
                if (bits.ReadBits(1) == LENGTH_IN_BITS)
                {
                    return ReadSubPacketsByBits(bits);
                }

                return ReadSubPacketsByPacket(bits);
            }

            private static List<Packet> ReadSubPacketsByBits(BitReader bits)
            {
                List<Packet> packets = new();
                int numBits = bits.ReadBits(15);
                int statingCount = bits.ReadSoFar;

                while (bits.ReadSoFar < statingCount + numBits)
                {
                    var packet = new Packet(bits);
                    packets.Add(packet);
                }
                if (bits.ReadSoFar != statingCount + numBits) throw new InvalidOperationException("Too many bits read");

                return packets;
            }

            private static List<Packet> ReadSubPacketsByPacket(BitReader bits)
            {
                List<Packet> packets = new();

                int numPackets = bits.ReadBits(11);
                while (numPackets --> 0)
                {
                    var packet = new Packet(bits);
                    packets.Add(packet);
                }

                return packets;
            }
        }

        [Fact]
        public void ReadLiteral2021()
        {
            var reader = new BitReader("D2FE28");
            var packet = new Packet(reader);
            packet.Version.Should().Be(6);
            packet.TypeId.Should().Be(TYPE_LITERAL);
            packet.Value.Should().Be(2021);
        }

        [Fact]
        public void ReadSubPacketsViaBitCount()
        {
            var reader = new BitReader("38006F45291200");
            var packet = new Packet(reader);
            packet.Version.Should().Be(1);
            packet.TypeId.Should().Be(TYPE_LT);
            packet.SubPackets.Count.Should().Be(2);

            // First sub packet
            packet.SubPackets[0].Version.Should().Be(6);
            packet.SubPackets[0].TypeId.Should().Be(TYPE_LITERAL);
            packet.SubPackets[0].Value.Should().Be(10);

            // Second sub packet
            packet.SubPackets[1].Version.Should().Be(2);
            packet.SubPackets[1].TypeId.Should().Be(TYPE_LITERAL);
            packet.SubPackets[1].Value.Should().Be(20);
        }

        [Fact]
        public void ReadSubPacketsViaPacketCount()
        {
            var reader = new BitReader("EE00D40C823060");
            var packet = new Packet(reader);
            packet.Version.Should().Be(7);
            packet.TypeId.Should().Be(TYPE_MAX);
            packet.SubPackets.Count.Should().Be(3);

            // First sub packet
            packet.SubPackets[0].Version.Should().Be(2);
            packet.SubPackets[0].TypeId.Should().Be(TYPE_LITERAL);
            packet.SubPackets[0].Value.Should().Be(1);

            // Second sub packet
            packet.SubPackets[1].Version.Should().Be(4);
            packet.SubPackets[1].TypeId.Should().Be(TYPE_LITERAL);
            packet.SubPackets[1].Value.Should().Be(2);

            // Third sub packet
            packet.SubPackets[2].Version.Should().Be(1);
            packet.SubPackets[2].TypeId.Should().Be(TYPE_LITERAL);
            packet.SubPackets[2].Value.Should().Be(3);
        }

        int SumVersions(Packet root)
        {
            int sum = root.Version;
            foreach (var sp in root.SubPackets)
                sum += SumVersions(sp);

            return sum;
        }

        [Theory]
        [InlineData("8A004A801A8002F478", 16)]
        [InlineData("620080001611562C8802118E34", 12)]
        [InlineData("C0015000016115A2E0802F182340", 23)]
        [InlineData("A0016C880162017C3686B18A3D4780", 31)]
        public void Part1_Examples(string data, int expectedAnswer)
        {
            var bits = new BitReader(data);
            var packet = new Packet(bits);
            SumVersions(packet).Should().Be(expectedAnswer);
        }

        [Fact]
        public void Part1_Solution()
        {
            var data = FileIterator.Lines("Data/Day16.txt").First();
            var bits = new BitReader(data);
            var packet = new Packet(bits);
            SumVersions(packet).Should().Be(949);
        }

        [Fact]
        public void Part1_Scott()
        {
           FileIterator.Lines("Data/Day16.txt")
                .First()
                .Packets()
                .Select(p => p.Version)
                .Sum()
                .Should().Be(949);
        }

        [Theory]
        [InlineData("C200B40A82", 3)]
        [InlineData("04005AC33890", 54)]
        [InlineData("880086C3E88112", 7)]
        [InlineData("CE00C43D881120", 9)]
        [InlineData("D8005AC2A8F0", 1)]
        [InlineData("F600BC2D8F", 0)]
        [InlineData("9C005AC2F8F0", 0)]
        [InlineData("9C0141080250320F1802104A08", 1)]
        public void Part2_Examples(string data, int expectedAnswer)
        {
            var bits = new BitReader(data);
            var packet = new Packet(bits);
            packet.Eval().Should().Be(expectedAnswer);
        }

        [Fact]
        public void Part2_Solution()
        {
            var data = FileIterator.Lines("Data/Day16.txt").First();
            var bits = new BitReader(data);
            var packet = new Packet(bits);
            packet.Eval().Should().Be(1114600142730);
        }
    }
}
