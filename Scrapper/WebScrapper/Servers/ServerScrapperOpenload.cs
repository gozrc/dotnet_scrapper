using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Commons.CustomHttpManager;

namespace WebScrapper.Servers
{
    public class ServerScrapperOpenload : IServerScrapper
    {
        public override string name ()
        {
            return "OPENLOAD";
        }

        public override bool scrappear (string url, ref Sources serverLinks, ref string error)
        {
            string      buffer    = string.Empty;
            string      urlVideo  = string.Empty;
            string      urlSubs   = string.Empty;
            string      urlThumb  = string.Empty;
            HttpHeaders rHeaders  = new HttpHeaders();

            if (0 == error.Length)
                HttpManager.requestGet(url, null, ref buffer, ref error);

            if (0 == error.Length)
                obtenerUrlSubs(url, buffer, ref urlSubs, ref error);

            if (0 == error.Length)
                obtenerUrlThumb(url, buffer, ref urlThumb, ref error);

            if (0 == error.Length)
                obtenerUrlVideo(url, buffer, ref urlVideo, ref error);

            if (0 == error.Length)
                HttpManager.requestGetSR(urlVideo, null, ref buffer, ref rHeaders, ref error);

            if (0 == error.Length)
                if (!rHeaders.exist("Location"))
                    error = "Error $$$$";

            if (0 == error.Length)
                urlVideo = rHeaders.value("Location").Replace("?mime=true", "");

            if (0 == error.Length)
                if (base.esArchivoValido(urlVideo))
                    serverLinks.Add(new Source(urlVideo, urlSubs, "Default", name(), urlThumb));

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
                string magic = openloadMagic(buffer);
                urlVideo = string.Format("https://openload.co/stream/{0}?mime=true", magic);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "ServerScrapperOpenload.obtenerUrlVideo -> " + error;

            return (0 == error.Length);
        }


        string openloadMagic (string offuscatedData)
        {
            // Search for code

            Regex rgxC = new Regex("<p style=\"\" id=\"[^\"]+\">(.*?)</p>", RegexOptions.Singleline);

            if (!rgxC.IsMatch(offuscatedData))
                throw new Exception("openloadMagix error 0");

            string code = rgxC.Match(offuscatedData).Value.Split("<>".ToCharArray())[2];

            // Search for param 1

            Regex rgxP1 = new Regex(@"_0x30725e,(\(parseInt.*?)\),", RegexOptions.Singleline);

            if (!rgxP1.IsMatch(offuscatedData))
                throw new Exception("openloadMagix error 1");

            string p1 = evalJS(rgxP1.Match(offuscatedData).Value, 1);

            // Search for param 2

            Regex rgxP2 = new Regex("_0x59ce16=([^;]+)", RegexOptions.Singleline);

            if (!rgxP2.IsMatch(offuscatedData))
                throw new Exception("openloadMagix error 2");

            string p2 = evalJS(rgxP2.Match(offuscatedData).Value.Split('=')[1], 2);

            // Search for param 3

            Regex rgxP3 = new Regex("_1x4bfb36=([^;]+)", RegexOptions.Singleline);

            if (!rgxP3.IsMatch(offuscatedData))
                throw new Exception("openloadMagix error 3");

            string p3 = evalJS(rgxP3.Match(offuscatedData).Value.Split('=')[1], 3);

            return decode (code, ulong.Parse(p1), ulong.Parse(p2), ulong.Parse(p3));
        }

        string decode (string cadena, ulong parametro1, ulong parametro2, ulong parametro3)
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
                ulong h01 = 64;
                ulong h03 = 0;
                ulong h04 = 0;
                ulong h05 = 0;

                while (true)
                {
                    if (iterador + 1 >= cadena.Length)
                        h01 = 143;

                    h05 = Convert.ToUInt64(cadena.Substring(iterador, 2), 16);
                    iterador = iterador + 2;

                    if (h04 < 30)
                        h03 += (ulong)((int)(h05 & 63) << (int)h04);
                    else
                        h03 += (h05 & 63) * Convert.ToUInt64(Math.Pow(2, h04));

                    h04 += 6;

                    if (!(h05 >= h01))
                        break;
                }

                ulong nuevo1 = (ulong)h03 ^ (ulong)(arreglo[(int)varD % 9]) ^ parametro1 ^ parametro3;
                ulong nuevo2 = h01 * 2 + 127;

                for (int i = 0; i < 4; i++)
                {
                    string nuevo3 = new String(new char[] { (char)(((nuevo1 & nuevo2) >> 8 * i) - 1) });

                    if (nuevo3 != "$")
                        resultado += nuevo3;

                    nuevo2 = (nuevo2 << 8);
                }

                varD += 0x1;
            }

            return resultado;
        }

        string evalJS (string jsCode, int param)
        {
            if (param == 1)
            {
                // "_0x30725e,(parseInt('33757170523',8)-917+0x4-2)/(14-0x8)),"

                ulong number1 = Convert.ToUInt64(jsCode.Split("'".ToCharArray())[1], 8);
                ulong number2 = Convert.ToUInt64(jsCode.Split("-+()".ToCharArray())[4]);
                ulong number3 = Convert.ToUInt64(jsCode.Split("-+()".ToCharArray())[5], 16);
                ulong number4 = Convert.ToUInt64(jsCode.Split("-+()".ToCharArray())[6]);
                ulong number5 = Convert.ToUInt64(jsCode.Split("-+()".ToCharArray())[8]);
                ulong number6 = Convert.ToUInt64(jsCode.Split("-+()".ToCharArray())[9], 16);

                return ((number1 - number2 + number3 - number4) / (number5 - number6)).ToString();
            }

            if (param == 2)
            {
                // 0x28a28dec

                ulong number = Convert.ToUInt64(jsCode, 16);
                return number.ToString();
            }

            if (param == 3)
            {
                // parseInt('24045132072',8)-34

                ulong number1  = Convert.ToUInt64(jsCode.Split("'".ToCharArray())[1], 8);
                ulong number2 = Convert.ToUInt64(jsCode.Split('-')[1]);

                return (number1 - number2).ToString();
            }

            throw new Exception("EvalJS -> error al parsear");
        }
    }
}
