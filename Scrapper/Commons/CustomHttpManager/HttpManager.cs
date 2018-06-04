using System;
using System.Net;
using System.IO;
using System.Text;
using Commons.CustomHttpManager.Hacks;

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
        public static bool request (
            string          requestUrl, 
            Method          requestMethod, 
            HttpHeaders     requestHeaders, 
            byte[]          requestData, 
            ref int         responseCode, 
            ref string      responseDescription, 
            ref string      responseData,  
            ref HttpHeaders responseHeaders,
            bool            autoRedirect, 
            bool            emuleUserAgent,
            int             timeout,
            ref string      error
        )
        {
            responseCode         = -1;
            responseDescription  = "Internal error";
            responseData         = string.Empty;

            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(requestUrl);

                switch (requestMethod)
                {
                    case Method.GET:  webRequest.Method = "GET";  break;
                    case Method.HEAD: webRequest.Method = "HEAD"; break;
                    case Method.POST: webRequest.Method = "POST"; break;
                }

                webRequest.AllowAutoRedirect = autoRedirect;
                webRequest.ReadWriteTimeout  = timeout;

                if (null == requestHeaders)
                    requestHeaders = new HttpHeaders();

                if (emuleUserAgent)
                    requestHeaders.Add(new HttpHeader("UserAgent", "Mozilla/5.0"));

                addHeaders (requestHeaders, ref webRequest);

                if (requestMethod == Method.POST)
                {
                    if (!requestHeaders.exist("ContentLenght"))
                    {
                        if (null != requestData)
                            webRequest.ContentLength = requestData.Length;
                        else
                            webRequest.ContentLength = 0;
                    }

                    using (Stream st = webRequest.GetRequestStream())
                    {
                        st.Write (requestData, 0, requestData.Length);
                        st.Close ();
                    }
                }

                HttpWebResponse httpws = null;

                try
                {
                    httpws = (HttpWebResponse)webRequest.GetResponse();
                }
                catch (WebException wex)
                {
                    httpws = (HttpWebResponse)wex.Response;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                responseCode        = (int)httpws.StatusCode;
                responseDescription = httpws.StatusDescription;

                using (StreamReader sr = new StreamReader(httpws.GetResponseStream()))
                    responseData = sr.ReadToEnd().Trim();

                parseHeaders(httpws.Headers, ref responseHeaders);

                if (autoRedirect)
                {
                    if (httpws.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(string.Format("Server error (HTTP {0}: {1})",
                            (int)httpws.StatusCode, httpws.StatusDescription));
                    }
                }
                else
                {
                    bool isRedirectResponse =
                        httpws.StatusCode == (HttpStatusCode)301 ||
                        httpws.StatusCode == (HttpStatusCode)302 ||
                        httpws.StatusCode == (HttpStatusCode)303 ||
                        httpws.StatusCode == (HttpStatusCode)307;

                    if (httpws.StatusCode != HttpStatusCode.OK && !isRedirectResponse)
                    {
                        throw new Exception(string.Format("Server error (HTTP {0}: {1})",
                            (int)httpws.StatusCode, httpws.StatusDescription));
                    }
                }

                if (HackSucuri.checkSucuriProtection(responseData, responseHeaders))
                {
                    HackSucuri.addSucuriHeaders(responseData, ref requestHeaders);

                    return request(
                        requestUrl, requestMethod, requestHeaders, requestData, ref responseCode, 
                        ref responseDescription, ref responseData, ref responseHeaders, 
                        autoRedirect, emuleUserAgent, timeout, ref error
                    );
                }
            }
            catch (Exception ex)
            {
                error = "HttpManager.request -> " + ex.Message;
            }

            return (0 == error.Length);
        }


        public static bool requestGet (string requestUrl, HttpHeaders requestHeaders, ref string responseData, ref string error)
        {
            int         responseCode        = 0;
            string      responseDescription = string.Empty;
            HttpHeaders responseHeaders     = new HttpHeaders();

            return request(requestUrl, Method.GET, requestHeaders, null, ref responseCode, 
                ref responseDescription, ref responseData, ref responseHeaders, true, true, 2000, ref error);
        }

        public static bool requestGetSR (string requestUrl, HttpHeaders requestHeaders, ref string responseData, ref string error)
        {
            int         responseCode        = 0;
            string      responseDescription = string.Empty;
            HttpHeaders responseHeaders     = new HttpHeaders();

            return request(requestUrl, Method.GET, requestHeaders, null, ref responseCode,
                ref responseDescription, ref responseData, ref responseHeaders, false, true, 2000, ref error);
        }

        public static bool requestGetSR (string requestUrl, HttpHeaders requestHeaders, ref string responseData, ref HttpHeaders responseHeaders, ref string error)
        {
            int         responseCode        = 0;
            string      responseDescription = string.Empty;

            return request(requestUrl, Method.GET, requestHeaders, null, ref responseCode,
                ref responseDescription, ref responseData, ref responseHeaders, false, true, 2000, ref error);
        }

        public static bool requestPost (string requestUrl, HttpHeaders requestHeaders, string requestData, ref string responseData, ref string error)
        {
            int         responseCode        = 0;
            string      responseDescription = string.Empty;
            HttpHeaders responseHeaders     = new HttpHeaders();

            return request(requestUrl, Method.POST, requestHeaders, Encoding.UTF8.GetBytes(requestData), ref responseCode, 
                ref responseDescription, ref responseData, ref responseHeaders, true, true, 2000, ref error);
        }





        static void addHeaders (HttpHeaders headers, ref HttpWebRequest webRequest)
        {
            if (null == headers)
                return;

            foreach (HttpHeader h in headers)
            {
                bool existe = false;

                foreach (HttpRequestHeader e in Enum.GetValues(typeof(HttpRequestHeader)))
                {
                    if (e.ToString() == h.key)
                    {
                        switch (e)
                        {
                            case HttpRequestHeader.Referer:     webRequest.Referer      = h.value; existe = true; break;
                            case HttpRequestHeader.ContentType: webRequest.ContentType  = h.value; existe = true; break;
                            case HttpRequestHeader.UserAgent:   webRequest.UserAgent    = h.value; existe = true; break;


                            case HttpRequestHeader.Cookie:

                                string[] cookies = h.value.Split(';');

                                webRequest.CookieContainer = new CookieContainer(cookies.Length);

                                foreach (string cookie in cookies)
                                {
                                    string name  = cookie.Split('=')[0];
                                    string value = cookie.Split('=')[1];

                                    if (value == "/") value = "";

                                    webRequest.CookieContainer.Add (new Cookie(name, value, "", webRequest.RequestUri.Host));
                                }

                                existe = true;
                                break;
                        }

                        if (existe)
                            break;
                    }
                }

                if (!existe)
                    webRequest.Headers.Add (h.key, h.value);
            }
        }

        static void parseHeaders (WebHeaderCollection whc, ref HttpHeaders responseHeaders)
        {
            responseHeaders = new HttpHeaders();

            foreach (string key in whc.AllKeys)
                responseHeaders.Add(new HttpHeader(key, whc[key].ToString()));
        }




























        //public static bool requestHead (string url, HttpHeaders headers, ref string dataResponse, ref string error)
        //{
        //    int    codeResponse = 0;
        //    string webResponse  = string.Empty;

        //    return request(url, Method.HEAD, headers, null, ref codeResponse, ref webResponse, ref dataResponse, false, ref error);
        //}

        
    }
}
