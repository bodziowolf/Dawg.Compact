namespace Dawg.Compact.Build
{
    using System.Collections.Generic;

    public class TrieNode
    {
        public ulong Id;
        public bool IsEOW;

        public readonly IDictionary<char, TrieNode> Edges;

        private int _edgesCountWhenHashCoded;
        private int _hashCodeCache;

        public TrieNode(ulong id)
        {
            Id = id;
            Edges = new SortedDictionary<char, TrieNode>();
            _edgesCountWhenHashCoded = 0;
        }

        public override int GetHashCode()
        {
            if (_edgesCountWhenHashCoded != Edges.Count)
            {
                unchecked
                {
                    ulong edgesHashCode = 0;
                    foreach (var kv in Edges)
                    {
                        edgesHashCode += kv.Value.Id * 11111 + kv.Key;
                    }
                    _hashCodeCache = (IsEOW ? 1 : 0) * 93018311 + edgesHashCode.GetHashCode();
                    _edgesCountWhenHashCoded = Edges.Count;
                }
            }

            return _hashCodeCache;
        }

        public override bool Equals(object obj)
        {
            TrieNode other = obj as TrieNode;
            if (other == null)
            {
                return false;
            }

            return IsEOW == other.IsEOW && HasSameEdgesWith(other) && other.HasSameEdgesWith(this);
        }

        public bool HasSameEdgesWith(TrieNode other)
        {
            foreach (var kv in Edges)
            {
                if (!other.Edges.TryGetValue(kv.Key, out TrieNode childNode))
                {
                    return false;
                }

                if (childNode.Id != kv.Value.Id)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
