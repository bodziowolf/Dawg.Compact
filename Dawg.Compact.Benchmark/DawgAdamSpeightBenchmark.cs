namespace Dawg.Compact.Benchmark
{
    using System.Collections.Generic;
    using DAWG;

    public class DawgAdamSpeightBenchmark : Benchmark
    {
        private class PrefixMatcher : IPrefixMatcher
        {
            private readonly DAWGList _dawg;

            public PrefixMatcher(DAWGList dawg)
            {
                _dawg = dawg;
            }

            public IEnumerable<string> GetWordsByPrefix(string prefix)
            {
                throw new System.NotImplementedException();
            }

            public bool HasWord(string prefix)
            {
                return _dawg.Find(prefix);
            }

            public bool HasPrefix(string prefix)
            {
                throw new System.NotImplementedException();
            }
        }

        public override string Name => "DAWG (AdamSpeight2008)";

        protected override IPrefixMatcher Build(string dictionaryFile)
        {
            using (var dictionarySource = new WordDictionary(dictionaryFile))
            {
                var dawg = new DAWGList();
                foreach (var word in dictionarySource)
                {
                    dawg.Add(word);
                }

               return new PrefixMatcher(dawg);
            }
        }
    }
}
