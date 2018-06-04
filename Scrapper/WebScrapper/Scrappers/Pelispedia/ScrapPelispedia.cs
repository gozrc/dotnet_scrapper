using System;
using System.Text.RegularExpressions;
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
            }

            return movies;
        }


        void getMovieTitles (ref Movies movies)
        {
            int    index        = 0;
            string responseData = string.Empty;
            string error        = string.Empty;

            while (HttpManager.requestGet(string.Format(URL_MOVIES, index, "true", "", "", ""), null, ref responseData, ref error))
            {
                Regex rgxMovies = new Regex("<li(.*?)</li>", RegexOptions.Singleline);

                if (!rgxMovies.IsMatch(responseData))
                    break;

                foreach (Match match in rgxMovies.Matches(responseData))
                {
                    string itemError = string.Empty;

                    if (!addParseHtmlMovie(match.Value, ref movies, ref itemError))
                        logPelispedia("ERROR", itemError);

                    index++;

                    { // sacar
                        Console.WriteLine("[{0:0000}] Title: {1}", index, movies[movies.Count - 1].title);
                        //break;
                    }
                }

                { // sacar
                    break;
                }
            }

            if (error.Length > 0)
                logPelispedia ("ERROR", "getMovieTitles -> " + error);
        }

        void getMovieSources (ref Movie movie)
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
                PelispediaHelper.getSources(movie.url_web, buffer, ref sources, ref error);

            if (0 == error.Length)
            {
                foreach (string source in sources)
                {
                    error = string.Empty;

                    if (decodeSource(source, ref urlDecoded, ref error))
                        ServerScrapper.scrap(urlDecoded, ref movie.sources, ref error);

                    if (error.Length > 0)
                        logPelispedia ("ERROR", string.Format("getMovieSources -> {0} [{1}]", error, source));
                }
            }
            else
            {
                logPelispedia ("ERROR", string.Format("getMovieSources -> {0} [{1}]", error, movie.url_web));
            }
        }

        bool addParseHtmlMovie (string htmlCode, ref Movies movies, ref string error)
        {
            try
            {
                Regex rgxUrl = new Regex("<a(.*?)</a>", RegexOptions.Singleline);

                if (!rgxUrl.IsMatch(htmlCode))
                    throw new Exception("ErrorCode 0001");

                string aux = rgxUrl.Match(htmlCode).Value.Replace("\n", "");

                rgxUrl = new Regex("href=\"[^\"]+\"", RegexOptions.IgnoreCase);

                if (!rgxUrl.IsMatch(aux))
                    throw new Exception("ErrorCode 0002");

                string url_web = rgxUrl.Match(aux).Value.Split("\"".ToCharArray())[1];

                Regex rgxImage = new Regex("src=\"[^\"]+\"", RegexOptions.IgnoreCase);

                if (!rgxImage.IsMatch(aux))
                    throw new Exception("ErrorCode 0003");

                aux = rgxImage.Match(aux).Value.Split("\"".ToCharArray())[1];

                string url_image = aux.Substring(aux.IndexOf("url=") + 4);

                Regex rgxTitle = new Regex("<h2(.*?)<br>", RegexOptions.Singleline);

                if (!rgxTitle.IsMatch(htmlCode))
                    throw new Exception("ErrorCode 0004");

                string title = rgxTitle.Match(htmlCode).Value.Split("<>".ToCharArray())[2];

                Regex rgxDescription = new Regex("<p(.*?)</p>", RegexOptions.Singleline);

                if (!rgxDescription.IsMatch(htmlCode))
                    throw new Exception("ErrorCode 0005");

                string description = rgxDescription.Match(htmlCode).Value.Split("<>".ToCharArray())[2];

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
