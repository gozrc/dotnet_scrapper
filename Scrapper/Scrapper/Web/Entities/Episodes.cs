using System;
using System.Collections.Generic;


namespace Scrapper.Web.Entities
{
    [Serializable]
    public class Episodes : List<Episode>
    {
        public Episodes ()
        {
            //
        }

        public bool contains (string id)
        {
            foreach (Episode e in this)
                if (e.id == id) return true;

            return false;
        }

        public Episode search (string id)
        {
            foreach (Episode e in this)
                if (e.id == id) return e;

            return null;
        }
    }
}
