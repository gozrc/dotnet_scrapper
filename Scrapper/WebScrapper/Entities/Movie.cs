using System;
using WebScrapper.Servers;

namespace WebScrapper.Entities
{
    [Serializable]
    public class Movie
    {
        public string  id            = "";
        public string  title         = "";
        public string  description   = "";
        public string  url_image     = "";
        public string  url_web       = "";
        public Sources sources       = new Sources();


        public Movie ()
        {
            //
        }

        public Movie (string id)
        {
            this.id = id;
        }
    }
}
