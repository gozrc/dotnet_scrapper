using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scrapper.Web.Entities;
using System.IO;
using System.Xml.Serialization;


namespace Scrapper.Persistencia
{
    public static class Conversion
    {
        public static bool getMovies (ref Movies movies, string filepath, ref string error)
        {
            try
            {
                movies = new Movies();

                if (File.Exists(filepath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Movies));

                    FileStream fileStream = new FileStream(filepath, FileMode.Open);

                    movies = (Movies)serializer.Deserialize(fileStream);

                    fileStream.Close();
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
                string filepathold = filepath + ".OLD";

                if (File.Exists(filepathold))
                    File.Delete(filepathold);

                if (File.Exists(filepath))
                    File.Move(filepath, filepathold);

                XmlSerializer serializer = new XmlSerializer(typeof(Movies));

                StreamWriter streamWriter = new StreamWriter(filepath);

                serializer.Serialize(streamWriter, movies);

                streamWriter.Close();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return (0 == error.Length);
        }
    }
}
