namespace Dawg.Compact.Node
{
    /// <summary>
    /// [16 - letter][1 - eow][1 - has sibling][1 - has child][45 - child index]
    /// </summary>
    public static class CompactDawgNodeExtensions
    {
        public static char GetLetter(this ulong encoded)
        {
            return (char)(encoded >> 48);
        }

        public static bool IsEOW(this ulong encoded)
        {
            return (encoded & Consts.MaskEOW) > 0;
        }

        public static bool HasSibling(this ulong encoded)
        {
            return (encoded & Consts.MaskHasSibling) > 0;
        }

        public static bool HasChild(this ulong encoded)
        {
            return (encoded & Consts.MaskHasChild) > 0;
        }

        public static ulong GetChildIndex(this ulong encoded)
        {
            return encoded & Consts.MaskChildIndex;
        }

        public static string PrintNode(this ulong node)
        {
            var child = node.HasChild() ? $"At:{node.GetChildIndex()}" : "false";
            return $"[{node.GetLetter()}][Sibling:{node.HasSibling()}][Child:{child}][EOW:{node.IsEOW()}]";
        }
    }
}
