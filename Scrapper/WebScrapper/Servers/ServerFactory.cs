
namespace WebScrapper.Servers
{
    public static class ServerFactory
    {
        public static IServerScrapper dameScrapper (string servidor)
        {
            switch (servidor.ToLower())
            {
                case "fembed.com":      return new ServerScrapperFembed     ();
                case "streamango.com":  return new ServerScrapperStreamango ();
                case "rapidvideo.com":  return new ServerScrapperRapidvideo ();
                case "Vidoza":          return new ServerScrapperVidoza     ();
                case "openload.co":     return new ServerScrapperOpenload   ();
            }

            return null;
        }
    }
}
