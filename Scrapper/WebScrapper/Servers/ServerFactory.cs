
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
                case "vidoza.net":      return new ServerScrapperVidoza     ();
                case "openload.co":     return new ServerScrapperOpenload   ();
                case "fastplay.to":     return new ServerScrapperFastPlay   ();
            }

            return null;
        }
    }
}
