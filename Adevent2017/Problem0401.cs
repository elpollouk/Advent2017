using FluentAssertions;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Adevent2017
{
    public class Problem0401
    {
        bool IsValid(string phrase)
        {
            HashSet<string> wordSet = new HashSet<string>();
            var words = phrase.Split(' ');

            foreach (var word in words)
            {
                var lword = word.ToLowerInvariant();
                if (wordSet.Contains(lword)) return false;
                wordSet.Add(lword);
            }
            return true;
        }

        [Theory]
        [InlineData("aa bb cc dd ee", true)]
        [InlineData("aa bb cc dd aa", false)]
        [InlineData("aa bb cc dd aaa", true)]
        public void Example01(string input, bool answer) => IsValid(input).Should().Be(answer);

        [Fact]
        public void Solution01()
        {
            var reader = new StreamReader("data/0401.txt");
            string phrase;
            var count = 0;
            while ((phrase = reader.ReadLine()) != null)
                if (IsValid(phrase))
                    count++;

            count.Should().Be(466);
        }
    }
}
