using System;
using System.Collections.Generic;
using Scrapper.Web.Helpers;


namespace Scrapper.Web.Servers
{
    public class ServerScrapperHtml5Pelispedia : IServerScrapper
    {
        const string SERVER = "Html5 Pelispedia";


        public override bool scrappear (string url, ref Sources serverLinks, ref string error)
        {
            string buffer   = string.Empty;
            string urlSubs  = string.Empty;
            string urlVideo = string.Empty;
            string codigo   = string.Empty;
            string token    = string.Empty;

            List<string> linksVideos = new List<string>();

            if (0 == error.Length)
                obtenerLinksVideos(url, ref linksVideos, ref error);

            if (0 == error.Length)
                obtenerUrlSubs(url, ref urlSubs, ref error);

            if (0 == error.Length)
            {
                foreach (string linkVideo in linksVideos)
                {
                    if (!HttpHelper.requestGet(linkVideo, null, ref buffer, ref error))
                        break;

                    if (!base.obtenerCodigo(buffer, ref codigo, ref error))
                        break;

                    if (!obtenerToken(codigo, ref token, ref error))
                        break;

                    if (!obtenerUrlVideo(linkVideo, token, ref urlVideo, ref error))
                        break;

                    if (base.esArchivoValido(urlVideo))
                        serverLinks.Add(new Source(urlVideo, urlSubs, "Opción " + (serverLinks.Count + 1).ToString(), SERVER, ""));
                }
            }

            if (error.Length > 0)
                error = "ServerScrapperOpenload.scrappear -> " + error;

            return (0 == error.Length);
        }

        bool obtenerLinksVideos (string url, ref List<string> linksVideos, ref string error)
        {
            try
            {
                string[] partes = url.Split("?&".ToCharArray());

                string idSub = partes[1].Split('=')[1];

                for (int k = 2; k < partes.Length - 2; k++)
                {
                    string id   = partes[k].Substring(partes[k].IndexOf('=') + 1);
                    string link = string.Format("https://www1.pelispedia.tv/ver.php?id={0}&sub={1}", id, idSub);

                    linksVideos.Add(link);
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "ServerScrapperHtml5Pelispedia.obtenerLinksVideos -> " + error;

            return (0 == error.Length);
        }

        bool obtenerUrlSubs (string url, ref string urlSubs, ref string error)
        {
            try
            {
                urlSubs = string.Format("https://www1.pelispedia.tv/sub/{0}.srt", url.Split(" ? &".ToCharArray())[1].Split('=')[1]);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "ServerScrapperHtml5Pelispedia.obtenerUrlVideo -> " + error;

            return (0 == error.Length);
        }

        bool obtenerToken (string codigo, ref string token, ref string error)
        {
            token = (new AES()).OpenSSLEncrypt2(codigo, "_KeyCryptSchedule");
            return true;
        }

        bool obtenerUrlVideo (string linkVideo, string token, ref string urlVideo, ref string error)
        {
            try
            {
                string buffer   = string.Empty;
                string id       = linkVideo.Split("?&".ToCharArray())[1].Split('=')[1];
                string request  = string.Format("link={0}&token={1}", id, token);
                string url      = "https://www1.pelispedia.tv/plugins/gkpluginsphp.php";
                Headers headers = new Headers();
                   
                headers.Add(new Header("ContentType", "application/x-www-form-urlencoded; charset=UTF-8"));

                if (!HttpHelper.requestPost(url, headers, request, ref buffer, ref error))
                    throw new Exception("No se encontró el link del video (url = " + linkVideo + ")");

                urlVideo = buffer.Split("\"".ToCharArray())[3].Replace("\\", "");
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "ServerScrapperHtml5Pelispedia.obtenerUrlVideo -> " + error;

            return (0 == error.Length);
        }
    }
}
