namespace Dawg.Compact.Build
{
    internal class UncheckedNodeInfo
    {
        public readonly TrieNode Node;
        public readonly char Letter;
        public readonly TrieNode Child;

        public UncheckedNodeInfo(TrieNode node, char letter, TrieNode next)
        {
            Node = node;
            Letter = letter;
            Child = next;
        }
    }
}
