using System;
using WebScrapper.Entities;

namespace WebScrapper.Scrappers
{
    public delegate void ScrapperMovieEventHandler (Movie movie);

    public class ScrapperMovieEventArgs : EventArgs
    {
        public readonly Movie movie = null;

        public ScrapperMovieEventArgs (Movie movie)
        {
            this.movie = movie;
        }
    }
}
