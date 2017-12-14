using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Adevent2017.Alogrithms
{
    public class PriorityQueueTests
    {
        [Fact]
        public void Test()
        {
            var queue = new PriorityQueue<string>();

            queue.Enqueue("C", 2);
            queue.Enqueue("A", 1);
            queue.Enqueue("B", 2);
            queue.Enqueue("E", 5);
            queue.Enqueue("D", 3);

            queue.Dequeue().Should().Be("A");
            queue.Dequeue().Should().Be("B");
            queue.Dequeue().Should().Be("C");
            queue.Dequeue().Should().Be("D");
            queue.Dequeue().Should().Be("E");
        }
    }
}
