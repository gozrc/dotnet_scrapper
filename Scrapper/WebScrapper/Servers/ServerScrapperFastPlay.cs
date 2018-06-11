using System.Collections.Generic;
using Commons.CustomHttpManager;

namespace WebScrapper.Servers
{
    public class ServerScrapperFastPlay : IServerScrapper
    {
        public override string name ()
        {
            return "FASTPLAY";
        }

        public override bool scrappear (string url, ref Sources serverLinks, ref string error)
        {
            string buffer  = string.Empty;
            string urlSubs = string.Empty;

            List<string> urlVideos = new List<string>();
            List<string> urlDescs  = new List<string>();

            error = "Falta verificar este servidor";

            if (0 == error.Length)
                HttpManager.requestGet(url, null, ref buffer, ref error);

            if (0 == error.Length)
                getUrlSubs(buffer, ref urlSubs, ref error);

            if (0 == error.Length)
                getUrlVideos(buffer, ref urlVideos, ref urlDescs, ref error);

            if (0 == error.Length)
                for (int k = 0; k < urlVideos.Count; k++)
                    serverLinks.Add(new Source(name(), urlVideos[k], urlSubs, urlDescs[k]));

            return (0 == error.Length);
        }


        bool getUrlSubs (string buffer, ref string urlSubs, ref string error)
        {

            /*
                <script type='text/javascript'>eval(function(p,a,c,k,e,d){while(c--)if(k[c])p=p.replace(new RegExp('\\b'+c.toString(a)+'\\b','g'),k[c]);return p}('7("2m").2l({2k:[{d:"6://e.3.2/2j/v.q",c:"2i","2h":"o"},{d:"6://e.3.2/2g/v.q",c:"2f"}],2e:"6://e.3.2/i/2d/n/g.2c",2b:"2a",29:"28%",27:"26",25:"16:9",24:"p",23:"p",22:"o",21:"20",1z:[{d:"/m/n/1y.m",c:"1x",1w:"l"}],l:{1v:\'#1u\',1t:15,1s:"1r",1q:0},1p:"1o",1n:"3.2 - 1m 1l 1k",1j:"6://3.2"});1i 8,b;7().1h(4(x){h(5>0&&x.1g>=5&&b!=1){b=1;$(\'a.1f\').1e(\'1d\')}});7().1c(4(x){k(x)});7().1b(4(){$(\'a.j\').1a()});4 k(x){$(\'a.j\').19();h(8)18;8=1;$.17(\'6://3.2/14?13=12&11=g&10=z-y-w-u-t\',4(f){$(\'#s\').r(f)})}',36,95,'||to|fastplay|function||http|jwplayer|vvplay||div|vvad|label|file|www004|data|mztxbd4wvqcx|if||video_ad|doPlay|captions|srt|00007|true|none|mp4|html|fviews|0ea96451d42055d33ff867f1690ed123|1528671706||228||181|36490|hash|file_code|view|op|dl|||get|return|hide|show|onComplete|onPlay|slow|fadeIn|video_ad_fadein|position|onTime|var|aboutlink|Hosting|Video|HD|abouttext|beelden|skin|backgroundOpacity|Verdana|fontFamily|fontSize|FFFFFF|color|kind|Spanish|mztxbd4wvqcx_Spanish|tracks|start|startparam|androidhls|preload|primary|aspectratio|450|height|100|width|5632|duration|jpg|03|image|720p|2tdv4lj5mqjtgz575os3r66recfnbr6lx6aj5cto7c6mlhlsjvqxgvqjv4gq|default|360p|2tdv4lj5mqjtgz575os3r66recfnbr6lx6aj5cto726mlhlsjvqydxoyiqjq|sources|setup|vplayer'.split('|')))
                </script>
            */

            string p = buffer.MatchRegex("<script type='text/javascript'>(eval.*)");




            urlSubs = buffer.MatchRegex("<track kind=\"subtitles\" src=\"(https://[^\"]*)\" srclang");

            if (urlSubs.Length == 0)
                error = "Subtitles link not found";

            return (0 == error.Length);
        }

        bool getUrlVideos (string buffer, ref List<string> urlVideos, ref List<string> urlDescs, ref string error)
        {
            urlVideos = new List<string>();
            urlDescs  = new List<string>();

            urlVideos.AddRange (buffer.MatchRegexs("src: \"(https://[^\"]*)\", type:"));
            urlDescs.AddRange  (buffer.MatchRegexs("type: 'video/mp4', label:'[^']*', res:'([^']*)'"));

            if (urlVideos.Count == 0)
                error = "Link video not found";

            return (0 == error.Length);
        }
    }
}
