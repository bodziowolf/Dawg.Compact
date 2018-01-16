namespace Dawg.Compact.Build
{
    using Dawg.Compact.Node;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class DawgCompacter
    {
        private readonly List<ulong> _compacted;
        private readonly Queue<TrieNode> _bfsQueue;
        private readonly HashSet<ulong> _bfsQueueUniqueChecker;
        private readonly Dictionary<ulong, List<int>> _nodeIdToCompactedIndex;
        private readonly TrieNode _graphRoot;
        private int _compactedTableIndex;

        public DawgCompacter(TrieNode root)
        {
            _graphRoot = root;
            _compacted = new List<ulong>();
            _bfsQueue = new Queue<TrieNode>();
            _bfsQueueUniqueChecker = new HashSet<ulong>();
            _nodeIdToCompactedIndex = new Dictionary<ulong, List<int>>();
            _compactedTableIndex = 0;
        }

        public ulong[] Compact()
        {
            _bfsQueue.Enqueue(_graphRoot);

            while (_bfsQueue.Count > 0)
            {
                var parentNode = DequeueUnique();

                var edgeKVArray = parentNode.Edges.Reverse().ToArray();
                for (int edgeIndex = 0; edgeIndex < edgeKVArray.Length; edgeIndex++)
                {
                    var edgeKV = edgeKVArray[edgeIndex];
                    var childNode = edgeKV.Value;
                    var childLetter = edgeKV.Key;

                    bool hasSibling = edgeIndex < edgeKVArray.Length - 1;
                    var entry = BuildCompactedEntry(childLetter, childNode.IsEOW, hasSibling);
                    
                    _compacted.Add(entry);

                    SaveNodeIdCompactedTableIndex(childNode.Id, _compactedTableIndex);

                    if (edgeIndex == 0)
                    {
                        SetFirstChildIndexForParents(parentNode.Id);
                    }

                    EnqueueUniqueHavingEdges(childNode);

                    _compactedTableIndex++;
                }
            }

            return _compacted.ToArray();
        }

        private void EnqueueUniqueHavingEdges(TrieNode node)
        {
            if (node.Edges.Count > 0 && _bfsQueueUniqueChecker.Add(node.Id))
            {
                _bfsQueue.Enqueue(node);
            }
        }

        private TrieNode DequeueUnique()
        {
            var node =_bfsQueue.Dequeue();
            _bfsQueueUniqueChecker.Remove(node.Id);
            return node;
        }

        private void SaveNodeIdCompactedTableIndex(ulong nodeId, int savedAtIndex)
        {
            if (!_nodeIdToCompactedIndex.TryGetValue(nodeId, out List<int> parents))
            {
                parents = new List<int>() { savedAtIndex };
                _nodeIdToCompactedIndex[nodeId] = parents;
            }
            else
            {
                parents.Add(savedAtIndex);
            }
        }

        private void SetFirstChildIndexForParents(ulong parentNodeId)
        {
            if (!_nodeIdToCompactedIndex.TryGetValue(parentNodeId, out List<int> parentCompactedIndices))
            {
                return;
            }

            foreach (var parentIndex in parentCompactedIndices)
            {
                var parentEntry = _compacted[parentIndex];
                var editedEntry = InjectChildIndexToCompactedEntry(parentEntry, _compactedTableIndex);
                _compacted[parentIndex] = editedEntry;
                _nodeIdToCompactedIndex.Remove(parentNodeId); 
            }           
        }

        private ulong InjectChildIndexToCompactedEntry(ulong entry, int childIndex)
        {
            return new CompactDawgNodeBuilder(entry)
                .WithNextChildAtIndex((ulong)childIndex)
                .Build();
        }

        private ulong BuildCompactedEntry(char letter, bool endsWord, bool hasSibling)
        {
            var entryBuilder = new CompactDawgNodeBuilder()
                .WithLetter(letter);

            if (endsWord)
            {
                entryBuilder.EndsWord();
            }

            if (hasSibling)
            {
                entryBuilder.WithNextSiblingRight();
            }

            return entryBuilder.Build();
        }
    }
}
