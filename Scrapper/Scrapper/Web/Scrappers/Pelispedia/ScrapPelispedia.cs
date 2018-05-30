using System;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Threading;
using Scrapper.Web.Servers;
using Scrapper.Web.Helpers;
using Scrapper.Web.Entities;


namespace Scrapper.Web.Scrappers.Pelispedia
{
    public class ScrapPelispedia : IWebScrap
    {
        const string URL_BASE = "https://www.pelispedia.tv/";

        MD5 md5 = new MD5CryptoServiceProvider();

        bool cancel = false;


        public override bool scrapMovies (ref Movies movies, ref string error)
        {
            if (null == movies)
                movies = new Movies();

            if (0 == error.Length)
                getMovieTitles(ref movies, ref error);

            if (0 == error.Length)
                getMovies(ref movies, ref error);

            if (cancel)
                throwCancelScrap();

            return (0 == error.Length);
        }

        public override bool scrapSeries (ref Series series, ref string error)
        {
            if (null == series)
                series = new Series();

            if (0 == error.Length)
                getSerieTitles(ref series, ref error);

            if (0 == error.Length)
                getEpisodes(ref series, ref error);

            if (cancel)
                throwCancelScrap();

            return (0 == error.Length);
        }

        public override void cancelScrap ()
        {
            cancel = true;
        }

        public override void scrapMoviesAsync (string path)
        {



            
        }

        public override void scrapSeriesAsync (string path)
        {
            
        }


