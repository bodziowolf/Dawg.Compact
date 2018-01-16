namespace Dawg.Compact
{
    using Node;
    using System.Collections.Generic;
    using System.Text;

    public class CompactDawg : IPrefixMatcher
    {
        private struct IndexAndDepth
        {
            public ulong Index;
            public int Depth;

            public IndexAndDepth(ulong index, int depth)
            {
                Index = index;
                Depth = depth;
            }
        }

        private readonly ulong[] _nodeTable;
        private readonly Stack<IndexAndDepth> _traversalStack;
        private readonly StringBuilder _wordBuilder;

        public CompactDawg(ulong[] nodes)
        {
            _nodeTable = nodes;
            _traversalStack = new Stack<IndexAndDepth>(100);
            _wordBuilder = new StringBuilder(100);
        }

        public bool HasPrefix(string prefix)
        {
            return TryGetNodeIndexByPrefix(prefix, out ulong index);
        }

        public bool HasWord(string prefix)
        {
            if (!TryGetNodeIndexByPrefix(prefix, out ulong index))
            {
                return false;
            }

            ulong node = _nodeTable[index];
            return node.IsEOW();
        }

        public IEnumerable<string> GetWordsByPrefix(string prefix)
        {
            if (!TryGetNodeIndexByPrefix(prefix, out ulong currentIndex))
            {
                yield break;
            }

            InitWordBuilderFromPrefix(prefix);
            InitDFSStack(currentIndex, _wordBuilder.Length);

            while (_traversalStack.Count > 0)
            {
                var currentIndexDepth = _traversalStack.Pop();
                currentIndex = currentIndexDepth.Index;
                ulong currentNode = _nodeTable[currentIndex];

                int currentDepth = currentIndexDepth.Depth;
                if (_wordBuilder.Length > currentDepth)
                {
                    _wordBuilder.Remove(currentDepth, _wordBuilder.Length - currentDepth);
                }

                _wordBuilder.Append(currentNode.GetLetter());

                if (currentNode.IsEOW())
                {
                    yield return _wordBuilder.ToString();
                }

                LoadChildren(currentIndex, currentDepth, currentNode);
            }
        }

        private bool TryGetNodeIndexByPrefix(string prefix, out ulong index)
        {
            bool hasChildren = _nodeTable.Length > 0;
            ulong childIndex = Consts.FirstRootChildIndex;
            index = Consts.RootIndex;
            for (int i = 0; i < prefix.Length; i++)
            {
                if (!hasChildren)
                {
                    return false;
                }

                char letter = prefix[i];
                index = childIndex;

                ulong node = _nodeTable[index];
                bool found = false;
                if (node.GetLetter() == letter)
                {
                    found = true;
                }
                else
                {
                    while (node.HasSibling())
                    {
                        index++;
                        node = _nodeTable[index];
                        if (node.GetLetter() == letter)
                        {
                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    return false;
                }

                hasChildren = node.HasChild();
                childIndex = node.GetChildIndex();
            }

            return true;
        }

        private void InitWordBuilderFromPrefix(string prefix)
        {
            _wordBuilder.Clear();
            if (prefix.Length > 0)
            {
                _wordBuilder.Append(prefix, 0, prefix.Length - 1);
            }
        }

        private void InitDFSStack(ulong currentIndex, int currentDepth)
        {
            _traversalStack.Clear();
            if (currentIndex == Consts.RootIndex)
            {
                LoadRootChildren();
            }
            else
            {
                _traversalStack.Push(new IndexAndDepth(currentIndex, currentDepth));
            }
        }

        private void LoadRootChildren()
        {
            ulong siblingIndex = Consts.FirstRootChildIndex;
            _traversalStack.Push(new IndexAndDepth(siblingIndex, 0));

            while (true)
            {
                ulong siblingNode = _nodeTable[siblingIndex];
                if (!siblingNode.HasSibling())
                {
                    break;
                }

                siblingIndex++;
                _traversalStack.Push(new IndexAndDepth(siblingIndex, 0));
            }
        }

        private void LoadChildren(ulong nodeIndex, int depth, ulong node)
        {
            if (!node.HasChild())
            {
                return;
            }

            ulong firstChildIndex = node.GetChildIndex();
            _traversalStack.Push(new IndexAndDepth(firstChildIndex, depth + 1));

            ulong childIndex = firstChildIndex;
            while (true)
            {
                ulong childNode = _nodeTable[childIndex];
                if (!childNode.HasSibling())
                {
                    break;
                }

                childIndex++;
                _traversalStack.Push(new IndexAndDepth(childIndex, depth + 1));
            }

            return;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _nodeTable.Length; i++)
            {
                var node = _nodeTable[i];
                sb.AppendLine(node.PrintNode());
            }
            return sb.ToString();
        }
    }
}
