using System;
using System.Text.RegularExpressions;
using Commons.CustomHttpManager;

namespace WebScrapper.Servers
{
    public class ServerScrapperStreamango : IServerScrapper
    {
        public override string name ()
        {
            return "STREAMANGO";
        }

        public override bool scrappear (string url, ref Sources serverLinks, ref string error)
        {
            string buffer   = string.Empty;
            string urlVideo = string.Empty;
            string urlSubs  = string.Empty;

            if (0 == error.Length)
                HttpManager.requestGet(url, null, ref buffer, ref error);

            if (0 == error.Length)
                getUrlSubs(buffer, ref urlSubs, ref error);

            if (0 == error.Length)
                getUrlVideo(buffer, ref urlVideo, ref error);

            if (0 == error.Length)
                serverLinks.Add(new Source(name(), urlVideo, urlSubs, "Default"));

            return (0 == error.Length);
        }


        bool getUrlSubs (string buffer, ref string urlSubs, ref string error)
        {
            urlSubs = buffer.MatchRegex("<track kind=\"captions\" src=\"(.+)\" srclang");

            if (urlSubs.Length == 0)
                error = "Subtitles link not found";

            return (0 == error.Length);
        }

        bool getUrlVideo (string buffer, ref string urlVideo, ref string error)
        {
            try
            {
                string token_str = buffer.MatchRegex(",src:d[(]'(.+)',");
                string token_nbr = buffer.MatchRegex(",src:d[(]'.+',(.+)[)],");

                if (token_nbr.Length > 0 && token_str.Length > 0)
                {
                    urlVideo = "https:" + streamangoMagic(token_str, int.Parse(token_nbr));

                    HttpHeaders responseHeaders = new HttpHeaders();

                    if (HttpManager.requestGetSR(urlVideo, null, ref buffer, ref responseHeaders, ref error))
                        urlVideo = responseHeaders.value("Location");
                }
            }
            catch
            {
                //
            }

            if (urlVideo.Length == 0)
                error = "Link video not found";

            return (0 == error.Length);
        }

        string streamangoMagic (string cadena, int parametro)
        {
            string k = "=/+9876543210zyxwvutsrqponmlkjihgfedcbaZYXWVUTSRQPONMLKJIHGFEDCBA";
            string resultado = "";

            int auxA, auxB, auxC;
            int varA, varB, varC, varD;

            int index = 0;


            while (index < cadena.Length)
            {
                varA = k.IndexOf(cadena.Substring(index++, 1));
                varB = k.IndexOf(cadena.Substring(index++, 1));
                varC = k.IndexOf(cadena.Substring(index++, 1));
                varD = k.IndexOf(cadena.Substring(index++, 1));

                auxA = (varA << 2) | (varB >> 4);
                auxB = ((varB & 0xf) << 4) | (varC >> 2);
                auxC = ((varC & 0x3) << 0x6) | varD;

                auxA = auxA ^ parametro;

                resultado = resultado + new string(new Char[] { (char)auxA });

                if (varC != 0x40)
                    resultado = resultado + new string(new Char[] { (char)auxB });

                if (varD != 0x40)
                    resultado = resultado + new string(new Char[] { (char)auxC });
            }

            return resultado;
        }
    }
}
