//creation based on python code at http://stevehanov.ca/blog/index.php?id=115
//then i minimize the graph structure to the array

namespace Dawg.Compact
{
    using Dawg.Compact.Build;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DawgBuilder
    {
        public ulong WordsCount { get; private set; }

        private ulong _nodeIdCounter;
        private string _previousWord;

        private readonly TrieNode _root;
        private readonly List<UncheckedNodeInfo> _uncheckedNodes;
        private readonly Dictionary<TrieNode, TrieNode> _minimizedNodes;
        private readonly DawgCompacter _compacter;

        public DawgBuilder()
        {
            _nodeIdCounter = 0;
            _previousWord = String.Empty;
            _root = CreateNode();
            _uncheckedNodes = new List<UncheckedNodeInfo>();
            _minimizedNodes = new Dictionary<TrieNode, TrieNode>();
            _compacter = new DawgCompacter(_root);
        }

        public IPrefixMatcher BuildDawg(DawgType dawgType)
        {
            switch (dawgType)
            {
                case DawgType.Node:
                    return BuildNodeDawg();
                case DawgType.Compact:
                    return BuildCompactDawg();
                default:
                    throw new ArgumentException();
            }
        }

        public IPrefixMatcher BuildCompactDawg()
        {
            return new CompactDawg(_compacter.Compact());
        }

        public IPrefixMatcher BuildNodeDawg()
        {
            return new NodeDawg(_root);
        }

        public DawgBuilder WithUnorderedWords(IEnumerable<string> words)
        {
            return WithOrderedWords(words.OrderBy(x => x));
        }

        public DawgBuilder WithOrderedWords(IEnumerable<string> orderedWords)
        {
            foreach (var word in orderedWords)
            {
                Insert(word);
                WordsCount++;
            }
            Minimize(0);
            return this;
        }

        public DawgBuilder WithOrderedWordsFromFile(string fileName)
        {
            using (var source = new WordListFileSource(fileName))
            {
                return WithOrderedWords(source);
            }
        }

        private void Insert(string word)
        {
            if(string.Compare(word, _previousWord) < 0)
            {
                throw new InvalidOperationException($"Unsorted words given: Previous: {_previousWord} Current: {word}");
            }

            int commonPrefix = FindCommonPrefix(word, _previousWord);
            Minimize(commonPrefix);
            InstallSuffix(word, commonPrefix);
            _previousWord = word;
        }

        private void InstallSuffix(string word, int commonPrefix)
        {
            TrieNode current;
            if (_uncheckedNodes.Count == 0)
            {
                current = _root;
            }
            else
            {
                current = _uncheckedNodes.Last().Child;
            }

            string suffix = word.Substring(commonPrefix);
            foreach (var letter in suffix)
            {
                TrieNode next = CreateNode();
                current.Edges[letter] = next;
                _uncheckedNodes.Add(new UncheckedNodeInfo(current, letter, next));
                current = next;
            }

            current.IsEOW = true;
        }

        private int FindCommonPrefix(string word1, string word2)
        {
            int commonPrefix = 0;
            int commonLength = Math.Min(word1.Length, word2.Length);
            for (; commonPrefix < commonLength; commonPrefix++)
            {
                if (word1[commonPrefix] != word2[commonPrefix])
                {
                    break;
                }
            }
            return commonPrefix;
        }

        private void Minimize(int to)
        {
            for (int i = _uncheckedNodes.Count - 1; i > to - 1; --i) 
            {
                var info = _uncheckedNodes[i];
                var child = info.Child;
                if (_minimizedNodes.TryGetValue(child, out TrieNode valueChild))
                {
                    var parent = info.Node;
                    parent.Edges[info.Letter] = valueChild;
                }
                else
                {
                    _minimizedNodes[child] = child;
                }
                _uncheckedNodes.RemoveAt(_uncheckedNodes.Count - 1);
            }
        }

        private TrieNode CreateNode()
        {
            return new TrieNode(_nodeIdCounter++);
        }
    }
}
