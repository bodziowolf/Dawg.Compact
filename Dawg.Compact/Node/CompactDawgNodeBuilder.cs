namespace Dawg.Compact.Node
{
    using System;

    public struct CompactDawgNodeBuilder
    {
        private ulong _node;

        public CompactDawgNodeBuilder(ulong node)
        {
            _node = node;
        }

        public ulong Build()
        {
            return _node;
        }

        public CompactDawgNodeBuilder WithLetter(char letter)
        {
            ulong value = ((ulong)letter) << 48;
            _node |= value;
            return this;
        }

        public CompactDawgNodeBuilder EndsWord()
        {
            _node |= Consts.MaskEOW;
            return this;
        }

        public CompactDawgNodeBuilder WithNextSiblingRight()
        {
            _node |= Consts.MaskHasSibling;
            return this;
        }

        public CompactDawgNodeBuilder WithNextChildAtIndex(ulong index)
        {
            if (index > Consts.MaxIndex)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            ulong value = index & Consts.MaskChildIndex;
            _node |= value;
            _node |= Consts.MaskHasChild;
            return this;
        }

        public override string ToString()
        {
            return $"[LETTER:{_node.GetLetter()}][EOW:{_node.IsEOW()}][SIB:{_node.HasSibling()}][CHILD:{_node.HasChild()}][IDX:{_node.GetChildIndex()}]";
        }
    }
}
