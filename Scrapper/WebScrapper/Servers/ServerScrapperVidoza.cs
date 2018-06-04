using System;
using System.Text.RegularExpressions;
using Commons.CustomHttpManager;

namespace WebScrapper.Servers
{
    public class ServerScrapperVidoza : IServerScrapper
    {
        public override string name ()
        {
            return "VIDOZA";
        }

        public override bool scrappear (string url, ref Sources serverLinks, ref string error)
        {
            string buffer    = string.Empty;
            string codigo    = string.Empty;
            string urlAux    = string.Empty;
            string urlVideo  = string.Empty;
            string urlSubs   = string.Empty;
            string urlThumb  = string.Empty;

            if (0 == error.Length)
                HttpManager.requestGet(url, null, ref buffer, ref error);

            if (0 == error.Length)
                obtenerUrlVideo(url, buffer, ref urlVideo, ref error);

            if (0 == error.Length)
                obtenerUrlSubs(url, buffer, ref urlSubs, ref error);

            if (0 == error.Length)
                obtenerUrlThumb(url, buffer, urlVideo, ref urlThumb, ref error);

            if (0 == error.Length)
                HttpManager.requestGet(urlVideo, null, ref buffer, ref error);

            if (0 == error.Length)
                buscarUrlDefinitiva(url, buffer, ref urlVideo, ref error);

            if (0 == error.Length)
                if (base.esArchivoValido(urlVideo))
                    serverLinks.Add(new Source(urlVideo, urlSubs, "Default", name(), urlThumb));

            if (error.Length > 0)
                error = "ServerScrapperVidoza.scrappear -> " + error;

            return (0 == error.Length);
        }


        bool obtenerUrlVideo (string url, string buffer, ref string urlVideo, ref string error)
        {
            try
            {
                Regex rgx = new Regex("embed-.+html%22", RegexOptions.IgnoreCase);

                if (!rgx.IsMatch(buffer))
                    throw new Exception("No se encontró el link del video (url = " + url + ")");

                urlVideo = "https://vidoza.net/" + rgx.Match(buffer).Value.Substring(0, rgx.Match(buffer).Value.Length - 3);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "ServerScrapperVidoza.obtenerUrlVideo -> " + error;

            return (0 == error.Length);
        }

        bool obtenerUrlSubs (string url, string buffer, ref string urlSubs, ref string error)
        {
            try
            {
                Regex rgx = new Regex("https.+[^empty][.]srt", RegexOptions.IgnoreCase);

                if (!rgx.IsMatch(buffer))
                    throw new Exception("No se encontró el link del subtitulo (url = " + url + ")");

                urlSubs = rgx.Match(buffer).Value;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "ServerScrapperVidoza.obtenerUrlSubs -> " + error;

            return (0 == error.Length);
        }

        bool obtenerUrlThumb (string url, string buffer, string urlVideo, ref string urlThumb, ref string error)
        {
            try
            {
                Regex rgx = new Regex("image:.+[.jpg]", RegexOptions.IgnoreCase);

                if (!rgx.IsMatch(buffer))
                    throw new Exception("No se encontró el link del thumb (url = " + url + ")");

                urlThumb = rgx.Match(buffer).Value.Split("\"".ToCharArray())[1];
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "ServerScrapperVidoza.obtenerUrlThumb -> " + error;

            return (0 == error.Length);
        }

        bool buscarUrlDefinitiva (string url, string buffer, ref string urlVideo, ref string error)
        {
            try
            {
                Regex rgx = new Regex(@"sources: \[\{file:.+", RegexOptions.IgnoreCase);

                if (!rgx.IsMatch(buffer))
                    throw new Exception("No se encontró el link del thumb (url = " + url + ")");

                urlVideo = rgx.Match(buffer).Value.Split("\"".ToCharArray())[1];
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "ServerScrapperVidoza.buscarUrlDefinitiva -> " + error;

            return (0 == error.Length);
        }
    }
}
