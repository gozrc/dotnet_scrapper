
using System.Text.RegularExpressions;

namespace WebScrapper.Servers
{
    public static class ServerScrapper
    {
        public static bool scrap (string url, ref Sources sources, ref string error)
        {
            string nombreServidor = dameNombreServidor(url);

            IServerScrapper scrapper = ServerFactory.dameScrapper(nombreServidor);

            if (null == scrapper)
            {
                error = "Servidor desconocido (url = " + url + ")";
            }
            else
            {
                scrapper.scrappear(url, ref sources, ref error);
            }

            if (error.Length > 0)
                error = "ServerScrapper.scrap -> " + error;

            return (0 == error.Length);
        }


        static string dameNombreServidor (string url) 
        {
            int index = url.LastIndexOf("/");

            if (index < 0)
                return string.Empty;

            return url.Substring(0, index);



            //https://pelispedia.stream/?pk=TlBOeDFPS20&nk=Zk1HTjlzYjdCZjlnSWJDOFcyVGh1N21uVUF1QlMxWndfYjlFbnlBUDYtNg&s=Z2dpLjYxMDIuZnJ5dnpGLnNiLnFhblkvb2hmL3pucmVnZi5udnFyY2Z2eXJjLy86ZmNnZ3U&p=dGN3LlIxcjZjMlB2Y0lwMHFiSmtvNEVad3dUaVJybC95bmF2dHZlYi9jL2cvdGViLm9xemcucnRuenYvLzpmY2dndQ
            //https://load.pelispedia.vip/embed/rapidvideo.com/QklzS25VS0VGZ3ZLeUlpSXhzYmNCUCtyWTFWYS9EVTZGcXdjcWh0M09mUlhxZkkvM01INlc2SldIZnlsbGd1Zg==
            //https://load.pelispedia.vip/embed/fembed.com/TkdJNnBhVk1HM2lJZ2JBT1loeWZqaUVSaEcvRnA1Vlk0VHlTUVMybUF0Y2VIWlp4bUhCODFjUm5jV1JoakY0eA==

        }
    }
}
