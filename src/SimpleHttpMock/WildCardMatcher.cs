using System.Text.RegularExpressions;

namespace SimpleHttpMock
{
    public class WildCardMatcher
    {
        private readonly string pattern;
        private readonly RegexOptions regexOptions;

        public WildCardMatcher(string pattern, RegexOptions regexOptions = RegexOptions.None)
        {
            this.pattern = pattern;
            this.regexOptions = regexOptions;
        }

        public bool Match(string s)
        {
            var regexPattern = string.Format("^{0}$", Regex.Escape(pattern))
                                     .Replace("\\*", ".*")
                                     .Replace("\\?",".");
            return new Regex(regexPattern, regexOptions).Match(s).Success;
        }
    }
}