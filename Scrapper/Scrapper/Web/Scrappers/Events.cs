using System;


namespace Scrapper.Web.Scrappers
{
    public delegate void OnMoviesTotal      (object sender, OnMoviesTotalEventArgs    omtea);
    public delegate void OnMoviesProgress   (object sender, OnMoviesProgressEventArgs ompea);
    public delegate void OnSeriesTotal      (object sender, OnSeriesTotalEventArgs    omtea);
    public delegate void OnSeriesProgress   (object sender, OnSeriesProgressEventArgs ompea);
    public delegate void OnCancelScrap      (object sender, OnCancelScrapEventArgs    ocsea);

    public class OnMoviesTotalEventArgs : EventArgs
    {
        public readonly int total = 0;

        public OnMoviesTotalEventArgs (int total)
        {
            this.total = total;
        }
    }

    public class OnMoviesProgressEventArgs : EventArgs
    {
        public readonly int index = 0;

        public OnMoviesProgressEventArgs (int index)
        {
            this.index = index;
        }
    }

    public class OnSeriesTotalEventArgs : EventArgs
    {
        public readonly int total = 0;

        public OnSeriesTotalEventArgs (int total)
        {
            this.total = total;
        }
    }

    public class OnSeriesProgressEventArgs : EventArgs
    {
        public readonly int index = 0;

        public OnSeriesProgressEventArgs (int index)
        {
            this.index = index;
        }
    }

    public class OnCancelScrapEventArgs : EventArgs
    {
        //
    }
}
