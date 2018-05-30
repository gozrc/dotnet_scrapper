using System.Text;
using System.Security.Cryptography;


namespace Scrapper.Web.Helpers
{
    public class Helper
    {
        public static string calculateHashMD5 (string texto)
        {
            MD5 md5 = MD5.Create();

            byte[] inputBytes = Encoding.UTF8.GetBytes(texto);
            byte[] hash       = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("X2"));

            return sb.ToString();
        }
    }
}
