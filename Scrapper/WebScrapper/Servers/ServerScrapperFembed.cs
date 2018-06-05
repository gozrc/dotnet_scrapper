using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Commons.CustomHttpManager;

namespace WebScrapper.Servers
{
    public class ServerScrapperFembed : IServerScrapper
    {
        public override string name ()
        {
            return "FEMBED";
        }

        public override bool scrappear (string url, ref Sources serverLinks, ref string error)
        {
            string buffer    = string.Empty;
            string codigo    = string.Empty;
            string urlAux    = string.Empty;
            string urlSubs   = string.Empty;
            string urlThumb  = string.Empty;

            List<string> urlVideos = new List<string>();

            if (0 == error.Length)
                HttpManager.requestGet(url, null, ref buffer, ref error);

            if (0 == error.Length)
                obtenerUrlThumb(buffer, ref urlThumb, ref error);

            if (0 == error.Length)
                HttpManager.requestPost(url.Replace("/v/", "/api/source/"), null, string.Empty, ref buffer, ref error);

            if (0 == error.Length)
                obtenerUrlSubs(buffer, ref urlSubs, ref error);

            if (0 == error.Length)
                obtenerUrlVideos(buffer, ref urlVideos, ref error);

            if (0 == error.Length)
            {
                foreach (string urlVideo in urlVideos)
                {
                    if (base.esArchivoValido(urlVideo))
                        serverLinks.Add(new Source(urlVideo, urlSubs, getDescription(urlVideo), name(), urlThumb));
                }
            }

            return (0 == error.Length);
        }


        bool obtenerUrlThumb (string buffer, ref string urlThumb, ref string error)
        {
            try
            {
                buffer = buffer.Replace("\\", "");
                Regex rgx = new Regex("https://asset.fembed.com/thumbnail/[^\"]*", RegexOptions.IgnoreCase);
                
                // usar Groups!!!!

                if (!rgx.IsMatch(buffer))
                    throw new Exception("No se encontró el link del thumb");

                urlThumb = rgx.Match(buffer).Value;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "obtenerUrlThumb -> " + error;

            return (0 == error.Length);
        }

        bool obtenerUrlVideos (string buffer, ref List<string> urlVideos, ref string error)
        {
            try
            {
                Regex rgx = new Regex("https://[^\"]*", RegexOptions.IgnoreCase);

                buffer = buffer.Replace("\\", "");

                if (!rgx.IsMatch(buffer))
                    throw new Exception("No se encontró el link del video (url = )");

                foreach (Match m in rgx.Matches(buffer))
                    urlVideos.Add(m.Value);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "obtenerUrlVideos -> " + error;

            return (0 == error.Length);
        }

        bool obtenerUrlSubs (string buffer, ref string urlSubs, ref string error)
        {
            try
            {
                Regex rgx = new Regex("/caption[^\"]*", RegexOptions.IgnoreCase);

                buffer = buffer.Replace("\\", "");

                if (!rgx.IsMatch(buffer))
                    throw new Exception("No se encontró el link del video (url = )");

                urlSubs = "https://asset.fembed.com" + rgx.Match(buffer).Value;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "obtenerUrlSubs -> " + error;

            return (0 == error.Length);
        }

        string getDescription (string urlVideo)
        {
            try
            {
                return urlVideo.Substring(urlVideo.Length - 8, 4);
            }
            catch
            {
                return "Unknow";
            }
        }
    }
}
