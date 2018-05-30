using Scrapper.Web.Entities;


namespace Scrapper.Web.Scrappers
{
    public abstract class IWebScrap
    {
        public event OnMoviesTotal      onMoviesTotal;
        public event OnMoviesProgress   onMoviesProgress;
        public event OnSeriesTotal      onSeriesTotal;
        public event OnSeriesProgress   onSeriesProgress;
        public event OnCancelScrap      onCancelScrap;

        public abstract bool scrapMovies        (ref Movies movies, ref string error);
        public abstract bool scrapSeries        (ref Series series, ref string error);
        public abstract void scrapMoviesAsync   (string path);
        public abstract void scrapSeriesAsync   (string path);
        public abstract void cancelScrap        ();


        protected void throwMoviesTotal (int total)
        {
            onMoviesTotal?.Invoke(this, new OnMoviesTotalEventArgs(total));
        }
        protected void throwMoviesProgress (int index)
        {
            onMoviesProgress?.Invoke(this, new OnMoviesProgressEventArgs(index));
        }
        protected void throwSeriesTotal (int total)
        {
            onSeriesTotal?.Invoke(this, new OnSeriesTotalEventArgs(total));
        }
        protected void throwSeriesProgress (int index)
        {
            onSeriesProgress?.Invoke(this, new OnSeriesProgressEventArgs(index));
        }
        protected void throwCancelScrap ()
        {
            onCancelScrap?.Invoke(this, new OnCancelScrapEventArgs());
        }
    }
}
