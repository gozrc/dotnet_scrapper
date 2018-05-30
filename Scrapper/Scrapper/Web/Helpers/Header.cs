using System.Collections.Generic;


namespace Scrapper.Web.Helpers
{
    public class Header
    {
        public string key   = string.Empty;
        public string value = string.Empty;

        public Header (string key, string value)
        {
            this.key   = key;
            this.value = value;
        }
    }

    public class Headers : List<Header>
    {
        //
    }
}
