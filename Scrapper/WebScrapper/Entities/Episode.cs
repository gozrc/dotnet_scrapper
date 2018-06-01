using System;
using WebScrapper.Servers;

namespace WebScrappers.Entities
{
    [Serializable]
    public class Episode
    {
        public string  id           = string.Empty;
        public string  title        = string.Empty;
        public string  description  = string.Empty;
        public string  url_web      = string.Empty;
        public string  url_image    = string.Empty;
        public Sources sources      = new Sources();


        public Episode ()
        {

        }


        public Episode (string id)
        {
            this.id = id;
        }
    }
}
