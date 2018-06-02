using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrapper.Web.Scrappers.Pelispedia
{
    class PelispediaHelper
    {
        public static bool getKeyMovie (string url, string buffer, ref string keyMovie, ref string error)
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
                error = "PelispediaHelper.getKeyMovie -> " + error;

            return (0 == error.Length);
        }

    }
}
