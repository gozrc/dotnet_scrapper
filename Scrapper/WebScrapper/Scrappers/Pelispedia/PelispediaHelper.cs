using System.Collections.Generic;
using Commons.CustomHttpManager;
using WebScrapper.Cryptography;

namespace WebScrapper.Scrappers.Pelispedia
{
    class PelispediaHelper
    {
        public static bool getKeyMovie (string url, string buffer, ref string keyMovie, ref string error)
        {
            keyMovie = buffer.MatchRegex("<iframe src=\"https://www.pelispedia.tv/api/iframes.php\\?id=([^\\?]*)\\?nocache");

            if (keyMovie.Length == 0)
                error = "getKeyMovie -> Key not found";

            return (0 == error.Length);
        }

        public static bool getUrlOptions (string urlMovie, string keyMovie, ref string buffer, ref string error)
        {
            HttpHeaders headers = new HttpHeaders();
            headers.Add (new HttpHeader("Referer", urlMovie));

            string url = string.Format("https://www.pelispedia.tv/api/iframes.php?id={0}?nocache", keyMovie);

            HttpManager.requestGet(url, headers, ref buffer, ref error);

            if (error.Length > 0)
                error = "getUrlOptions -> " + error;

            return (0 == error.Length);
        }

        public static bool getSources (string buffer, ref List<string> sources, ref string error)
        {
            string[] links1 = buffer.MatchRegexs("<a href=\"(https://load.pelispedia.vip/embed[^\"]*)\"");
            string[] links2 = buffer.MatchRegexs("<a href=\"(https://pelispedia.stream[^\"]*)\"");

            foreach (string link in links1)
                sources.Add(link);

            foreach (string link in links2)
                sources.Add(link);

            if (sources.Count == 0)
                error = "getSources -> Source not found";

            return (0 == error.Length);
        }

        public static bool getCode (string buffer, ref string code, ref string error)
        {
            code = buffer.MatchRegex("<meta name=\"google-site-verification\" content=(\"[^\"]*\")>");

            if (code.Length == 0)
                error = "getCode -> Code not found";

            return (0 == error.Length);
        }

        public static bool decryptUrl (string urlBase, string codigo, ref string url, ref string error)
        {
            HelperAES aes = new HelperAES();

            urlBase = urlBase.Replace("embed", "stream");

            url = string.Format(
                "{0}/{1}", 
                urlBase, 
                aes.OpenSSLEncrypt2(
                    codigo, 
                    "4fe554b59d760c9986c903b07af8b7a4yt4fe554b59d760c9986c903b07af8b7a4785446346")
            );

            return true;
        }
    }
}
