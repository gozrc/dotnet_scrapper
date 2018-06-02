using System;
using WebScrapper.Entities;

namespace WebScrapper.Scrappers
{
    public abstract class IWebScrapper
    {
        public abstract Movies scrapMovies ();
        public abstract Series scrapSeries ();
    }
}
