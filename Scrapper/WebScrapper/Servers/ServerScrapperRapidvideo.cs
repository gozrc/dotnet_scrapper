using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Commons.CustomHttpManager;

namespace WebScrapper.Servers
{
    public class ServerScrapperRapidvideo : IServerScrapper
    {
        const string SERVER = "Rapidvideo";

        public override bool scrappear (string url, ref Sources serverLinks, ref string error)
        {
            string buffer    = string.Empty;
            string urlSubs   = string.Empty;
            string urlVideo  = string.Empty;
            string urlThumb  = string.Empty;

            List<string> urlsVideos = new List<string>();

            if (0 == error.Length)
                HttpManager.requestGet(url, null, ref buffer, ref error);

            if (0 == error.Length)
                obtenerUrlsVideos(url, buffer, ref urlsVideos, ref error);

            if (0 == error.Length)
            {
                foreach (string urlTmp in urlsVideos)
                {
                    if (!HttpManager.requestGet(urlTmp, null, ref buffer, ref error))
                        break;

                    if (!obtenerUrlThumb(url, buffer, ref urlThumb, ref error))
                        break;

                    if (!obtenerUrlVideoSubs(url, buffer, ref urlVideo, ref urlSubs, ref error))
                        break;

                    if (base.esArchivoValido(urlVideo))
                        serverLinks.Add(new Source(urlVideo, urlSubs, urlTmp.Substring(urlTmp.IndexOf("=") + 1), SERVER, urlThumb));
                }
            }

            if (error.Length > 0)
                error = "ServerScrapperRapidvideo.scrappear -> " + error;

            return (0 == error.Length);
        }


        bool obtenerUrlsVideos (string url, string buffer, ref List<string> urlsVideos, ref string error)
        {
            try
            {
                Regex rgx = new Regex("<a href=\"https://www[.]rapidvideo[.]com/e/(.+)p\">", RegexOptions.IgnoreCase);

                foreach (Match match in rgx.Matches(buffer))
                    urlsVideos.Add(match.Value.Split("\"".ToCharArray())[1]);

                if (urlsVideos.Count == 0)
                    throw new Exception("No se encontró el link del stream para (url = " + url + ")");
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "ServerScrapperRapidvideo.obtenerUrlsVideos -> " + error;

            return (0 == error.Length);
        }

        bool obtenerUrlVideoSubs (string url, string buffer, ref string urlVideo, ref string urlSubs, ref string error)
        {
            try
            {
                Regex rgx = new Regex("<video.+</video>", RegexOptions.Singleline);

                if (!rgx.IsMatch(buffer))
                    throw new Exception("No se encontró el link del video (url = " + url + ")");

                string value = rgx.Match(buffer).Value;

                rgx = new Regex("<source.+/>", RegexOptions.Singleline);

                if (!rgx.IsMatch(value))
                    throw new Exception("No se encontró el link del video (url = " + url + ")");

                urlVideo = rgx.Match(value).Value.Split("\"".ToCharArray())[1];

                rgx = new Regex("<track.+default>", RegexOptions.Singleline);

                if (!rgx.IsMatch(value))
                    throw new Exception("No se encontró el link del subtitulo (url = " + url + ")");

                urlSubs = "https://www.rapidvideo.com" + rgx.Match(value).Value.Split("\"".ToCharArray())[1];
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "ServerScrapperRapidvideo.obtenerUrlVideoSubs -> " + error;

            return (0 == error.Length);
        }

        bool obtenerUrlThumb (string url, string buffer, ref string urlThumb, ref string error)
        {
            try
            {
                Regex rgx = new Regex("<meta property=\"og:image.+>", RegexOptions.IgnoreCase);

                if (!rgx.IsMatch(buffer))
                    throw new Exception("No se encontró el link del thumb (url = " + url + ")");

                urlThumb = rgx.Match(buffer).Value.Split('\"')[3];
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "ServerScrapperRapidvideo.obtenerUrlThumb -> " + error;

            return (0 == error.Length);
        }
    }
}
