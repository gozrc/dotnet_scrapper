
using System;

namespace WebScrapper.Servers
{
    public static class ServerScrapper
    {
        public static bool scrap (string url, ref Sources sources, ref string error)
        {
            string nombreServidor = dameNombreServidor(url);

            IServerScrapper scrapper = ServerFactory.dameScrapper(nombreServidor);

            if (null == scrapper)
            {
                error = "Servidor desconocido (url = " + url + ")";
            }
            else
            {
                scrapper.scrappear(url, ref sources, ref error);
            }

            if (error.Length > 0)
                error = "ServerScrapper.scrap -> " + error;

            return (0 == error.Length);
        }


        static string dameNombreServidor (string url) 
        {
            int index = url.LastIndexOf("/");

            if (index < 0)
                return string.Empty;

            return url.Substring(0, index);
        }
    }
}
