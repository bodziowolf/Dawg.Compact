namespace Dawg.Compact.Benchmark
{
    using System.Collections.Generic;
    using System.Linq;
    using DawgSharp;

    public class DawgSharpBenchmark : Benchmark
    {
        private class PrefixMatcher : IPrefixMatcher
        {
            private readonly Dawg<bool> _dawg;

            public PrefixMatcher(Dawg<bool> dawg)
            {
                _dawg = dawg;
            }

            public IEnumerable<string> GetWordsByPrefix(string prefix)
            {
                return _dawg.MatchPrefix(prefix).Select(x => x.Key);
            }

            public bool HasWord(string prefix)
            {
                return _dawg[prefix];
            }

            public bool HasPrefix(string prefix)
            {
                throw new System.NotImplementedException();
            }
        }

        public override string Name => "DawgSharp";

        protected override IPrefixMatcher Build(string dictionaryFile)
        {
            using (var dictionarySource = new WordDictionary(dictionaryFile))
            {
                var dawgBuilder = new DawgBuilder<bool>();
                foreach (var word in dictionarySource)
                {
                    dawgBuilder.Insert(word, true);
                }

               return new PrefixMatcher(dawgBuilder.BuildDawg());
            }
        }
    }
}
