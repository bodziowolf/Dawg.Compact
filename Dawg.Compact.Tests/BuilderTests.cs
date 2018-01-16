using Dawg.Compact.Build;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Sdk;

namespace Dawg.Compact.Tests
{
    public class BuilderTests
    {
        [Theory]
        [InlineData(DawgType.Compact)]
        [InlineData(DawgType.Node)]
        public void Build_HandleNonTreeWords_CreateDawgFromOrderedWords(DawgType dawgType)
        {
            var orderedWords = new[] { "hip", "hop" };

            var result = new DawgBuilder().WithOrderedWords(orderedWords).BuildDawg(dawgType);

            AssertHaveProvidedWords(result, orderedWords);
        }

        [Theory]
        [InlineData(DawgType.Compact)]
        [InlineData(DawgType.Node)]
        public void Build_OrderedWords_CreatesDawgFromOrderedWords(DawgType dawgType)
        {
            var orderedWords = new[] { "b¹k", "chrz¹szcz", "hy¿o" };

            var result = new DawgBuilder().WithOrderedWords(orderedWords).BuildDawg(dawgType);

            AssertHaveProvidedWords(result, orderedWords);
        }

        [Theory]
        [InlineData(DawgType.Compact)]
        [InlineData(DawgType.Node)]
        public void Build_UnorderedWords_CreatesDawgFromUnorderedWords(DawgType dawgType)
        {
            var words = new[] { "¿ywo", "hip", "s³abo", "hy¿o", "kaper" };

            var result = new DawgBuilder().WithUnorderedWords(words).BuildDawg(dawgType);

            AssertHaveProvidedWords(result, words);
        }

        [Theory]
        [InlineData(DawgType.Compact)]
        [InlineData(DawgType.Node)]
        public void Build_OrderedWordsFile_CreatesDawgFromOrderedWords(DawgType dawgType)
        {
            string[] subset;
            using (var source = new WordListFileSource("scrabble-polish-words.txt"))
            {
                subset = source.Take(1000).ToArray();
                File.WriteAllLines("tmp-words-subset.txt", subset);
            }

            var result = new DawgBuilder().WithOrderedWordsFromFile("tmp-words-subset.txt").BuildDawg(dawgType);

            AssertHaveProvidedWords(result, subset);
        }

        [Fact]
        public void WordCount_WithUnorderedWords_ShouldProvideWordCount()
        {
            var words = new[] { "hip", "hup", "hop" };

            var result = new DawgBuilder().WithUnorderedWords(words).WordsCount;

            Assert.Equal((ulong)3, result);
        }

        [Fact]
        public void WordCount_WithOrderedWords_ShouldProvideWordCount()
        {
            var words = new[] { "hop", "hup" };

            var result = new DawgBuilder().WithOrderedWords(words).WordsCount;

            Assert.Equal((ulong)2, result);
        }

        private void AssertHaveProvidedWords(IPrefixMatcher tester, IEnumerable<string> words)
        {
            var orderedWords = words.OrderBy(x => x).ToArray();
            var testerWords = tester.GetWordsByPrefix("").OrderBy(x => x).ToArray();
            try
            {
                Assert.Equal(orderedWords, testerWords);
            }
            catch (EqualException)
            {
                var onlyInTester = string.Join(", ", testerWords.Except(orderedWords));
                var onlyInExpected = string.Join(", ", orderedWords.Except(testerWords));
                var duplicates = string.Join(", ", testerWords.GroupBy(x => x).Select(x => new { count = x.Count(), word = x.Key }).Where(x => x.count > 1));
                Assert.True(false, $"Collections differ:\nOnly in tester: {onlyInTester}\nOnly in expected: {onlyInExpected}\nDuplicates: {duplicates}");
            }

            foreach (var word in orderedWords)
            {
                Assert.True(tester.HasPrefix(word), $"Prefix: {word} should exist in Dawg, but its not");
                Assert.True(tester.HasWord(word), $"Word: {word} should exist in Dawg, but its not");
            }
        }
    }
}
