using FluentAssertions;
using Xunit;

namespace Adevent2017.DataStructures
{
    public class HeapTests
    {
        [Fact]
        public void PushPop_RandomOrder()
        {
            var heap = new Heap<int, string>();

            heap.Push(4, "D");
            heap.Push(6, "F");
            heap.Push(2, "B");
            heap.Push(3, "C");
            heap.Push(15, "O");
            heap.Push(14, "N");
            heap.Push(10, "J");
            heap.Push(7, "G");
            heap.Push(11, "K");
            heap.Push(13, "M");
            heap.Push(1, "A");
            heap.Push(8, "H");
            heap.Push(12, "L");
            heap.Push(9, "I");
            heap.Push(5, "E");

            heap.Pop().Should().Be("A");
            heap.Pop().Should().Be("B");
            heap.Pop().Should().Be("C");
            heap.Pop().Should().Be("D");
            heap.Pop().Should().Be("E");
            heap.Pop().Should().Be("F");
            heap.Pop().Should().Be("G");
            heap.Pop().Should().Be("H");
            heap.Pop().Should().Be("I");
            heap.Pop().Should().Be("J");
            heap.Pop().Should().Be("K");
            heap.Pop().Should().Be("L");
            heap.Pop().Should().Be("M");
            heap.Pop().Should().Be("N");
            heap.Pop().Should().Be("O");
        }

        [Fact]
        public void PushPop_InOrder()
        {
            var heap = new Heap<int, string>();

            heap.Push(1, "A");
            heap.Push(2, "B");
            heap.Push(3, "C");
            heap.Push(4, "D");
            heap.Push(5, "E");
            heap.Push(6, "F");
            heap.Push(7, "G");
            heap.Push(8, "H");
            heap.Push(9, "I");
            heap.Push(10, "J");
            heap.Push(11, "K");
            heap.Push(12, "L");
            heap.Push(13, "M");
            heap.Push(14, "N");
            heap.Push(15, "O");

            heap.Pop().Should().Be("A");
            heap.Pop().Should().Be("B");
            heap.Pop().Should().Be("C");
            heap.Pop().Should().Be("D");
            heap.Pop().Should().Be("E");
            heap.Pop().Should().Be("F");
            heap.Pop().Should().Be("G");
            heap.Pop().Should().Be("H");
            heap.Pop().Should().Be("I");
            heap.Pop().Should().Be("J");
            heap.Pop().Should().Be("K");
            heap.Pop().Should().Be("L");
            heap.Pop().Should().Be("M");
            heap.Pop().Should().Be("N");
            heap.Pop().Should().Be("O");
        }

        [Fact]
        public void PushPop_ReverseOrder()
        {
            var heap = new Heap<int, string>();

            heap.Push(15, "O");
            heap.Push(14, "N");
            heap.Push(13, "M");
            heap.Push(12, "L");
            heap.Push(11, "K");
            heap.Push(10, "J");
            heap.Push(9, "I");
            heap.Push(8, "H");
            heap.Push(7, "G");
            heap.Push(6, "F");
            heap.Push(5, "E");
            heap.Push(4, "D");
            heap.Push(3, "C");
            heap.Push(2, "B");
            heap.Push(1, "A");

            heap.Pop().Should().Be("A");
            heap.Pop().Should().Be("B");
            heap.Pop().Should().Be("C");
            heap.Pop().Should().Be("D");
            heap.Pop().Should().Be("E");
            heap.Pop().Should().Be("F");
            heap.Pop().Should().Be("G");
            heap.Pop().Should().Be("H");
            heap.Pop().Should().Be("I");
            heap.Pop().Should().Be("J");
            heap.Pop().Should().Be("K");
            heap.Pop().Should().Be("L");
            heap.Pop().Should().Be("M");
            heap.Pop().Should().Be("N");
            heap.Pop().Should().Be("O");
        }

        [Fact]
        public void Count()
        {
            var heap = new Heap<int, int>();

            for (var i = 0; i < 10; i++)
            {
                heap.Count.Should().Be(i);
                heap.Push(i, i);
            }

            heap.Count.Should().Be(10);

            for (var i = 9; i >= 0; i--)
            {
                heap.Pop();
                heap.Count.Should().Be(i);
            }
        }
    }
}
