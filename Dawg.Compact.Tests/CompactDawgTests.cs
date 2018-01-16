namespace Dawg.Compact.Tests
{
    using System.Linq;
    using Xunit;
    using B = Node.CompactDawgNodeBuilder;

    public class CompactDawgTests
    {
        private readonly ulong[] _nodes = new ulong[]
        {
            /*0*/ new B().WithLetter('a').WithNextSiblingRight().EndsWord().Build(),
            /*1*/ new B().WithLetter('l').WithNextChildAtIndex(5).WithNextSiblingRight().Build(),
            /*2*/ new B().WithLetter('m').WithNextChildAtIndex(3).Build(),
            /*3*/ new B().WithLetter('a').WithNextChildAtIndex(4).Build(),
            /*4*/ new B().WithLetter('ć').EndsWord().Build(),
            /*5*/ new B().WithLetter('a').WithNextChildAtIndex(6).EndsWord().Build(),
            /*6*/ new B().WithLetter('ć').WithNextSiblingRight().EndsWord().Build(),
            /*7*/ new B().WithLetter('m').WithNextChildAtIndex(8).Build(),
            /*8*/ new B().WithLetter('a').EndsWord().Build()
        };

        protected IPrefixMatcher CreateSUT()
        {
            return new CompactDawg(_nodes);
        }

        [Fact]
        public void GetWordsByPrefix_EmptyPrefixGiven_ShouldReturnAllWords()
        {
            var sut = CreateSUT();

            var result = sut.GetWordsByPrefix("");

            Assert.Equal(new[] { "a", "la", "lać", "lama", "mać" }, result.OrderBy(x => x).ToArray());
        }

        [Fact]
        public void GetWordsByPrefix_PrefixGiven_ShouldReturnAllWordsBeginningWithThisPrefix()
        {
            var sut = CreateSUT();

            var result = sut.GetWordsByPrefix("la");

            Assert.Equal(new[] { "la", "lać", "lama" }, result.OrderBy(x => x).ToArray());
        }

        [Fact]
        public void GetWordsByPrefix_LetterAPrefixGiven_ShouldReturnAllWordsBeginningWithThisPrefix()
        {
            var sut = CreateSUT();

            var result = sut.GetWordsByPrefix("a");

            Assert.Equal(new[] { "a" }, result.OrderBy(x => x).ToArray());
        }

        [Fact]
        public void GetWordsByPrefix_SubsequentCalls_ShouldAlwaysReturnSameResults()
        {
            var sut = CreateSUT();

            Assert.Equal(new[] { "mać" }, sut.GetWordsByPrefix("m"));
            Assert.Equal(new[] { "mać" }, sut.GetWordsByPrefix("m"));
        }

        [Theory]
        [InlineData("m")]
        [InlineData("mać")]
        [InlineData("lam")]
        [InlineData("lama")]
        public void HasPrefix_ExistingPrefixGiven_ShouldReturnTrue(string prefix)
        {
            var sut = CreateSUT();

            var result = sut.HasPrefix(prefix);

            Assert.True(result);
        }

        [Fact]
        public void HasPrefix_EmptyPrefixGiven_ShouldReturnTrue()
        {
            var sut = CreateSUT();

            var result = sut.HasPrefix("");

            Assert.True(result);
        }

        [Fact]
        public void HasPrefix_NonExistingPrefixGiven_ShouldReturnFalse()
        {
            var sut = CreateSUT();

            var result = sut.HasPrefix("x");

            Assert.False(result);
        }

        [Theory]
        [InlineData("mać")]
        [InlineData("la")]
        [InlineData("lama")]
        public void HasWord_ExistingWordGiven_ShouldReturnTrue(string word)
        {
            var sut = CreateSUT();

            var result = sut.HasWord(word);

            Assert.True(result);
        }

        [Theory]
        [InlineData("ma")]
        [InlineData("l")]
        [InlineData("lam")]
        public void HasWord_ExistingPrefixWhichIsNotAWordGiven_ShouldReturnFalse(string prefix)
        {
            var sut = CreateSUT();

            var result = sut.HasWord(prefix);

            Assert.False(result);
        }

        [Fact]
        public void HasPrefix_NonExistingWordGiven_ShouldReturnFalse()
        {
            var sut = CreateSUT();

            var result = sut.HasWord("x");

            Assert.False(result);
        }
    }
}
