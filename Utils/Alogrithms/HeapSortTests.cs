using Xunit;
using FluentAssertions;

namespace Utils.Alogrithms
{
    public class HeapSortTests
    {
        static int CompareInts_InOrder(int a, int b) => a - b;
        static int CompareInts_ReverseOrder(int a, int b) => b - a;

        [Theory]
        [InlineData(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15)]
        [InlineData(15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0)]
        [InlineData(9, 12, 2, 13, 3, 15, 5, 1, 6, 7, 4, 11, 0, 8, 14, 10)]
        [InlineData(15, 7, 9, 12, 13, 3, 6, 1, 2, 5, 10, 4, 11, 0, 8, 14)]
        public void Sort_IntoOrder(params int[] input)
        {
            HeapSort.Sort(input, CompareInts_InOrder);

            for (var i = 0; i < input.Length; i++)
                input[i].Should().Be(i);
        }

        [Theory]
        [InlineData(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15)]
        [InlineData(15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0)]
        [InlineData(9, 12, 2, 13, 3, 15, 5, 1, 6, 7, 4, 11, 0, 8, 14, 10)]
        [InlineData(15, 7, 9, 12, 13, 3, 6, 1, 2, 5, 10, 4, 11, 0, 8, 14)]
        public void Sort_IntoReverseOrder(params int[] input)
        {
            HeapSort.Sort(input, CompareInts_ReverseOrder);

            for (var i = 0; i < input.Length; i++)
                input[i].Should().Be(15 - i);
        }
    }
}
