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
                pathAndQuery =>
                {
                    var lowerStr = urlPattern.ToLower();
                    return pathAndQuery.ToLower() == (
                                        Uri.IsWellFormedUriString(lowerStr, UriKind.Absolute)
                                            ? new Uri(lowerStr).PathAndQuery
                                            : lowerStr
                                        );
                };
        }

        public static Func<string, bool> Wildcard(string wildCardPattern)
        {
            return s => new WildCardMatcher(wildCardPattern, RegexOptions.IgnoreCase).Match(s);
        }
    }
}