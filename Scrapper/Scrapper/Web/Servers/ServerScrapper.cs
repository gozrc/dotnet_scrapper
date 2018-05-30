
using System.Text.RegularExpressions;


namespace Scrapper.Web.Servers
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
            Regex rgx = new Regex("(https://|http://)[a-zA-Z0-9(.)]+(/)", RegexOptions.IgnoreCase);

            if (rgx.IsMatch(url))
            {
                string valor = rgx.Match(url).Value;
                valor = valor.Substring(valor.IndexOf("//") + 2);

                if (valor.StartsWith("www."))
                    valor = valor.Substring(4);

                return valor.Substring(0, valor.Length - 1);
            }

            return string.Empty;
        }
    }
}
