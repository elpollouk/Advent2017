using Adevent2017.Utils;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Adevent2017
{
    public class Problem0401
    {
        bool IsValid(string phrase)
        {
            HashSet<string> wordSet = new HashSet<string>();
            return phrase.ForEachLowerWord(word =>
            {
                if (wordSet.Contains(word)) StopIteration.Now();
                wordSet.Add(word);
            });
        }

        int[] CreateSignature(string word)
        {
            var signature = new int[26];
            for (var i = 0; i < word.Length; i++)
                signature[word[i] - 'a']++;

            return signature;
        }

        bool HasSignature(List<int[]> signatures, int[] signature)
        {
            foreach (var s in signatures)
            {
                for (var i = 0; i < 26; i++)
                {
                    if (s[i] != signature[i])
                        goto nextsig;
                }
                return true;
            nextsig:;
            }

            return false;
        }

        bool IsValid2(string phrase)
        {
            var signatureList = new List<int[]>();

            return phrase.ForEachLowerWord(word =>
            {
                var signature = CreateSignature(word);
                if (HasSignature(signatureList, signature))
                    StopIteration.Now();

                signatureList.Add(signature);
            });
        }

        [Theory]
        [InlineData("aa bb cc dd ee", true)]
        [InlineData("aa bb cc dd aa", false)]
        [InlineData("aa bb cc dd aaa", true)]
        public void Example01(string input, bool answer) => IsValid(input).Should().Be(answer);

        [Fact]
        public void Solution01()
        {
            var count = 0;
            FileIterator.EachLine("data/0401.txt", line =>
            {
                if (IsValid(line))
                    count++;
            });

            count.Should().Be(466);
        }

        [Theory]
        [InlineData("abcde fghij", true)]
        [InlineData("abcde xyz ecdab", false)]
        [InlineData("a ab abc abd abf abj", true)]
        [InlineData("iii oiii ooii oooi oooo", true)]
        [InlineData("oiii ioii iioi iiio", false)]
        public void Example02(string input, bool answer) => IsValid2(input).Should().Be(answer);

        [Fact]
        public void Solution02()
        {
            var count = 0;
            FileIterator.EachLine("data/0401.txt", line =>
            {
                if (IsValid2(line))
                    count++;
            });

            count.Should().Be(251);

        }
    }
}
