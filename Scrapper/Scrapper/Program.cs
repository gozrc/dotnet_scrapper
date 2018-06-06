using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Scrapper.Web;
using Scrapper.Web.Scrappers;
using Scrapper.Web.Scrappers.Pelispedia;
using Scrapper.Web.Entities;
using Scrapper.Persistencia;


namespace Scrapper
{
    static class Program
    {
        [STAThread]
        static void Main_()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
        }

        static void Main (string[] args)
        {
            string error = string.Empty;

            string connectionString = string.Format("Data Source={0};Initial Catalog={1};User id={2};Password={3};Application Name=SsnService;MultipleActiveResultSets=True",
                "gozgvm.ddns.net", "VideoScrapDB", "usr_test", "logica22s1$");

            Commons.CustomDatabaseManager.CustomDatabase.ejecutar(connectionString, "select * from testtable", ref error);
        }
    }
}
