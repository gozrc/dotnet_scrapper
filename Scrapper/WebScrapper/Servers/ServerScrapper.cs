
using System;
using System.Text.RegularExpressions;

namespace WebScrapper.Servers
{
    public static class ServerScrapper
    {
        public static bool scrap (string url, ref Sources sources, ref string error)
        {
            string serverName = getServerName(url);

            IServerScrapper scrapper = ServerFactory.dameScrapper(serverName);

            if (0 == error.Length)
                if (null == scrapper)
                    error = "Server unknow (name = " + serverName + ")";

            if (0 == error.Length)
                if (!scrapper.scrappear(url, ref sources, ref error))
                    error = "[" + scrapper.name() + "] " + error;

            return (0 == error.Length);
        }


        static string getServerName (string url) 
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

            return url;
        }
    }
}
