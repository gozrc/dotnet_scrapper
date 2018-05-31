using System;
using System.Net;
using System.IO;
using System.Text;

namespace Commons.CustomHttpManager
{
    public enum Method
    {
        POST,
        GET,
        HEAD
    }

    public class HttpManager
    {
        public static bool request (string url, Method method, HttpHeaders headers, byte[] data, ref int codeResponse, ref string webResponse, ref string dataResponse, bool autoRedirect, ref string error)
        {
            codeResponse = -1;
            webResponse = string.Empty;
            dataResponse = string.Empty;

            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

                webRequest.Method = (method == Method.GET ? "GET" : (method == Method.POST ? "POST" : "HEAD"));
                webRequest.AllowAutoRedirect = autoRedirect;
                webRequest.ReadWriteTimeout = 2000;

                addHeaders (headers, ref webRequest);

                webRequest.ContentLength = null != data ? data.Length : 0;

                if (method == Method.POST)
                {
                    using (Stream st = webRequest.GetRequestStream())
                    {
                        st.Write(data, 0, data.Length);
                        st.Close();
                    }
                }

                using (WebResponse wres = webRequest.GetResponse())
                {
                    HttpWebResponse httpws = (HttpWebResponse)wres;

                    codeResponse = (int)httpws.StatusCode;
                    webResponse = httpws.StatusDescription;

                    if (autoRedirect)
                    {
                        if (httpws.StatusCode != HttpStatusCode.OK)
                            throw new Exception(string.Format("Server error (HTTP {0}: {1})", (int)httpws.StatusCode, httpws.StatusDescription));
                    }
                    else
                    {
                        if (httpws.StatusCode != HttpStatusCode.OK && httpws.StatusCode != (HttpStatusCode)302)
                            throw new Exception(string.Format("Server error (HTTP {0}: {1})", (int)httpws.StatusCode, httpws.StatusDescription));
                    }

                    using (StreamReader sr = new StreamReader(wres.GetResponseStream()))
                        dataResponse = sr.ReadToEnd();

                    if (dataResponse.Length == 0)
                    {
                        if (!autoRedirect && codeResponse == 302)
                        {
                            if (httpws.Headers.Count > 0)
                                if (httpws.Headers["Location"] != null)
                                    dataResponse = httpws.Headers["Location"].ToString();
                        }
                        else
                        {
                            if (method == Method.HEAD)
                            {
                                dataResponse = httpws.Headers.ToString();
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                error = "HttpHelper.request -> " + ex.Message;
            }

            return (0 == error.Length);
        }





        static void addHeaders (HttpHeaders headers, ref HttpWebRequest webRequest)
        {
            if (null != headers)
            {
                foreach (HttpHeader h in headers)
                {
                    bool existe = false;

                    foreach (HttpRequestHeader e in Enum.GetValues(typeof(HttpRequestHeader)))
                    {
                        if (e.ToString() == h.key)
                        {
                            switch (e)
                            {
                                case HttpRequestHeader.Referer:

                                    webRequest.Referer = h.value;
                                    existe = true;
                                    break;

                                case HttpRequestHeader.ContentType:

                                    webRequest.ContentType = h.value;
                                    existe = true;
                                    break;

                                case HttpRequestHeader.UserAgent:

                                    webRequest.UserAgent = h.value;
                                    existe = true;
                                    break;

                                case HttpRequestHeader.Cookie:

                                    string[] cookies = h.value.Split(';');

                                    webRequest.CookieContainer = new CookieContainer(cookies.Length);

                                    foreach (string cookie in cookies)
                                    {
                                        string name = cookie.Split('=')[0];
                                        string value = cookie.Split('=')[1];

                                        if (value == "/") value = "";

                                        webRequest.CookieContainer.Add(new Cookie(name, value, "", webRequest.RequestUri.Host));
                                    }

                                    existe = true;
                                    break;
                            }

                            if (existe)
                                break;
                        }
                    }

                    if (!existe)
                        webRequest.Headers.Add(h.key, h.value);
                }
            }

        }
































        public static bool requestGet (string url, HttpHeaders headers, ref string dataResponse, ref string error)
        {
            int     codeResponse = 0;
            string  webResponse  = string.Empty;

            return request (url, Method.GET, headers, null, ref codeResponse, ref webResponse, ref dataResponse, true, ref error);
        }

        public static bool requestGetSinReditect(string url, HttpHeaders headers, ref string dataResponse, ref string error)
        {
            int codeResponse = 0;
            string webResponse = string.Empty;

            return request(url, Method.GET, headers, null, ref codeResponse, ref webResponse, ref dataResponse, false, ref error);
        }

        public static bool requestPost (string url, HttpHeaders headers, string data, ref string dataResponse, ref string error)
        {
            int    codeResponse = 0;
            string webResponse  = string.Empty;

            return request(url, Method.POST, headers, Encoding.UTF8.GetBytes(data), ref codeResponse, ref webResponse, ref dataResponse, true, ref error);
        }

        public static bool requestHead (string url, HttpHeaders headers, ref string dataResponse, ref string error)
        {
            int    codeResponse = 0;
            string webResponse  = string.Empty;

            return request(url, Method.HEAD, headers, null, ref codeResponse, ref webResponse, ref dataResponse, false, ref error);
        }

        
    }

}
