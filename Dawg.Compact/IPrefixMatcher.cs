namespace Dawg.Compact
{
    using System.Collections.Generic;

    public interface IPrefixMatcher
    {
        bool HasPrefix(string prefix);
        bool HasWord(string prefix);
        IEnumerable<string> GetWordsByPrefix(string prefix);
    }
}