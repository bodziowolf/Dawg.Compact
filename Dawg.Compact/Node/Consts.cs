namespace Dawg.Compact.Node
{
    public class Consts
    {
        public const ulong MaskLetter = 0b1111_1111_1111_1111_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000;
        public const ulong MaskEOW = 0b0000_0000_0000_0000_1000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000;
        public const ulong MaskHasSibling = 0b0000_0000_0000_0000_0100_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000;
        public const ulong MaskHasChild = 0b0000_0000_0000_0000_0010_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000;
        public const ulong MaskChildIndex = 0b0000_0000_0000_0000_0001_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111;

        public const ulong MaxIndex = ((ulong)1 << 46) - 1;

        public const ulong RootIndex = ulong.MaxValue;
        public const ulong FirstRootChildIndex = 0;
    }
}
