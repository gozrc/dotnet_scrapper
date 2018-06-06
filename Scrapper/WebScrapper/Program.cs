using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commons.CustomHttpManager;
using WebScrapper.Entities;
using WebScrapper.Scrappers;
using WebScrapper.Scrappers.Pelispedia;

namespace WebScrapper
{
    class Program
    {
        static void Main (string[] args)
        {
            //testearPelicula("https://www.pelispedia.tv/pelicula/the-hollow-child/");

            IWebScrapper scrapper = new ScrapPelispedia();
            Movies movies = scrapper.scrapMovies();

            foreach (Movie m in movies)
            {
                Console.WriteLine();
                Console.WriteLine(m.title);

                foreach (WebScrapper.Servers.Source s in m.sources)
                    Console.WriteLine("\t" + s.name_server + " " + s.description);
            }

            Console.ReadKey();
        }

        static void testearPelicula (string urlPelicula)
        {
            Movie m = new Movie("id");
            m.url_web = urlPelicula;

            ScrapPelispedia s = new ScrapPelispedia();

            //s.getMovieSources(ref m);
        }
    }
}
