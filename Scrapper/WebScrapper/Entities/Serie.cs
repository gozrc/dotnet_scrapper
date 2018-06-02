using System;

namespace WebScrapper.Entities
{
    [Serializable]
    public class Serie
    {
        public string  id            = "";
        public string  title         = "";
        public string  description   = "";
        public string  url_image     = "";
        public string  url_web       = "";
        public Seasons seasons       = new Seasons();


        public Serie ()
        {
            //
        }

        public Serie (string id)
        {
            this.id = id;
        }
    }
}
