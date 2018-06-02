using System;
using System.Collections.Generic;

namespace Commons.CustomHttpManager
{
    public class HttpHeaders : List<HttpHeader>
    {
        public bool exist (string key)
        {
            foreach (HttpHeader h in this)
                if (h.key == key)
                    return true;

            return false;
        }

        public string value (string key)
        {
            foreach (HttpHeader h in this)
                if (h.key == key)
                    return h.value;

            return string.Empty;
        }
    }
}
