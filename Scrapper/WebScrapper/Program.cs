using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commons.CustomHttpManager;

namespace WebScrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            string error = string.Empty;
            string requestUrl = "https://www.pelispedia.tv/pelicula/perturbada/";
            HttpHeaders requestHeaders = new HttpHeaders();
            byte[] requestData = new byte[0];

            int responseCode = 0;
            string responseDescription = string.Empty;
            string responseData = string.Empty;
            HttpHeaders responseHeaders = new HttpHeaders();


            if (HttpManager.request(requestUrl, Method.GET, requestHeaders, requestData, ref responseCode, ref responseDescription, ref responseData, ref responseHeaders, true, true, 2000, ref error))
                Console.WriteLine("OK");
            else
                Console.WriteLine("Error: " + error);

            Console.ReadKey();
        }
    }
}