        bool getMovieTitles (ref Movies movies, ref string error)
        {
            string buffer = string.Empty;
            int    index  = 0;

            try
            { 
                while (HttpHelper.requestGet(URL_BASE + "api/more.php?rangeStart=" + index.ToString(), null, ref buffer, ref error))
                {
                    Regex rgx = new Regex("<li(.*?)</li>", RegexOptions.Singleline);

                    if (!rgx.IsMatch(buffer))
                        break;

                    foreach (Match match in rgx.Matches(buffer))
                    {
                        Regex rgx1 = new Regex("<a(.*?)</a>", RegexOptions.Singleline);

                        if (!rgx1.IsMatch(match.Value))
                            throw new Exception("Scrapper Pelispedia desactualziado: Error 1");

                        string aux = rgx1.Match(match.Value).Value.Replace("\n", "");

                        rgx1 = new Regex("href=\"[^\"]+\"", RegexOptions.IgnoreCase);

                        if (!rgx1.IsMatch(aux))
                            throw new Exception("Scrapper Pelispedia desactualziado: Error 2");

                        string url_web = rgx1.Match(aux).Value.Split("\"".ToCharArray())[1];

                        rgx1 = new Regex("src=\"[^\"]+\"", RegexOptions.IgnoreCase);

                        if (!rgx1.IsMatch(aux))
                            throw new Exception("Scrapper Pelispedia desactualziado: Error 3");

                        string url_image = rgx1.Match(aux).Value.Split("\"".ToCharArray())[1];

                        rgx1 = new Regex("<h2(.*?)<br>", RegexOptions.Singleline);

                        if (!rgx1.IsMatch(match.Value))
                            throw new Exception("Scrapper Pelispedia desactualziado: Error 4");

                        string title = rgx1.Match(match.Value).Value.Split("<>".ToCharArray())[2];

                        rgx1 = new Regex("<p(.*?)</p>", RegexOptions.Singleline);

                        if (!rgx1.IsMatch(match.Value))
                            throw new Exception("Scrapper Pelispedia desactualziado: Error 5");

                        string id = Helper.calculateHashMD5(url_web); 

                        if (!movies.contains(id))
                        {
                            Movie movie = new Movie(id);

                            movie.title         = title;
                            movie.url_image     = url_image;
                            movie.url_web       = url_web;
                            movie.description   = rgx1.Match(match.Value).Value.Split("<>".ToCharArray())[2];

                            movies.Add (movie);
                        }

                        index++;
                    }

                    if (cancel)
                        break;
                }

                if (error.Length == 0)
                    throwMoviesTotal(movies.Count);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "ScrapPelispedia.getMovieTitles -> " + error;

            return (0 == error.Length);
        }

        bool getMovies (ref Movies movies, ref string error)
        {
            for (int k = 0; k < movies.Count; k++)
            {
                Movie movie = movies[k];

                if (movie.sources.Count == 0)
                {
                    if (!getMovie(ref movie, ref error))
                        break;
                }

                throwMoviesProgress(k+1);

                if (cancel)
                    break;
            }

            if (error.Length > 0)
                error = "ScrapPelispedia.getMovies -> " + error;

            return (0 == error.Length);
        }

        bool getMovie (ref Movie movie, ref string error)
        {
            string       buffer   = string.Empty;
            string       keyMovie = string.Empty;
            List<string> sources  = new List<string>();
            string       codigo   = string.Empty;
            string       urlAux   = string.Empty;

            if (0 == error.Length)
                HttpHelper.requestGet(movie.url_web, null, ref buffer, ref error);

            if (0 == error.Length)
                getKeyMovie(movie.url_web, buffer, ref keyMovie, ref error);

            if (0 == error.Length)
                getUrlOptions(movie.url_web, keyMovie, ref buffer, ref error);

            if (0 == error.Length)
                getSources(movie.url_web, buffer, ref sources, ref error);

            if (0 == error.Length)
            {
                foreach (string source in sources)
                {
                    string url = source;

                    if (!isHtml5(source))
                    {
                        if (!HttpHelper.requestGet(source, null, ref buffer, ref error))
                            break;

                        if (!getCode(buffer, ref codigo, ref error))
                            break;

                        if (!decryptUrl(source, codigo, ref urlAux, ref error))
                            break;

                        if (!HttpHelper.requestGetSinReditect(urlAux, null, ref url, ref error))
                            break;
                    }

                    if (!ServerScrapper.scrap(url, ref movie.sources, ref error))
                        break;
                }
            }

            if (error.Length > 0)
                error = "ScrapPelispedia.getMovie -> " + error;

            return (0 == error.Length);
        }


        bool getSerieTitles (ref Series series, ref string error)
        {
            string  buffer = string.Empty;
            int     index  = 0;

            try
            {
                while (HttpHelper.requestGet(URL_BASE + "api/more.php?rangeStart=" + index.ToString() + "&tipo=serie", null, ref buffer, ref error))
                {
                    Regex rgx = new Regex("<li(.*?)</li>", RegexOptions.Singleline);

                    if (!rgx.IsMatch(buffer))
                        break;

                    foreach (Match match in rgx.Matches(buffer))
                    {
                        Regex rgx1 = new Regex("<a(.*?)</a>", RegexOptions.Singleline);

                        if (!rgx1.IsMatch(match.Value))
                            throw new Exception("Scrapper Pelispedia desactualziado: Error 1");

                        string aux = rgx1.Match(match.Value).Value.Replace("\n", "");

                        rgx1 = new Regex("href=\"[^\"]+\"", RegexOptions.IgnoreCase);

                        if (!rgx1.IsMatch(aux))
                            throw new Exception("Scrapper Pelispedia desactualziado: Error 2");

                        string url_web = rgx1.Match(aux).Value.Split("\"".ToCharArray())[1];

                        rgx1 = new Regex("src=\"[^\"]+\"", RegexOptions.IgnoreCase);

                        if (!rgx1.IsMatch(aux))
                            throw new Exception("Scrapper Pelispedia desactualziado: Error 3");

                        string url_image = rgx1.Match(aux).Value.Split("\"".ToCharArray())[1];

                        rgx1 = new Regex("<h2(.*?)<br>", RegexOptions.Singleline);

                        if (!rgx1.IsMatch(match.Value))
                            throw new Exception("Scrapper Pelispedia desactualziado: Error 4");

                        string title = rgx1.Match(match.Value).Value.Split("<>".ToCharArray())[2];

                        rgx1 = new Regex("<p(.*?)</p>", RegexOptions.Singleline);

                        if (!rgx1.IsMatch(match.Value))
                            throw new Exception("Scrapper Pelispedia desactualziado: Error 5");

                        string id = Helper.calculateHashMD5(url_web);

                        Serie serie = null;

                        if (!series.contains(id))
                        {
                            serie = new Serie(id);

                            serie.title         = title;
                            serie.description   = rgx1.Match(match.Value).Value.Split("<>".ToCharArray())[2];
                            serie.url_web       = url_web;
                            serie.url_image     = url_image;

                            series.Add(serie);
                        }
                        else
                        {
                            serie = series.search(id);
                        }

                        if (!getSeasons(ref serie, ref error))
                            throw new Exception(error);

                        index++;
                    }

                    if (cancel)
                        break;
                }

                if (error.Length == 0)
                    throwSeriesTotal(series.episodesCount);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "ScrapPelispedia.getSeriesTitles -> " + error;

            return (0 == error.Length);
        }

        bool getSeasons (ref Serie serie, ref string error)
        {
            string buffer = string.Empty;

            try
            {
                Regex rgxHeader    = new Regex("<header class=\"container\">(.*?)</header>", RegexOptions.Singleline);
                Regex rgxImagen    = new Regex("<img data-cfsrc=\"(.+)\"", RegexOptions.Singleline);
                Regex rgxLista     = new Regex("<ul class(.*?)</ul>", RegexOptions.Singleline);
                Regex rgxEpisodio  = new Regex("<li(.*?)</li>", RegexOptions.Singleline);
                Regex rgxDireccion = new Regex("<a href=\"(.+)\"", RegexOptions.Singleline);
                Regex rgxNombre    = new Regex("<small>(.+)</small>", RegexOptions.Singleline);

                if (!HttpHelper.requestGet(serie.url_web, null, ref buffer, ref error))
                    throw new Exception(error);

                if (!rgxHeader.IsMatch(buffer))
                    throw new Exception(string.Format("No se encontraron temporadas (url = {0})", serie.url_web));

                foreach (Match header in rgxHeader.Matches(buffer))
                {
                    Regex rgxTemporada = new Regex("<h3 class=\"(.*?)\">(.*?)<span", RegexOptions.Singleline);

                    string titleSeason = rgxTemporada.Match(header.Value).Value.Split("<>".ToCharArray())[2].Replace("\n", "");
                    string idSeason    = Helper.calculateHashMD5(serie.url_web + titleSeason);

                    Season season = null;

                    if (!serie.seasons.contains(idSeason))
                    {
                        season = new Season(idSeason);

                        season.title      = titleSeason;
                        season.url_imagen = rgxImagen.Match(buffer, header.Index + header.Length).Value.Split("\"".ToCharArray())[1];
                        season.url_web    = serie.url_web;
                    }
                    else
                    {
                        season = serie.seasons.search(idSeason);
                    }

                    if (!rgxLista.IsMatch(buffer, header.Index + header.Length))
                        throw new Exception(string.Format("No se encontraron episodios para la {0} (url = {1})", season.title, season.url_web));

                    Match lista = rgxLista.Match(buffer, header.Index + header.Length);

                    foreach (Match episodio in rgxEpisodio.Matches(lista.Value))
                    {
                        if (!rgxDireccion.IsMatch(episodio.Value))
                            throw new Exception(string.Format("No se encontró un episodio para la {0} (url = {1})", season.title, serie.url_web));

                        if (!rgxNombre.IsMatch(episodio.Value))
                            throw new Exception(string.Format("No se encontró un episodio para la {0} (url = {1})", season.title, serie.url_web));

                        string url_web_episode  = rgxDireccion.Match(episodio.Value).Value.Split("\"".ToCharArray())[1];
                        string idEpisode        = Helper.calculateHashMD5(url_web_episode);

                        if (!season.episodes.contains(idEpisode))
                        {
                            Episode episode = new Episode(idEpisode);

                            episode.title   = rgxNombre.Match(episodio.Value).Value.Split("<>".ToCharArray())[2];
                            episode.url_web = url_web_episode;

                            season.episodes.Add(episode);
                        }
                    }

                    season.episodes.Sort((x, y) => x.title.CompareTo(y.title));

                    if (!serie.seasons.contains(season.id))
                        serie.seasons.Add(season);
                }

                if (serie.seasons.Count > 0)
                    serie.seasons.Sort((x, y) => x.title.CompareTo(y.title));
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "ScrapPelispedia.getSeasons -> " + error;

            return (0 == error.Length);
        }

        bool getEpisodes (ref Series series, ref string error)
        {
            int count = 1;

            foreach (Serie serie in series)
            {
                foreach (Season season in serie.seasons)
                {
                    for (int k = 0; k < season.episodes.Count; k++)
                    {
                        Episode episode = season.episodes[k];

                        if (episode.sources.Count == 0)
                        {
                            if (!getEpisode(ref episode, ref error))
                                break;
                        }

                        throwSeriesProgress(count++);

                        if (cancel)
                            break;
                    }
                }
            }

            if (error.Length > 0)
                error = "ScrapPelispedia.getEpisodes -> " + error;

            return (0 == error.Length);
        }

        bool getEpisode (ref Episode episode, ref string error)
        {
            string       buffer   = string.Empty;
            string       keyMovie = string.Empty;
            List<string> sources  = new List<string>();
            string       codigo   = string.Empty;
            string       urlAux   = string.Empty;

            if (0 == error.Length)
                HttpHelper.requestGet(episode.url_web, null, ref buffer, ref error);

            if (0 == error.Length)
                getKeyMovie(episode.url_web, buffer, ref keyMovie, ref error);

            if (0 == error.Length)
                getUrlOptions(episode.url_web, keyMovie, ref buffer, ref error);

            if (0 == error.Length)
                getSources(episode.url_web, buffer, ref sources, ref error);

            if (0 == error.Length)
            {
                foreach (string source in sources)
                {
                    string url = source;

                    if (!isHtml5(source))
                    {
                        if (!HttpHelper.requestGet(source, null, ref buffer, ref error))
                            break;

                        if (!getCode(buffer, ref codigo, ref error))
                            break;

                        if (!decryptUrl(source, codigo, ref urlAux, ref error))
                            break;

                        if (!HttpHelper.requestGetSinReditect(urlAux, null, ref url, ref error))
                            break;
                    }

                    if (!ServerScrapper.scrap(url, ref episode.sources, ref error))
                        break;
                }
            }

            if (error.Length > 0)
                error = "ScrapPelispedia.getEpisode -> " + error;

            return (0 == error.Length);
        }


        bool getKeyMovie (string url, string buffer, ref string keyMovie, ref string error)
        {
            try
            {
                Regex rgx = new Regex("api/iframes.php[?]id=[0-9]+", RegexOptions.IgnoreCase);

                if (!rgx.IsMatch(buffer))
                    throw new Exception("No se encontró el iframe de reproduccion (url = " + url + ")");

                keyMovie = rgx.Match(buffer).Value.Split('=')[1];
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "ScrapPelispedia.getKeyMovie -> " + error;

            return (0 == error.Length);
        }

        bool getUrlOptions (string urlMovie, string keyMovie, ref string buffer, ref string error)
        {
            Headers headers = new Headers();
            headers.Add(new Header("Referer", urlMovie));

            string url = string.Format("{0}api/iframes.php?id={1}&update1.1", URL_BASE, keyMovie);
            
            HttpHelper.requestGet(url, headers, ref buffer, ref error);

            if (error.Length > 0)
                error = "ScrapPelispedia.getUrlOptions -> " + error;

            return (0 == error.Length);
        }

        bool getSources (string url, string buffer, ref List<string> sources, ref string error)
        {
            try
            {
                Regex rgxOptions = new Regex(
                    "<a href=\"((https://cloud.pelispedia.tv/html5.php)|(https://load.pelispedia.co/embed/)|(https://www.pelispedia.tv/api/calidades)|(https://www1.pelispedia.tv/api/html5.php?)).+>",
                    RegexOptions.IgnoreCase
                );

                if (!rgxOptions.IsMatch(buffer))
                    throw new Exception("No se encontraron fuentes para la película (url = " + url + ")");

                foreach (Match optionMatch in rgxOptions.Matches(buffer))
                {
                    string link = optionMatch.Value.Split('\"')[1];

                    if (!link.StartsWith("https:"))
                        link = "https:" + link;

                    sources.Add(link);
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "ScrepPelispedia.getSources -> " + error;

            return (0 == error.Length);
        }

        bool decryptUrl (string urlBase, string codigo, ref string url, ref string error)
        {
            url = urlBase.Replace("embed", "stream") + "/"
                + (new AES()).OpenSSLEncrypt2(codigo, "_KeyCryptBlock");

            return true;
        }

        bool getCode (string buffer, ref string code, ref string error)
        {
            try
            {
                Regex rgx = new Regex("<meta name=\"google-site-verification.+>", RegexOptions.IgnoreCase);

                if (!rgx.IsMatch(buffer))
                    throw new Exception("No se encontró el codigo de la película.");

                code = "\"" + rgx.Match(buffer).Value.Split('\"')[3] + "\"";
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0)
                error = "ScrapPelispedia.getCode -> " + error;

            return (0 == error.Length);
        }        

        bool isHtml5 (string source)
        {
            return (source.IndexOf("cloud.pelispedia.tv") > 0);
        }
    }
}