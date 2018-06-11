using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Commons.CustomHttpManager;
using WebScrapper.Entities;
using WebScrapper.Scrappers;
using WebScrapper.Scrappers.Pelispedia;
using Commons.CustomDatabaseManager;

namespace WebScrapper
{
    class Program
    {
        static void Main (string[] args)
        {
            // ---------------------------------------------------------------
            //testearBaseDeDatos();
            // ---------------------------------------------------------------

            // ---------------------------------------------------------------
            //testearPelicula("https://www.pelispedia.tv/pelicula/ibiza/", "Ibiza");
            //testearPelicula("https://www.pelispedia.tv/pelicula/entre-sombras/", "Entre Sombras");
            //Console.ReadKey();
            //return;
            // ---------------------------------------------------------------

            // ---------------------------------------------------------------
            IWebScrapper scrapper = new ScrapPelispedia();
            Movies movies = scrapper.scrapMovies();

            foreach (Movie m in movies)
            {
                //Console.WriteLine();
                Console.WriteLine(m.title + " " + m.sources.Count().ToString());

                //foreach (WebScrapper.Servers.Source s in m.sources)
                //    Console.WriteLine("\t" + s.name_server + " " + s.description);
            }
            // ---------------------------------------------------------------

            Console.ReadKey();
        }

        static void testearPelicula (string urlPelicula, string title)
        {
            Movie m = new Movie("id");
            m.url_web = urlPelicula;
            m.title = title;

            ScrapPelispedia s = new ScrapPelispedia();
            s.getMovieSources(ref m);
        }

        static void testearBaseDeDatos ()
        {
            string error = string.Empty;

            CustomDatabase customDb = new CustomDatabase(
                Config.dbServer, Config.dbName, Config.dbUser, Config.dbPassword);

            if (customDb.open(ref error))
            {
                string sql = Persistence.DbPersistence.sqlInsertSource(1, "Servidor", "direccion_pelicula", "direccion_subtitulo", "descripcion pelicula", "HASDASDASDASDAS", DateTime.Now);

                if (customDb.execute(sql, ref error))
                    Console.WriteLine("Ok");
            }

            customDb.close();

            if (error.Length > 0)
                Console.WriteLine(error);
        }
    }
}
