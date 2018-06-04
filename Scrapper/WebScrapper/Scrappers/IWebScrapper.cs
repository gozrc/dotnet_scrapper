using System;
using WebScrapper.Entities;

namespace WebScrapper.Scrappers
{
    public abstract class IWebScrapper
    {
        public abstract Movies scrapMovies ();
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
    }
}
