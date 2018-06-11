using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace WebScrapper
{
    public static class Extensions
    {
        public static string MatchRegex (this string str, string regex)
        {
            return str.MatchRegex(regex, false, false);
        }

        public static string [] MatchRegexs (this string str, string regex)
        {
            return str.MatchRegexs(regex, false, false);
        }

        public static string MatchRegex (this string str, string regex, bool ignoreCase, bool singleLine)
        {
            RegexOptions rgxOptions = RegexOptions.None;

            if (ignoreCase)
                rgxOptions = rgxOptions | RegexOptions.IgnoreCase;

            if (singleLine)
                rgxOptions = rgxOptions | RegexOptions.Singleline;

            Regex rgx = new Regex(regex, rgxOptions);

            if (!rgx.IsMatch(str))
                return string.Empty;

            Match match = rgx.Match(str);

            return match.Groups[match.Groups.Count - 1].Value;
        }

        public static string[] MatchRegexs (this string str, string regex, bool ignoreCase, bool singleLine)
        {
            RegexOptions rgxOptions = RegexOptions.None;

            if (ignoreCase)
                rgxOptions = rgxOptions | RegexOptions.IgnoreCase;

            if (singleLine)
                rgxOptions = rgxOptions | RegexOptions.Singleline;

            Regex rgx = new Regex(regex, rgxOptions);

            List<string> rst = new List<string>();

            foreach (Match match in rgx.Matches(str))
                rst.Add(match.Groups[match.Groups.Count - 1].Value);

            return rst.ToArray();
        }
    }
}
