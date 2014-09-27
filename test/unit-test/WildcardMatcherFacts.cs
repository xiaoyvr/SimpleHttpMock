using System.Text.RegularExpressions;
using SimpleHttpMock;
using Xunit;
using Xunit.Extensions;

namespace test
{
    public class WildcardMatcherFacts
    {
        [Theory]
        [InlineData("pass", true)]
        [InlineData("Pass", false)]
        [InlineData("pas", false)]
        [InlineData("passw", true)]
        [InlineData("password", true)]
        public void should_match_star_wild_card(string s, bool expectedResult)
        {
            var wildCardMatcher = new WildCardMatcher("pass*");
            Assert.Equal(expectedResult, wildCardMatcher.Match(s));
        }

        [Fact]
        public void should_ignore_case_when_set_ignore_case()
        {
            var wildCardMatcher = new WildCardMatcher("pass*", RegexOptions.IgnoreCase);
            Assert.True(wildCardMatcher.Match("Pass"));
        }

        [Theory]
        [InlineData("pass", false)]
        [InlineData("pas", false)]
        [InlineData("passw", true)]
        [InlineData("password", false)]
        public void should_match_quesiton_mark_as_single_character(string s, bool expectedResult)
        {
            var wildCardMatcher = new WildCardMatcher("pass?");
            Assert.Equal(expectedResult, wildCardMatcher.Match(s));
        }
    }
}