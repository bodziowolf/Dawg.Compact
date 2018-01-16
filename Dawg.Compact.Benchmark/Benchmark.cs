namespace Dawg.Compact.Benchmark
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public abstract class Benchmark
    {
        public const int TestRuns = 10;

        public abstract string Name { get; }
        public TimeSpan DictionaryBuildTime { get; private set; }
        public TimeSpan? PrefixCompletionSuggestionsTestTime { get; private set; }
        public TimeSpan? WordExistenceTestTime { get; private set; }
        public TimeSpan? PrefixExistenceTestTime { get; private set; }

        protected abstract IPrefixMatcher Build(string dictionaryFile);
        
        public void Run(IList<string> testData, string dictionaryFile)
        {
            Console.WriteLine("Preparing dictionary...");
            var watch = new Stopwatch();
            watch.Start();
            var sut = Build(dictionaryFile);
            watch.Stop();
            DictionaryBuildTime = watch.Elapsed;
            Console.WriteLine($"Preparing dictionary... done in: {DictionaryBuildTime}");

            Console.WriteLine($"Testing {testData.Count} prefix completion suggestions {TestRuns} times...");
            PrefixCompletionSuggestionsTestTime = Test(watch, sut, testData, TestPrefixCompletionSuggestions);
            Console.WriteLine($"done in: {PrefixCompletionSuggestionsTestTime} (averaged by test runs)");

            Console.WriteLine($"Testing {testData.Count} word existence {TestRuns} times...");
            WordExistenceTestTime = Test(watch, sut, testData, TestWordExistence);
            Console.WriteLine($"done in: {WordExistenceTestTime} (averaged by test runs)");

            Console.WriteLine($"Testing {testData.Count} prefix existence {TestRuns} times...");
            PrefixExistenceTestTime = Test(watch, sut, testData, TestPrefixExistence);
            Console.WriteLine($"done in: {WordExistenceTestTime} (averaged by test runs)");
        }

        private TimeSpan? Test(Stopwatch watch, IPrefixMatcher sut, IList<string> testData, Action<IPrefixMatcher, IList<string>> test)
        {
            try
            {
                watch = new Stopwatch();
                watch.Start();
                for (int i = 0; i < TestRuns; i++)
                {
                    test(sut, testData);
                }
                watch.Stop();
                return watch.Elapsed / TestRuns;
            }
            catch(NotImplementedException)
            {
                return null;
            }
        }

        private void TestPrefixCompletionSuggestions(IPrefixMatcher sut, IList<string> testData)
        {
            for (int i = 0; i < testData.Count; i++)
            {
                IList<string> prefixes = sut.GetWordsByPrefix(testData[i]).Take(10).ToList(); 
                //to list ensures we actually use the generator if there is a generator implementation underneath
                //take limits number of responses like somebody would do on a drop down list
            }
        }

        private void TestWordExistence(IPrefixMatcher sut, IList<string> testData)
        {
            for (int i = 0; i < testData.Count; i++)
            {
                sut.HasWord(testData[i]);
            }
        }

        private void TestPrefixExistence(IPrefixMatcher sut, IList<string> testData)
        {
            for (int i = 0; i < testData.Count; i++)
            {
                sut.HasPrefix(testData[i]);
            }
        }
    }
}
