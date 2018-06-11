using System;
using System.Configuration;

namespace WebScrapper
{
    public static class Config
    {
        public static string dbServer
        {
            get { return getValue<string>("db_server"); }
        }

        public static string dbName
        {
            get { return getValue<string>("db_name"); }
        }

        public static string dbUser
        {
            get { return getValue<string>("db_user"); }
        }

        public static string dbPassword
        {
            get { return getValue<string>("db_password"); }
        }


        static T getValue<T> (string campo)
        {
            try
            {
                string valor = ConfigurationManager
                    .AppSettings[campo].ToString();

                return (T)Convert.ChangeType(valor, typeof(T));
            }
            catch (Exception ex)
            {
                throw new Exception("Field [" + campo + "] missing in web.config: " + ex.Message);
            }
        }
    }
}
