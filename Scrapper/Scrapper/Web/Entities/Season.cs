using System;


namespace Scrapper.Web.Entities
{
    [Serializable]
    public class Season
    {
        public string   id            = string.Empty;
        public string   title         = string.Empty;
        public string   description   = string.Empty;
        public string   url_web       = string.Empty;
        public string   url_imagen    = string.Empty;
        public Episodes episodes      = new Episodes();


        public Season ()
        {
            //
        }

        public Season (string id)
        {
            this.id = id;
        }


        //  sacar
        public Season (string nombre, string direccion, string imagen)
        {
            this.title     = nombre;
            this.url_web  = direccion;
            this.url_imagen     = imagen;
        }
    }
}
