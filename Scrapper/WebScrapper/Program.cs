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
    }
}
