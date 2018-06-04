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
            Console.WriteLine(movies[0].title);


            Console.ReadKey();
        }
    }
}
