using System;

namespace WebScrapper.Servers
{
    [Serializable]
    public class Source
    {
        public string name_server   = string.Empty;
        public string url_source    = string.Empty;
        public string url_subtitles = string.Empty;
        public string description   = string.Empty;
        

        public Source ()
        {

        }

        public Source (string name_server, string url_source, string url_subtitles, string description)
        {
            this.name_server    = name_server;
            this.url_source     = url_source;
            this.url_subtitles  = url_subtitles;
            this.description    = description;
        }
    }
}
