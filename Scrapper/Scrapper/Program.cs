using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Scrapper.Web;
using Scrapper.Web.Scrappers;
using Scrapper.Web.Scrappers.Pelispedia;
using Scrapper.Web.Entities;
using Scrapper.Persistencia;


namespace Scrapper
{
    static class Program
    {
        [STAThread]
        static void Main_()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
        }

        static void Main (string[] args)
        {
            string error = string.Empty;

            IWebScrap scrapper = new ScrapPelispedia();



            Movies movies = new Movies();

            if (0 == error.Length)
                Library.loadMovies(ref movies, "pelicula.xml", ref error);

            if (0 == error.Length)
                scrapper.scrapMovies(ref movies, ref error);

            if (0 == error.Length)
                Library.saveMovies(movies, "pelicula.xml", ref error);

            if (0 == error.Length)
                Console.WriteLine("Ok");
            else
                Console.WriteLine("Error: " + error);
        }
    }
}
