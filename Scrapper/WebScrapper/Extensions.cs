using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace WebScrapper
{
    public static class Extensions
    {
        public static string MatchRegex (this string str, string regex)
        {
            Regex rgx = new Regex(regex, RegexOptions.IgnoreCase);

            if (!rgx.IsMatch(str))
                return string.Empty;

            Match match = rgx.Match(str);

            return match.Groups[match.Groups.Count - 1].Value;
        }

        public static string [] MatchRegexs (this string str, string regex)
        {
            Regex rgx = new Regex(regex, RegexOptions.IgnoreCase);

            List<string> rst = new List<string>();

            foreach (Match match in rgx.Matches(str))
                rst.Add(match.Groups[match.Groups.Count - 1].Value);

            return rst.ToArray();
        }
    }
}
