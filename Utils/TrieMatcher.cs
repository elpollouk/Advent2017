using System.Collections.Generic;

namespace Utils
{
    public class TrieMatcher<TMatchResult>
    {
        public delegate TMatchResult MatchFunc(string text, int index = 0);

        private class Node
        {
            public readonly Dictionary<char, Node> Children = [];
            public TMatchResult MatchValue;
        }

        private readonly Node root = new();
        private readonly TMatchResult notFoundResult;

        public TrieMatcher(TMatchResult notFoundResult = default)
        {
            root.MatchValue = notFoundResult;
            this.notFoundResult = notFoundResult;
        }

        public TrieMatcher<TMatchResult> AddSequence(string sequence, TMatchResult matchResult)
        {
            var node = root;

            foreach (var c in sequence)
            {
                node = node.Children.GetOrCreate(c, () => new() { MatchValue = notFoundResult });
            }

            node.MatchValue = matchResult;
            return this;
        }

        public TMatchResult Match(string text, int index = 0)
        {
            var node = root;

            while (index < text.Length)
            {
                var nextNode = node.Children.GetOrDefault(text[index]);
                if (nextNode == null) break;
                node = nextNode;
                index++;
            }

            return node.MatchValue;
        }

        public MatchFunc Build()
        {
            return Match;
        }
    }
}
