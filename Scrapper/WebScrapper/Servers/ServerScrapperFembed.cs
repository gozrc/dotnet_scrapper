using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Commons.CustomHttpManager;

namespace WebScrapper.Servers
{
    public class ServerScrapperFembed : IServerScrapper
    {
        public override string name ()
        {
            return "FEMBED";
        }

        public override bool scrappear(string url, ref Sources serverLinks, ref string error)
        {
            error = "sdf";
            return false;
        }
    }
}
