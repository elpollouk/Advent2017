using Utils;
using FluentAssertions;
using System.IO;
using Xunit;

namespace Adevent2017
{
    public class Problem0901
    {
        int removeCharacters = 0;

        int SkipGarbage(string input, int start)
        {
            var offset = start + 1;

            while (true)
            {
                if (offset == input.Length) Oh.Bollocks();
                switch(input[offset])
                {
                    case '>':
                        removeCharacters += (offset - start) - 1;
                        return offset - start;

                    case '!':
                        removeCharacters -= 2;
                        offset += 2;
                        break;

                    default:
                        offset++;
                        break;
                }
            }
        }

        int ScoreGroup(string input)
        {
            int index = 0;
            int score = 0;
            int nextScoreValue = 0;

            while (index < input.Length)
            {
                switch (input[index])
                {
                    case '{':
                        nextScoreValue++;
                        break;

                    case '}':
                        score += nextScoreValue;
                        nextScoreValue--;
                        break;

                    case '<':
                        index += SkipGarbage(input, index);
                        break;
                }
                index++;
            }

            return score;
        }

        [Theory]
        [InlineData("<>", 0)]
        [InlineData("<fhsdgahdfg>", 10)]
        [InlineData("<<<<>", 3)]
        [InlineData("<{!>}>", 2)]
        [InlineData("<!!>", 0)]
        [InlineData("<!!!>>", 0)]
        [InlineData("<{o\"i!a,<{i<a>", 10)]
        public void SkipGarbageTest(string input, int removed)
        {
            SkipGarbage(input, 0).Should().Be(input.Length - 1);
            removeCharacters.Should().Be(removed);
        }

        [Theory]
        [InlineData("{}", 1)]
        [InlineData("{{{}}}", 6)]
        [InlineData("{{},{}}", 5)]
        [InlineData("{{{},{},{{}}}}", 16)]
        [InlineData("{<a>,<a>,<a>,<a>}", 1)]
        [InlineData("{{<ab>},{<ab>},{<ab>},{<ab>}}", 9)]
        [InlineData("{{<!!>},{<!!>},{<!!>},{<!!>}}", 9)]
        [InlineData("{{<a!>},{<a!>},{<a!>},{<ab>}}", 3)]
        public void ScoreGroupTest(string input, int answer) => ScoreGroup(input).Should().Be(answer);

        [Fact]
        public void Solution()
        {
            var input = File.ReadAllText("Data/0901.txt");
            ScoreGroup(input).Should().Be(16689);
            removeCharacters.Should().Be(7982);
        }
    }
}
