using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Commons.CustomHttpManager;

namespace WebScrapper.Servers
{
    public class ServerScrapperFembed : IServerScrapper
    {
        public override bool scrappear(string url, ref Sources serverLinks, ref string error)
        {
            throw new NotImplementedException();
        }
    }
}
