using System;
using WebScrapper.Entities;
using WebScrapper.Servers;

namespace WebScrapper.Scrappers
{
    public abstract class IWebScrapper
    {
        public event ScrapperLogEventHandler    onLog;
        public event ScrapperMovieEventHandler  onMovie;


        public abstract string name ();

        public abstract void scrapMovies ();

        public abstract void getMovieSources (string urlMovie, ref Sources sources);



        public abstract Series scrapSeries ();

        protected void log (string scrapper, string title, string detail)
        {
            string text = string.Format(
                "[{0}] :: {1} :: {2} :: {3}", 
                DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), 
                scrapper,
                title, 
                detail
            );

            Console.WriteLine(text);
        }


        protected void runOnLog (string scrapperName, string title, string description)
        {
            onLog?.Invoke(scrapperName + " :: " + title, description);
        }

        protected void runOnMovie (Movie movie)
        {
            onMovie?.Invoke(movie);
        }
    }
}
