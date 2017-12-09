using FluentAssertions;
using System;
using System.IO;
using Xunit;

namespace Adevent2017
{
    public class Problem0901
    {
        int removeCharacters = 0;

        int IsGarbage(string input, int start)
        {
            if (input[start] != '<') return 0;
            var offset = start;

            offset++;
            while (true)
            {
                if (offset == input.Length) throw new Exception("Get tae fuck!!");
                switch(input[offset])
                {
                    case '>':
                        removeCharacters += (offset - start) - 1;
                        return (offset + 1) - start;

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
            int index = 1;
            int score = 0;
            int scoreValue = 1;

            while (index < input.Length)
            {
                switch (input[index])
                {
                    case '{':
                        scoreValue++;
                        index++;
                        break;

                    case '}':
                        score += scoreValue;
                        scoreValue--;
                        index++;
                        break;

                    case '<':
                        index += IsGarbage(input, index);
                        break;

                    default:
                        index++;
                        break;
                }
            }

            return score;
        }

        int Solve(string[] input)
        {
            return -1;
        }

        [Theory]
        [InlineData("<>", 0)]
        [InlineData("<fhsdgahdfg>", 10)]
        [InlineData("<<<<>", 3)]
        [InlineData("<{!>}>", 2)]
        [InlineData("<!!>", 0)]
        [InlineData("<!!!>>", 0)]
        [InlineData("<{o\"i!a,<{i<a>", 10)]
        public void IsGarbageTest(string input, int removed)
        {
            removeCharacters = 0;
            IsGarbage(input, 0).Should().Be(input.Length);
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
            var reader = new StreamReader("Data/0901.txt");
            var input = reader.ReadLine();
            ScoreGroup(input).Should().Be(16689);
            removeCharacters.Should().Be(7982);
        }
    }
}
