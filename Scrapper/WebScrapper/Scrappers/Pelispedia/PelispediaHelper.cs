using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Commons.CustomHttpManager;
using WebScrapper.Cryptography;

namespace WebScrapper.Scrappers.Pelispedia
{
    class PelispediaHelper
    {
        public static bool getKeyMovie (string url, string buffer, ref string keyMovie, ref string error)
        {
            try
            {
                Regex rgx = new Regex("api/iframes.php[?]id=[0-9]+", RegexOptions.IgnoreCase);

                if (!rgx.IsMatch(buffer))
                    throw new Exception("No se encontró el iframe de reproduccion (url = " + url + ")");

                keyMovie = rgx.Match(buffer).Value.Split('=')[1];
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "getKeyMovie -> " + error;

            return (0 == error.Length);
        }

        public static bool getUrlOptions (string urlMovie, string keyMovie, ref string buffer, ref string error)
        {
            HttpHeaders headers = new HttpHeaders();
            headers.Add (new HttpHeader("Referer", urlMovie));

            string url = string.Format("https://www.pelispedia.tv/api/iframes.php?id={0}&update1.1", keyMovie);

            HttpManager.requestGet(url, headers, ref buffer, ref error);

            if (error.Length > 0)
                error = "getUrlOptions -> " + error;

            return (0 == error.Length);
        }

        public static bool getSources (string url, string buffer, ref List<string> sources, ref string error)
        {
            try
            {
                Regex rgxOptions = new Regex(
                    "<a href=\"((https://load.pelispedia.vip/embed)|(https://pelispedia.stream/)).+>",
                    RegexOptions.IgnoreCase
                );

                if (!rgxOptions.IsMatch(buffer))
                    throw new Exception("No se encontraron fuentes para la película (url = " + url + ")");

                foreach (Match optionMatch in rgxOptions.Matches(buffer))
                {
                    string link = optionMatch.Value.Split('\"')[1];

                    if (!link.StartsWith("https:"))
                        link = "https:" + link;

                    sources.Add(link);
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "getSources -> " + error;

            return (0 == error.Length);
        }

        public static bool getCode (string buffer, ref string code, ref string error)
        {
            try
            {
                Regex rgx = new Regex("<meta name=\"google-site-verification.+>", RegexOptions.IgnoreCase);

                if (!rgx.IsMatch(buffer))
                    throw new Exception("No se encontró el codigo de la película.");

                code = "\"" + rgx.Match(buffer).Value.Split('\"')[3] + "\"";
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "getCode -> " + error;

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
