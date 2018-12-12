using FluentAssertions;
using Xunit;

namespace Utils.DataStructures
{
    public class PriorityQueueTests
    {
        [Fact]
        public void Integer()
        {
            var queue = new PriorityQueue<string>();

            queue.Enqueue("C", 3);
            queue.Enqueue("A", 1);
            queue.Enqueue("B", 2);
            queue.Enqueue("E", 5);
            queue.Enqueue("D", 4);

            queue.Dequeue().Should().Be("A");
            queue.Dequeue().Should().Be("B");
            queue.Dequeue().Should().Be("C");
            queue.Dequeue().Should().Be("D");
            queue.Dequeue().Should().Be("E");
        }


        [Fact]
        public void Double()
        {
            var queue = new PriorityQueue<string, double>();

            queue.Enqueue("C", 0.2);
            queue.Enqueue("A", 0.1);
            queue.Enqueue("B", 0.2);
            queue.Enqueue("E", 5);
            queue.Enqueue("D", 0.3);
            queue.Enqueue("Z", -1234.567);

            queue.Dequeue().Should().Be("Z");
            queue.Dequeue().Should().Be("A");
            queue.Dequeue().Should().Be("B");
            queue.Dequeue().Should().Be("C");
            queue.Dequeue().Should().Be("D");
            queue.Dequeue().Should().Be("E");
        }

        [Fact]
        public void TryDequeue_EmptyQueue()
        {
            var queue = new PriorityQueue<string>();
            queue.TryDequeue(out string item).Should().BeFalse();
            item.Should().BeNull();
            queue.Count.Should().Be(0);
        }

        [Fact]
        public void TryDequeue_OneItemQueue()
        {
            var queue = new PriorityQueue<string>();
            queue.Enqueue("test", 0);
            queue.TryDequeue(out string item).Should().BeTrue();
            item.Should().Be("test");
            queue.Count.Should().Be(0);
        }

        [Fact]
        public void TryDequeue_TwoItemQueue()
        {
            var queue = new PriorityQueue<string>();
            queue.Enqueue("wrong item", 1);
            queue.Enqueue("right item", 0);
            queue.TryDequeue(out string item).Should().BeTrue();
            item.Should().Be("right item");
            queue.Count.Should().Be(1);
        }
    }
}
