using System;

namespace Commons.CustomHttpManager
{
    public class HttpHeader
    {
        public string key   = string.Empty;
        public string value = string.Empty;

        public HttpHeader (string key, string value)
        {
            this.key   = key;
            this.value = value;
        }

        public override string ToString()
        {
            return key + ": " + value;
        }
    }
}
