namespace Dawg.Compact.Benchmark
{
    public class DawgCompactBenchmark : Benchmark
    {
        public override string Name => "Dawg.Compact";

        protected override IPrefixMatcher Build(string dictionaryFile)
        {
            using (var dictionarySource = new WordDictionary(dictionaryFile))
            {
                return new DawgBuilder()
                    .WithOrderedWords(dictionarySource)
                    .BuildCompactDawg();
            }
        }
    }
}
