using System;
using System.Collections.Generic;
using WebScrapper.Entities;
using WebScrapper.Cryptography;
using Commons.CustomHttpManager;
using WebScrapper.Servers;

namespace WebScrapper.Scrappers.Pelispedia
{
    public class ScrapPelispedia : IWebScrapper
    {
        const string URL_MOVIES = "https://www.pelispedia.tv/api/morex.php?rangeStart={0}&flagViewMore={1}&letra={2}&year={3}&genre={4}";
        

        public override Movies scrapMovies ()
        {
            Movies movies = new Movies();

            getMovieTitles (ref movies);

            for (int k = 0; k < movies.Count; k++)
            {
                Movie movie = movies[k];
                getMovieSources (ref movie);
                Console.WriteLine(" {0}", k + 1);
            }

            return movies;
        }


        void getMovieTitles (ref Movies movies)
        {
            int    index  = 1000;
            string buffer = string.Empty;
            string error  = string.Empty;

            while (HttpManager.requestGet(string.Format(URL_MOVIES, index, "true", "", "", ""), null, ref buffer, ref error))
            {
                foreach (string li in buffer.MatchRegexs("<li.*?</li>", false, true))
                {
                    string itemError = string.Empty;

                    if (!addParseHtmlMovie(li, ref movies, ref itemError))
                        logPelispedia("ERROR", itemError);

                    index++;

                    { // sacar
                        Console.WriteLine("[{0:0000}] Title: {1}", index, movies[movies.Count - 1].title);
                        //break;
                        //if (index == 200) break;
                    }
                }

                { // sacar
                    //break;
                    if (index > 1200) break;
                }
            }

            if (error.Length > 0)
                logPelispedia ("ERROR", "getMovieTitles -> " + error);
        }

        public void getMovieSources (ref Movie movie)
        {
            string       buffer          = string.Empty;
            string       keyMovie        = string.Empty;
            List<string> sources         = new List<string>();
            string       error           = string.Empty;
            string       urlDecoded      = string.Empty;

            if (0 == error.Length)
                HttpManager.requestGet(movie.url_web, null, ref buffer, ref error);

            if (0 == error.Length)
                PelispediaHelper.getKeyMovie(movie.url_web, buffer, ref keyMovie, ref error);

            if (0 == error.Length)
                PelispediaHelper.getUrlOptions(movie.url_web, keyMovie, ref buffer, ref error);

            if (0 == error.Length)
                PelispediaHelper.getSources(buffer, ref sources, ref error);

            if (0 == error.Length)
            {
                foreach (string source in sources)
                {
                    if (source.StartsWith("https://load.pelispedia.vip/embed"))
                    {
                        error = string.Empty;

                        if (decodeSource(source, ref urlDecoded, ref error))
                            ServerScrapper.scrap(urlDecoded, ref movie.sources, ref error);

                        if (error.Length > 0)
                            logPelispedia ("ERROR", string.Format("[{0}] {1}", movie.title, error));
                    }
                }
            }
            else
            {
                logPelispedia ("ERROR", string.Format("[{0}] {1}", movie.title, error));
            }
        }

        bool addParseHtmlMovie (string htmlCode, ref Movies movies, ref string error)
        {
            try
            {
                string url_web      = htmlCode.MatchRegex("<a href=\"([^\"]*)\" alt=");
                string title        = htmlCode.MatchRegex("<h2 class=\"mb10\">([^<]*)<br><span");
                string url_image    = htmlCode.MatchRegex("url=(https://[^\"]*)\" alt=");
                string description  = htmlCode.MatchRegex("<p class=\"font12\">([^<]*)</p>");

                if (url_web.Length == 0)
                    throw new Exception("Url web not found");

                if (title.Length == 0)
                    throw new Exception("Title not found");

                if (url_image.Length == 0)
                    throw new Exception("Url image not found");

                if (description.Length == 0)
                    throw new Exception("Description not found");

                Movie movie = new Movie(HelperMD5.calculateHashMD5(url_web));

                movie.title       = title;
                movie.url_image   = url_image;
                movie.url_web     = url_web;
                movie.description = description;

                movies.Add (movie);
            }
            catch (Exception ex)
            {
                error = "addParseHtmlMovie -> " + ex.Message;
            }

            return (0 == error.Length);
        }

        bool decodeSource (string urlCodedSource, ref string urlDecodedSource, ref string error)
        {
            string codigo = string.Empty;
            string buffer = string.Empty;
            string urlAux = string.Empty;

            HttpHeaders responseHeaders = new HttpHeaders();

            if (0 == error.Length)
                HttpManager.requestGet(urlCodedSource, null, ref buffer, ref error);

            if (0 == error.Length)
                PelispediaHelper.getCode(buffer, ref codigo, ref error);

            if (0 == error.Length)
                PelispediaHelper.decryptUrl(urlCodedSource, codigo, ref urlAux, ref error);

            if (0 == error.Length)
                HttpManager.requestGetSR(urlAux, null, ref buffer, ref responseHeaders, ref error);

            if (0 == error.Length)
                if (!responseHeaders.exist("Location"))
                    error = "Location header missing";

            if (0 == error.Length)
                urlDecodedSource = responseHeaders.value("Location");

            if (error.Length > 0)
                error = "decodeSource -> " + error;

            return (0 == error.Length);
        }

































        public override Series scrapSeries ()
        {
            throw new NotImplementedException();
        }



































        void logPelispedia (string title, string detail)
        {
            log ("pelispedia", title, detail);
        }
    }
}
