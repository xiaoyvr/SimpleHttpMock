using System;
using System.Text.RegularExpressions;

namespace SimpleHttpMock
{
    public class It
    {
        public static Func<string, bool> IsRegex(string regexPattern)
        {
            return url =>
                {
                    var match = Regex.Match(url, regexPattern, RegexOptions.IgnoreCase);
                    return match.Success;
                };
        }

        public static Func<string, bool> Is(string urlAndQuery)
        {
            return url => url == urlAndQuery;
        }

        public static Func<string, bool> IsWildcard(string wildCardPattern)
        {
            return s => new WildCardMatcher(wildCardPattern, RegexOptions.IgnoreCase).Match(s);
        }
    }
}