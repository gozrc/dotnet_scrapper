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
            string urlThumb = string.Empty;

            if (0 == error.Length)
                HttpManager.requestGet(url, null, ref buffer, ref error);

            if (0 == error.Length)
                obtenerUrlSubs(url, buffer, ref urlSubs, ref error);

            if (0 == error.Length)
                obtenerUrlVideo(url, buffer, ref urlVideo, ref error);

            if (0 == error.Length)
                obtenerUrlThumb(url, buffer, ref urlThumb, ref error);

            if (0 == error.Length)
                if (base.esArchivoValido(urlVideo))
                    serverLinks.Add(new Source(urlVideo, urlSubs, "Default", name(), urlThumb));

            if (error.Length > 0)
                error = "scrappear -> " + error;

            return (0 == error.Length);
        }


        bool obtenerUrlSubs (string url, string buffer, ref string urlSubs, ref string error)
        {
            try
            {
                Regex rgx = new Regex("<track kind=\"captions\" src=(.+)/>", RegexOptions.IgnoreCase);

                if (!rgx.IsMatch(buffer))
                    throw new Exception("No se encontró el link del subtitulo (url = " + url + ")");

                string valor = rgx.Match(buffer).Value;

                urlSubs = valor.Split("\"".ToCharArray())[3];
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "obtenerUrlSubs -> " + error;

            return (0 == error.Length);
        }

        bool obtenerUrlVideo (string url, string buffer, ref string urlVideo, ref string error)
        {
            try
            {
                Regex rgx = new Regex("srces.push.+", RegexOptions.IgnoreCase);

                if (!rgx.IsMatch(buffer))
                    throw new Exception("No se encontró el link del video (url = " + url + ")");

                string[] valores = rgx.Match(buffer).Value.Split("()',".ToCharArray());

                string token_str = valores[4];
                int    token_nbr = int.Parse(valores[6]);

                urlVideo = "https:" + decodificar(token_str, token_nbr);

                if (!HttpManager.requestGetSR(urlVideo, null, ref buffer, ref error))
                    throw new Exception(error);

                urlVideo = buffer;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "obtenerUrlVideo -> " + error;

            return (0 == error.Length);
        }

        bool obtenerUrlThumb (string url, string buffer, ref string urlThumb, ref string error)
        {
            try
            {
                Regex rgx = new Regex("<meta name=\"og:image.+>", RegexOptions.IgnoreCase);

                if (!rgx.IsMatch(buffer))
                    throw new Exception("No se encontró el link del thumb (url = " + url + ")");

                urlThumb = rgx.Match(buffer).Value.Split('\"')[3];
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "obtenerUrlThumb -> " + error;

            return (0 == error.Length);
        }

        string decodificar (string cadena, int parametro)
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
