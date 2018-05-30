using System;
using System.Collections.Generic;


namespace Scrapper.Web.Entities
{
    [Serializable]
    public class Series : List<Serie>
    {
        public Series ()
        {
            //
        }


        public bool contains (string id)
        {
            foreach (Serie s in this)
                if (s.id == id) return true;

            return false;
        }

        public Serie search (string id)
        {
            foreach (Serie s in this)
                if (s.id == id) return s;

            return null;
        }

        public int episodesCount
        {
            get
            {
                int count = 0;

                foreach (Serie serie in this)
                    foreach (Season season in serie.seasons)
                        foreach (Episode episode in season.episodes)
                            count++;

                return count;
            }
        }
    }
}
