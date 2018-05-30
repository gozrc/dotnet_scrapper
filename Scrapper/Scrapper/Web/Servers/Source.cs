using System;


namespace Scrapper.Web.Servers
{
    [Serializable]
    public class Source
    {
        public string url_source    = string.Empty;
        public string url_subtitles = string.Empty;
        public string description   = string.Empty;
        public string name_server   = string.Empty;
        public string url_thumbnail = string.Empty;
        public string file_size     = string.Empty;


        public Source ()
        {

        }

        public Source (string url_source, string url_subtitles, string description, string name_server, string url_thumbnail)
        {
            this.url_source     = url_source;
            this.url_subtitles  = url_subtitles;
            this.description    = description;
            this.name_server    = name_server;
            this.url_thumbnail  = url_thumbnail;
        }
    }
}
