using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Commons.CustomHttpManager;

namespace WebScrapper.Servers
{
    public class ServerScrapperOpenload : IServerScrapper
    {
        const string SERVER = "Openload";


        public override bool scrappear (string url, ref Sources serverLinks, ref string error)
        {
            string buffer    = string.Empty;
            string urlVideo  = string.Empty;
            string urlSubs   = string.Empty;
            string urlThumb  = string.Empty;

            if (0 == error.Length)
                HttpManager.requestGet(url, null, ref buffer, ref error);

            if (0 == error.Length)
                obtenerUrlSubs(url, buffer, ref urlSubs, ref error);

            if (0 == error.Length)
                obtenerUrlThumb(url, buffer, ref urlThumb, ref error);

            if (0 == error.Length)
                obtenerUrlVideo(url, buffer, ref urlVideo, ref error);

            if (0 == error.Length)
                HttpManager.requestGetSR(urlVideo, null, ref buffer, ref error);

            if (0 == error.Length)
                buscarUrlDefinitiva(buffer, ref urlVideo, ref error);

            if (0 == error.Length)
                if (base.esArchivoValido(urlVideo))
                    serverLinks.Add(new Source(urlVideo, urlSubs, "Default", SERVER, urlThumb));

            if (error.Length > 0)
                error = "ServerScrapperOpenload.scrappear -> " + error;

            return (0 == error.Length);
        }


        bool obtenerUrlSubs (string url, string buffer, ref string urlSubs, ref string error)
        {
            try
            {
                Regex rgx = new Regex(@"<track kind.+>", RegexOptions.IgnoreCase);

                if (!rgx.IsMatch(buffer))
                    throw new Exception("No se encontró el link del subtitulo (url = " + url + ")");

                urlSubs = rgx.Match(buffer).Value.Split("\"".ToCharArray())[3];
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "ServerScrapperOpenload.obtenerUrlSubs -> " + error;

            return (0 == error.Length);
        }

        bool obtenerUrlThumb (string url, string buffer, ref string urlThumb, ref string error)
        {
            try
            {
                Regex rgx = new Regex("<meta name=\"og:image.+>", RegexOptions.IgnoreCase);

                if (!rgx.IsMatch(buffer))
                    throw new Exception("No se encontró el link del thumb (url = " + url + ")");

                urlThumb = rgx.Match(buffer).Value.Split("\"".ToCharArray())[3];
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "ServerScrapperOpenload.obtenerUrlThumb -> " + error;

            return (0 == error.Length);
        }

        bool obtenerUrlVideo (string url, string buffer, ref string urlVideo, ref string error)
        {
            try
            {
                ulong parametro1 = 0;
                ulong parametro2 = 0;
                ulong parametro3 = 0;

                Regex rgx = new Regex("<span.+</span>", RegexOptions.Singleline);

                // aca buscar ">window.fileid="TO4fqkfjp80""

                if (!rgx.IsMatch(buffer))
                    throw new Exception("No se encontró el link del video (url = " + url + ")");

                string id = rgx.Match(buffer).Value.Split("<>".ToCharArray())[2];

                rgx = new Regex("_0x30725e,[(]parseInt.+_1x4bfb36[)];continue;case", RegexOptions.Singleline);

                if (!rgx.IsMatch(buffer))
                    throw new Exception("No se encontró el link del video (url = " + url + ")");

                string valor = rgx.Match(buffer).Value;

                string aux1 = valor.Split("'".ToCharArray())[1];
                parametro1 = Convert.ToUInt64(aux1, 8);

                aux1 = valor.Split(")".ToCharArray())[1];
                aux1 = aux1.Substring(0, aux1.Length - 4);
                aux1 = aux1.Substring(1);

                ulong numberAux = Convert.ToUInt64(aux1);

                parametro1 = parametro1 - numberAux + 4;

                aux1 = valor.Split("()".ToCharArray())[5];
                aux1 = aux1.Substring(0, aux1.Length - 4);

                parametro2 = Convert.ToUInt64(aux1) - 8;

                rgx = new Regex("_1x4bfb36=parseInt[(]'.+'[,]8[)]", RegexOptions.Singleline);

                if (!rgx.IsMatch(buffer))
                    throw new Exception("No se encontró el link del video (url = " + url + ")");

                valor = rgx.Match(buffer).Value;

                aux1 = valor.Split("'".ToCharArray())[1];

                parametro3 = Convert.ToUInt64(aux1, 8);

                urlVideo = string.Format("https://oload.stream/stream/{0}?mime=true", decodificar(id, parametro1, parametro2, parametro3));
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "ServerScrapperOpenload.obtenerUrlVideo -> " + error;

            return (0 == error.Length);
        }

        string decodificar (string cadena, ulong parametro1, ulong parametro2, ulong parametro3)
        {
            string          resultado   = "";
            string          id          = cadena.Substring(0, 72);
            List<ulong>     arreglo     = new List<ulong>();
            int             iterador    = 0;
            ulong           varD        = 0;

            for (int i = 0; i < id.Length; i += 8)
            {
                string a = id.Substring(i, 8);
                arreglo.Add(Convert.ToUInt32(a, 16));
            }

            cadena = cadena.Substring(72);

            while (iterador < cadena.Length)
            {
                ulong h01 = 0x40;
                ulong h02 = 0x7f;
                ulong h03 = 0x0;
                ulong h04 = 0x0;
                ulong h05 = 0x0;

                ulong varC = 0x3f;

                do
                {

                    if (iterador + 1 >= cadena.Length)
                        h01 = 0x8f;

                    string aux06 = cadena.Substring(iterador, 2);

                    iterador++;
                    iterador++;

                    h05 = Convert.ToUInt64(aux06, 16);

                    if (h04 < 30)
                    {
                        h03 += (ulong)((int)(h05 & varC) << (int)h04);
                    }
                    else
                    {
                        h03 += (h05 & varC) * Convert.ToUInt64(Math.Pow(2, h04));
                    }

                    h04 += 0x6;

                } while (h05 >= h01);

                ulong aux01 = parametro3;
                ulong aux02 = (ulong)h03 ^ (ulong)(arreglo[(int)varD % 0x9]);

                aux02 = (ulong)(((ulong)aux02 ^ parametro1 / parametro2) ^ (ulong)aux01);

                ulong aux03 = (h01 * 2) + h02;

                for (int i = 0; i < 4; i++)
                {
                    ulong aux04 = aux02 & aux03;
                    int aux05 = (72 / 0x9) * i;

                    aux04 = aux04 >> aux05;

                    string aux06 = new String(new char[] { (char)(aux04 - 1) });

                    if (aux06 != "%")
                        resultado += aux06;

                    aux03 = (aux03 << 72 / 0x9);
                }

                varD += 0x1;
            }

            return resultado;
        }

        bool buscarUrlDefinitiva (string buffer, ref string urlStream, ref string error)
        {
            urlStream = buffer.Replace("?mime=true", "");
            return true;
        }
    }
}
