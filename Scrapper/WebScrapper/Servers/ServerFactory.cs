
namespace WebScrapper.Servers
{
    public static class ServerFactory
    {
        public static IServerScrapper dameScrapper (string servidor)
        {
            switch (servidor.ToLower())
            {
                case "streamango.com"     : return new ServerScrapperStreamango      ();
                case "rapidvideo.com"     : return new ServerScrapperRapidvideo      ();
                case "vidoza.net"         : return new ServerScrapperVidoza          ();
                case "openload.co"        : return new ServerScrapperOpenload        ();
                case "cloud.pelispedia.tv": return new ServerScrapperHtml5Pelispedia ();
            }

            return null;
        }
    }
}
