
namespace WebScrapper.Servers
{
    public static class ServerFactory
    {
        public static IServerScrapper dameScrapper (string servidor)
        {
            switch (servidor.ToLower())
            {
                //case "https://pelispedia.stream":                        return new ServerScrapperMega       ();
                //case "https://load.pelispedia.vip/embed/fembed.com":     return new ServerScrapperFembed     ();
                case "streamango.com"     :                              return new ServerScrapperStreamango ();
                case "https://load.pelispedia.vip/embed/rapidvideo.com": return new ServerScrapperRapidvideo ();
                case "vidoza.net"         :                              return new ServerScrapperVidoza     ();
                case "https://load.pelispedia.vip/embed/openload.co":    return new ServerScrapperOpenload   ();
            }

            return null;
        }
    }
}
