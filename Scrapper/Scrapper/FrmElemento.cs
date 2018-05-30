using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Scrapper.Web.Scrappers.Pelispedia;
using Scrapper.Web.Entities;
using System.Diagnostics;


namespace Scrapper
{
    public partial class FrmElemento : Form
    {
        //Element element = null;

        //public FrmElemento (Element element)
        //{
        //    this.element = element;
        //    InitializeComponent();
        //}

        //public static void mostrar (Element element)
        //{
        //    FrmElemento frm = new FrmElemento(element);
        //    frm.ShowDialog();
        //    frm.Dispose();
        //}

        void FrmElemento_Load (object sender, EventArgs e)
        {
            //pictureBox1.Load(element.imagen);
            //label1.Text = element.nombre;
            //label2.Text = element.descripcion;

            //this.Refresh();

            //IWebScrap webScrap = new ScrapPelispedia();
            //PlayItems items = null;
            //string error = string.Empty;

            //webScrap.dameItems(element.id_pelispedia, ref items, ref error);

            //if (error.Length == 0)
            //{
            //    linkLabel1.Text = items[1].urlStream;
            //}
            //else
            //{
            //    MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        void linkLabel1_Click(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(linkLabel1.Text);
            Process.Start(sInfo);
        }

        private void cmdPlay_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.URL = linkLabel1.Text;
            axWindowsMediaPlayer1.Ctlcontrols.play();
        }
    }
}
