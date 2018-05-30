using System;
using System.Collections.Generic;


namespace Scrapper.Web.Entities
{
    [Serializable]
    public class Movies : List<Movie>
    {
        public Movies ()
        {
            //
        }

        public bool contains (string id)
        {
            foreach (Movie m in this)
                if (m.id == id) return true;

            return false;
        }
    }
}
