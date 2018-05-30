using System;
using System.Collections.Generic;


namespace Scrapper.Web.Entities
{
    [Serializable]
    public class Seasons : List<Season>
    {
        public Seasons ()
        {
            //
        }

        public bool contains (string id)
        {
            foreach (Season s in this)
                if (s.id == id) return true;

            return false;
        }

        public Season search (string id)
        {
            foreach (Season s in this)
                if (s.id == id) return s;

            return null;
        }
    }
}
