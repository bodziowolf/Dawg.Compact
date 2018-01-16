namespace Dawg.Compact
{
    using Dawg.Compact.Build;
    using System.Collections.Generic;
    using System.Text;

    public class NodeDawg : IPrefixMatcher
    {
        private struct TraversalNode
        {
            public TrieNode Node;
            public int Depth;
            public char Letter;
            public bool IsRoot;

            public TraversalNode(TrieNode node, int depth, char letter)
            {
                Node = node;
                Depth = depth;
                Letter = letter;
                IsRoot = false;
            }

            internal static TraversalNode MakeRoot(TrieNode currentNode)
            {
                var root = new TraversalNode();
                root.IsRoot = true;
                root.Depth = -1;
                return root;
            }
        }

        private readonly TrieNode _root;
        private readonly Stack<TraversalNode> _traversalStack;

        public NodeDawg(TrieNode root)
        {
            _root = root;
            _traversalStack = new Stack<TraversalNode>(100);
        }

        public IEnumerable<string> GetWordsByPrefix(string prefix)
        {
            if (!TryGetNodeByPrefix(prefix, out TrieNode currentNode))
            {
                yield break;
            }

            var wordBuilder = new StringBuilder(100);
            if (prefix.Length > 0)
            {
                wordBuilder.Append(prefix, 0, prefix.Length - 1);
            }

            _traversalStack.Clear();
            if (prefix.Length > 0)
            {
                char lastLetter = prefix[prefix.Length - 1];
                _traversalStack.Push(new TraversalNode(currentNode, prefix.Length - 1, lastLetter));
            }
            else
            {
                _traversalStack.Push(TraversalNode.MakeRoot(currentNode));
            }

            while (_traversalStack.Count > 0)
            {
                var traversalNode = _traversalStack.Pop();
                int currentDepth = traversalNode.Depth;

                if (!traversalNode.IsRoot)
                {
                    currentNode = traversalNode.Node;

                    if (wordBuilder.Length > currentDepth)
                    {
                        wordBuilder.Remove(currentDepth, wordBuilder.Length - currentDepth);
                    }

                    wordBuilder.Append(traversalNode.Letter);

                    if (currentNode.IsEOW)
                    {
                        yield return wordBuilder.ToString();
                    }
                }

                LoadChildren(currentDepth, currentNode);
            }
        }

        private void LoadChildren(int currentDepth, TrieNode currentNode)
        {
            foreach (var childKV in currentNode.Edges)
            {
                _traversalStack.Push(new TraversalNode(childKV.Value, currentDepth + 1, childKV.Key));
            }
        }

        private bool TryGetNodeByPrefix(string prefix, out TrieNode currentNode)
        {
            currentNode = _root;
            foreach (char letter in prefix)
            {
                if (!currentNode.Edges.TryGetValue(letter, out currentNode))
                {
                    return false;
                }
            }
            return true;
        }

        public bool HasPrefix(string prefix)
        {
            return TryGetNodeByPrefix(prefix, out TrieNode node);
        }

        public bool HasWord(string prefix)
        {
            if (!TryGetNodeByPrefix(prefix, out TrieNode node))
            {
                return false;
            }

            return node.IsEOW;
        }
    }
}
