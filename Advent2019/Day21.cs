using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Advent2019
{
    public class Day21
    {
        private readonly ITestOutputHelper output;
 
        public Day21(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Part1()
        {
            var vm = IntCode.CreateVM("Data/Day21.txt");
            vm.WriteLines(
                "NOT A J",
                "NOT B T",
                "OR T J",
                "NOT C T",
                "OR T J",
                "AND D J",
                "WALK"
            );

            vm.Execute();

            foreach (var line in vm.ReadLines())
                output.WriteLine(line);

            vm.Read().Should().Be(19357335);
        }

        [Fact]
        public void Part2()
        {
            var vm = IntCode.CreateVM("Data/Day21.txt");
            vm.WriteLines(
                "NOT A J",
                "NOT B T",
                "OR T J",
                "NOT C T",
                "OR T J",
                "AND D J",
                "AND H J",
                "NOT A T",
                "OR T J",
                "RUN"
            );

            vm.Execute();

            foreach (var line in vm.ReadLines())
                output.WriteLine(line);

            vm.Read().Should().Be(1140147758);
        }
    }
}
