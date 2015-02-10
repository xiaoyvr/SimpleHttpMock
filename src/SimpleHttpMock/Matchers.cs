using System;
using System.Text.RegularExpressions;

namespace SimpleHttpMock
{
    public class Matchers
    {
        public static Func<string, bool> Regex(string regexPattern)
        {
            return url =>
                {
                    var match = System.Text.RegularExpressions.Regex.Match(url, regexPattern, RegexOptions.IgnoreCase);
                    return match.Success;
                };
        }

        public static Func<string, bool> Is(string urlPattern)
        {
            return
                pathAndQuery => pathAndQuery == (
                    Uri.IsWellFormedUriString(urlPattern, UriKind.Absolute)
                        ? new Uri(urlPattern).PathAndQuery
                        : urlPattern
                    );
        }

        public static Func<string, bool> Wildcard(string wildCardPattern)
        {
            return s => new WildCardMatcher(wildCardPattern, RegexOptions.IgnoreCase).Match(s);
        }
    }
}