
namespace WebScrapper.Servers
{
    public static class ServerFactory
    {
        public static IServerScrapper dameScrapper (string servidor)
        {
            switch (servidor.ToLower())
            {
                case "Mega":            return new ServerScrapperMega       ();
                case "fembed.com":      return new ServerScrapperFembed     ();
                case "Streamango":      return new ServerScrapperStreamango ();
                case "rapidvideo.com":  return new ServerScrapperRapidvideo ();
                case "Vidoza":          return new ServerScrapperVidoza     ();
                case "openload.co":     return new ServerScrapperOpenload   ();
            }

            return null;
        }
    }
}
