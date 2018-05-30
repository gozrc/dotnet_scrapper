using System;
using Scrapper.Web.Entities;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace Scrapper.Persistencia
{
    public static class Library
    {
        public static bool loadMovies (ref Movies movies, string filepath, ref string error)
        {
            try
            {
                movies = new Movies();

                if (File.Exists(filepath))
                {
                    BinaryFormatter bf = new BinaryFormatter();

                    using (FileStream fs = File.OpenRead(filepath))
                        movies = (Movies)bf.Deserialize(fs);

                    bf = null;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return (0 == error.Length);
        }

        public static bool saveMovies (Movies movies, string filepath, ref string error)
        {
            try
            {
                if (File.Exists(filepath))
                    File.Delete(filepath);

                BinaryFormatter bf = new BinaryFormatter();

                using (FileStream fs = File.Create(filepath))
                    bf.Serialize(fs, movies);

                bf = null;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return (0 == error.Length);
        }
    }
}
