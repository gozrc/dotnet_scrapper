using System;
using System.Text;

namespace WebScrapper.Persistence
{
    class DbPersistence
    {
        public static string sqlInsertAudit (string title, string description, DateTime created)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append ("INSERT INTO ");
            sb.Append ("    Audit  (title, description, created) ");
            sb.Append ("VALUES ");
            sb.Append ("    ('{0}','{1}','{2}')");

            return string.Format(sb.ToString(), title, description, created.ToString());
        }

        public static string sqlInsertMovie (string title, string description, string urlImage, string urlWeb, DateTime created)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append ("INSERT INTO ");
            sb.Append ("    Movies  (title, description, url_image, url_web, created, modified, active) ");
            sb.Append ("VALUES ");
            sb.Append ("    ('{0}','{1}','{2}','{3}','{4}','{5}',{6})");

            return string.Format(sb.ToString(), title, description, urlImage, urlWeb, created.ToString(), created.ToString(), 1);
        }

        public static string sqlInsertSource (int idMovie, string serverName, string urlSource, string urlSubtitles, string description, string hash, DateTime created)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append ("INSERT INTO ");
            sb.Append ("    Sources  (id_movie, server_name, url_source, url_subtitles, description, hash, created, modified, active) ");
            sb.Append ("VALUES ");
            sb.Append ("    ({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8})");

            return string.Format(sb.ToString(), idMovie, serverName, urlSource, urlSubtitles, description, hash, created.ToString(), created.ToString(), 1);
        }

        public static string sqlSelectMovie (string urlWeb)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append ("SELECT ");
            sb.Append ("    id_movie ");
            sb.Append ("FROM ");
            sb.Append ("    Movies ");
            sb.Append ("WHERE ");
            sb.Append ("    Movies.url_web = '{0}'");

            return string.Format(sb.ToString(), urlWeb);
        }
    }
}
