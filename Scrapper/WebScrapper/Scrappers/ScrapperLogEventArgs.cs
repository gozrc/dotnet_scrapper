using System;

namespace WebScrapper.Scrappers
{
    public delegate void ScrapperLogEventHandler (string title, string description);

    public class ScrapperLogEventArgs : EventArgs
    {
        public readonly string title        = string.Empty;
        public readonly string description  = string.Empty;

        public ScrapperLogEventArgs (string title, string description)
        {
            this.title       = title;
            this.description = description;
        }
    }
}
