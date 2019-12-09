using FluentAssertions;
using Utils;
using Xunit;

namespace Advent2019
{
    public class Day05
    {
        [Theory]
        [InlineData("Data/Day05-example2.txt", 7, 999)]
        [InlineData("Data/Day05-example2.txt", 8, 1000)]
        [InlineData("Data/Day05-example2.txt", 9, 1001)]
        public void Example2(string inputProg, long input, int expectedOutput)
        {
            long output = int.MinValue;
            var prog = FileIterator.LoadCSV<int>(inputProg);
            var vm = IntCode.CreateVM(prog);
            vm.State.Input = Generators.Reader(input);
            vm.State.Output = o => output = o;
            vm.Execute();

            output.Should().Be(expectedOutput);
        }

        [Theory]
        [InlineData("Data/Day05.txt", 1, 9775037)]
        [InlineData("Data/Day05.txt", 5, 15586959)]
        public void Problem(string inputProgram, long input, int answer)
        {
            long output = int.MinValue;
            var prog = FileIterator.LoadCSV<int>(inputProgram);
            var vm = IntCode.CreateVM(prog);
            vm.State.Input = Generators.Reader(input);
            vm.State.Output = o => output = o;
            vm.Execute();

            output.Should().Be(answer);
        }
    }
}
